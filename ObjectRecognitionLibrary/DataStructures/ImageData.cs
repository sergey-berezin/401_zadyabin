using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectRecognitionLibrary.DataStructures
{
    public class ImageData
    {
        public string imagePath;
        public IReadOnlyList<YoloV4Result> boundingBoxes;
        public ImageData(string imagePath, IReadOnlyList<YoloV4Result> boundingBoxes)
        {
            this.imagePath = imagePath;
            this.boundingBoxes = boundingBoxes;
        }
    }
}
