using System;

namespace ObjectRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ObjectRecognitionLibrary.ObjectRecognitionLibrary.AnalyseFolder(args[0]);
            }
            else
            {
                Console.WriteLine("Folder was not specified");
            }
        }
    }
}
