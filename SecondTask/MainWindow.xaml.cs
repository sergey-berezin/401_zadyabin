using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Concurrent;
using ObjectRecognitionLibrary.DataStructures;
using System.Threading.Tasks;
using ObjectRecognitionLibrary;
using System.Threading;
using System.Collections.Generic;

namespace SecondTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        private ObservableCollection<BitmapSource> _images = new();
        private Dictionary<string, int> ImagePathToIndex = new();

        CancellationTokenSource cancelTokenSource = new();
        CancellationToken token;

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
            if (imagesFolder.Length > 0)
            {
                var results = new BlockingCollection<ImageData>();

                Task.Factory.StartNew(() => ObjectRecognitionLibrary.ObjectRecognitionLibrary.AnalyseFolder(imagesFolder, results, token),
                    TaskCreationOptions.LongRunning);

                Task.Factory.StartNew(() => {
                    ImageData imageData;
                    while (!results.IsCompleted)
                    {
                        if (results.TryTake(out imageData))
                        {
                            var imageIndex = ImagePathToIndex[imageData.imagePath];
                            var newBitmap = ObjectRectangles.Draw(Helpers.BitmapFromBitmapSource(Images[imageIndex]), imageData.boundingBoxes);
                            Dispatcher.Invoke(() =>
                            {
                                Images[imageIndex] = Helpers.BitmapSourceFromBitmap(newBitmap);
                            });
                        }
                    }
                });
           }
        }

        private void StartObjectRecognitionButton_Click(object sender, RoutedEventArgs e)
        {
            StartObjectRecognition();
        }
    }
}
