using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Concurrent;
using ObjectRecognitionLibrary.DataStructures;
using ObjectRecognitionLibrary;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;


namespace ThirdTask
{
    class LibraryContext : DbContext
    {
        public DbSet<AnalyzedImage> AnalyzedImages { get; set; }

        public string DbPath { get; }

        public LibraryContext()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            DbPath = System.IO.Path.Join(projectDirectory, "library.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
            => o.UseLazyLoadingProxies().UseSqlite($"Data Source={DbPath}");
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<BitmapSource> _images = new();
        private Dictionary<string, int> ImagePathToIndex = new();

        CancellationTokenSource cancelTokenSource = new();
        CancellationToken token;

        private bool isAnalyzing = false;
        private ObjectRecognitionLibrary.ObjectRecognitionLibrary objectRecognitionLibrary = new();

        public ObservableCollection<BitmapSource> Images
        {
            get { return _images; }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            token = cancelTokenSource.Token;
        }
        private void StopAnalyzingButton_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();
            isAnalyzing = false;
        }

        public string imagesFolder;

        private void ChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result.ToString() != string.Empty && dialog.SelectedPath != string.Empty)
            {
                PathTextBlock.Text = dialog.SelectedPath;

                imagesFolder = dialog.SelectedPath;

                List<FileInfo> info = new DirectoryInfo(dialog.SelectedPath).GetFiles("*.*", SearchOption.AllDirectories).ToList();

                Images.Clear();

                for (int i = 0; i < info.Count; i++)
                {
                    Images.Add(new BitmapImage(new Uri(info[i].FullName)));
                    ImagePathToIndex[info[i].FullName] = i;
                }
            }
        }

        private void StartObjectRecognition()
        {
            isAnalyzing = true;
            if (imagesFolder.Length > 0)
            {
                var results = new BlockingCollection<ImageData>();

                Task.Factory.StartNew(() => objectRecognitionLibrary.AnalyseFolder(imagesFolder, results, token),
                    TaskCreationOptions.LongRunning);

                Task.Factory.StartNew(() => {
                    ImageData imageData;
                    while (!results.IsCompleted)
                    {
                        if (results.TryTake(out imageData))
                        {
                            var imageIndex = ImagePathToIndex[imageData.imagePath];

                            Dispatcher.Invoke(() =>
                            {
                                var newBitmap = ObjectRectangles.Draw(Helpers.BitmapFromBitmapSource(Images[imageIndex]), imageData.boundingBoxes);
                                Images[imageIndex] = Helpers.BitmapSourceFromBitmap(newBitmap);

                                Collection<BoundingBox> boundingBoxes = new();

                                foreach(var bb in imageData.boundingBoxes)
                                {
                                    boundingBoxes.Add(new BoundingBox { 
                                        Label = bb.Label, 
                                        Confidence = bb.Confidence, 
                                        x1 = bb.BBox[0],
                                        y1 = bb.BBox[1],
                                        x2 = bb.BBox[2],
                                        y2 = bb.BBox[3],
                                        
                                    });
                                }
                                    
                                var db = new LibraryContext();
                                db.AnalyzedImages.Add(new AnalyzedImage { Image = Helpers.BitmasSourceToByteArray(Images[imageIndex]), BoundingBoxes = boundingBoxes });
                                db.SaveChanges();
                            });
                        }
                    }
                    isAnalyzing = false;
                });
               
            }
        }

        private void StartObjectRecognitionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnalyzing)
            {
                StartObjectRecognition();
            }
        }
    }
}
