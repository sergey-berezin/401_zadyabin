using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ObjectRecognitionLibrary.DataStructures;

namespace ObjectRecognitionLibrary
{
    public static class ObjectRecognitionLibrary
    {
        private static readonly object outputLock = new object();
        public static void AnalyseFolder(string imageFolder, BlockingCollection<ImageData> output, 
            CancellationToken cancellationToken)
        {
            if (imageFolder.Length > 0)
            {
                //float progress = 0;

                string[] imagePaths = Directory
                    .GetFiles(imageFolder);

                int imagesAmount = imagePaths.Length;

                var model = new Model();

                try
                {
                    Parallel.ForEach(imagePaths, new ParallelOptions { CancellationToken = cancellationToken }, 
                        imagePath =>
                    {
                        var imageName = Path.GetFileName(imagePath);
                        var bitmap = new Bitmap(Image.FromFile(imagePath));

                        var results = model.PredictionResults(bitmap);
                        ObjectRectangles.Draw(bitmap, results, imageName);

                        lock (outputLock)
                        {
                            //progress++;
                            //Console.WriteLine($"Progress is {progress / imagesAmount * 100} %\n");
                            output.Add(new ImageData(imagePath, results));
                            //LogDetectedObjects(imageName, results, output);
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
