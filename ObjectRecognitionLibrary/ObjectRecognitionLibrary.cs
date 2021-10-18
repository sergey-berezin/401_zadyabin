using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static void AnalyseFolder(this string imageFolder)
        {
            if (imageFolder.Length > 0)
            {
                Console.WriteLine("Type ctrl+c to stop all threads");
                float progress = 0;

                string[] imagePaths = Directory
                    .GetFiles(imageFolder);

                int imagesAmount = imagePaths.Length;

                var model = new Model();

                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                CancellationToken token = cancelTokenSource.Token;

                Console.CancelKeyPress += (s, ev) =>
                {
                    cancelTokenSource.Cancel();
                    ev.Cancel = true;
                };

                try
                {
                    Parallel.ForEach(imagePaths, new ParallelOptions { CancellationToken = token }, imagePath =>
                    {
                        var imageName = Path.GetFileName(imagePath);
                        var bitmap = new Bitmap(Image.FromFile(imagePath));

                        var results = model.PredictionResults(bitmap);
                        ObjectRectangles.Draw(bitmap, results, imageName);

                        lock (outputLock)
                        {
                            progress++;
                            Console.WriteLine($"Progress is {progress / imagesAmount * 100} %\n");
                            LogDetectedObjects(imageName, results);
                        }
                    }
                   );
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation was canceled");
                }
                finally
                {
                    cancelTokenSource.Dispose();
                }
            } 
            else
            {
                Console.WriteLine("Folder was not specified");
            }
        }

        private static void LogDetectedObjects(string imageName, IReadOnlyList<YoloV4Result> boundingBoxes)
        {
            Console.WriteLine($".....The objects in the image {imageName} are detected as below....");
            foreach (var box in boundingBoxes)
            {
                Console.WriteLine($"{box.Label} and its Confidence score: {box.Confidence}");
            }
            Console.WriteLine("");
        }

    }
}
