using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using ObjectRecognitionLibrary.DataStructures;

namespace ObjectRecognitionLibrary
{
    public class ObjectRectangles
    {
        public static void Draw(Bitmap bitmap, IReadOnlyList<YoloV4Result> results, string imageName)
        {
            var g = Graphics.FromImage(bitmap);

            foreach (var res in results)
            {
                var x1 = res.BBox[0];
                var y1 = res.BBox[1];
                var x2 = res.BBox[2];
                var y2 = res.BBox[3];
                g.DrawRectangle(Pens.Red, x1, y1, x2 - x1, y2 - y1);
                g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Red)), x1, y1, x2 - x1, y2 - y1);
                g.DrawString(res.Label + " " + res.Confidence.ToString("0.00"),
                             new Font("Arial", 12), Brushes.Blue, new PointF(x1, y1));
            }

            var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var imageOutputFolder = buildDir + @"\Output";
            if (!Directory.Exists(imageOutputFolder))
            {
                Directory.CreateDirectory(imageOutputFolder);
            }
            var filename = Path.Combine(imageOutputFolder, Path.ChangeExtension(imageName, "_processed" + Path.GetExtension(imageName)));
            bitmap.Save(filename);
        }
    }
}
