﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecondTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.DialogResult observedDirectory;
        String[] images;
        public MainWindow()
        {
            InitializeComponent();
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
                     
                }
            }
        }

        private void ListOfImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
