using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ObjectRecognitionLibrary.DataStructures;

namespace ObjectRecognitionLibrary
{
    public class ObjectRecognitionLibrary
    {
        private static readonly object outputLock = new object();
        private Model model = new();

        public void AnalyseFolder(string imageFolder, BlockingCollection<ImageData> output, 
            CancellationToken cancellationToken)
        {
            if (imageFolder.Length > 0)
            {
                string[] imagePaths = Directory
                    .GetFiles(imageFolder);

                int imagesAmount = imagePaths.Length;
                
                try
                {
                    Parallel.ForEach(imagePaths, new ParallelOptions { CancellationToken = cancellationToken }, 
                        imagePath =>
                    {
                        var bitmap = new Bitmap(Image.FromFile(imagePath));
                        var results = model.PredictionResults(bitmap);
                       
                        lock (outputLock)
                        {
                            output.Add(new ImageData(imagePath, results));
                        }
                    }
                   );
                }
                catch (OperationCanceledException)
                {
                    output.CompleteAdding();
                    Console.WriteLine("Operation was canceled");
                }
            } 
            else
            {
                Console.WriteLine("Folder was not specified");
            }
            output.CompleteAdding();
        }
    }
}
