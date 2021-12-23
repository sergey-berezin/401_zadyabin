using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ObjectRecognitionLibrary.DataStructures;

namespace FirstTask
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            ObjectRecognitionLibrary.ObjectRecognitionLibrary objectRecognitionLibrary = new();

        Console.CancelKeyPress += (s, ev) =>
            {
                cancelTokenSource.Cancel();
                ev.Cancel = true;
            };

            if (args.Length > 0)
            {
                Console.WriteLine("Type ctrl+c to stop all threads");
                var results = new BlockingCollection<ImageData>();

                float progress = 0;
                int imagesAmount = Directory.GetFiles(args[0]).Length;
               
                Task.Factory.StartNew(() => objectRecognitionLibrary.AnalyseFolder(args[0], results, token), 
                    TaskCreationOptions.LongRunning);
                
                ImageData imageData;
                while (!results.IsCompleted)
                {
                    if (results.TryTake(out imageData)) {
                        progress++;
                        Console.WriteLine($"Progress is {progress / imagesAmount * 100} %\n");
                        LogDetectedObjects(Path.GetFileName(imageData.imagePath), imageData.boundingBoxes);
                    }
                }
            }
            else
            {
                Console.WriteLine("Folder was not specified");
            }
            cancelTokenSource.Dispose();
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
