using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace SecondTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

   

    public partial class MainWindow : Window
    {

        // Class defintion should be provided within the namespace being used, outside of any other classes.
        // These two declarations belong outside of the main page class.
        private ObservableCollection<ImageSource> _images = new ObservableCollection<ImageSource>();

        public ObservableCollection<ImageSource> Images
        {
            get { return this._images; }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result.ToString() != string.Empty)
                {
                    PathTextBlock.Text = dialog.SelectedPath;

                    var dirInfo = new DirectoryInfo(dialog.SelectedPath);

                    FileInfo[] info = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
                   
                    var targetList = info
                                 .Select(x => ListOfImages.Items.Add(x.Name)).ToList();

                  info.ToList().ForEach(imagePath => Images.Add(Helpers.ImageSourceFromBitmap(new Bitmap(Image.FromFile(imagePath.FullName)))));

                    imagePlaceholder.Source = Images[0];

                }
            }
        }

        private void ListOfImages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void Photos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
