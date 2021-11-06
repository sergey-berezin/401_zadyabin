using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ObjectRecognitionLibrary.DataStructures;

namespace ObjectRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Console.CancelKeyPress += (s, ev) =>
            {
                cancelTokenSource.Cancel();
                ev.Cancel = true;
            };

            if (args.Length > 0)
            {
                Console.WriteLine("Type ctrl+c to stop all threads");
                var results = new BlockingCollection<ImageData>();
                
                Task.Run(() => ObjectRecognitionLibrary.ObjectRecognitionLibrary.AnalyseFolder(args[0], results, token));
                
                ImageData imageData;
                while (!results.IsCompleted)
                {
                    if (results.TryTake(out imageData)) {
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
