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
using ThirdTask.Database;

namespace ThirdTask
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<BitmapSource> _images = new();
        private Dictionary<string, int> ImagePathToIndex = new();

        CancellationTokenSource cancelTokenSource = new();
        CancellationToken token;

        private bool isAnalyzing = false;
        private ObjectRecognitionLibrary.ObjectRecognitionLibrary objectRecognitionLibrary = new();
        private ImageDatabase db = new();
        
        public ObservableCollection<BitmapSource> Images
        {
            get { return _images; }
        }

        private void LoadDataFromDatabase()
        {
            db.LoadImages(Images);
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            token = cancelTokenSource.Token;
            LoadDataFromDatabase();
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
                            var newBitmap = ObjectRectangles.Draw(Helpers.BitmapFromBitmapSource(Images[imageIndex]), imageData.boundingBoxes);
                            BitmapSource image = Helpers.BitmapSourceFromBitmap(newBitmap);
                            image.Freeze();

                             Dispatcher.Invoke(() =>
                             {
                                Images[imageIndex] = image;
                             });

                            Collection<BoundingBox> boundingBoxes = new();

                            foreach (var bb in imageData.boundingBoxes)
                            {
                                boundingBoxes.Add(new BoundingBox
                                {
                                    Label = bb.Label,
                                    Confidence = bb.Confidence,
                                    x1 = bb.BBox[0],
                                    y1 = bb.BBox[1],
                                    x2 = bb.BBox[2],
                                    y2 = bb.BBox[3],

                                });
                            }
                            db.SaveImage(image, boundingBoxes);
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

        private void ClearDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            db.Clear();

            Dispatcher.Invoke(() =>
            {
                Images.Clear();
            });
        }
    }
}
