using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ThirdTask.Database
{
    public class ImageDatabase
    {
        private ImageContext db = new();

        public void LoadImages(ICollection<BitmapSource> destination)
        {
            db.AnalyzedImages.ForEachAsync((analyzedImage) => destination.Add(Helpers.ByteArrayToBitmapSource(analyzedImage.Image)));
        }
        public void SaveImage(BitmapSource image, Collection<BoundingBox> boundingBoxes)
        {
            byte[] byteArray = Helpers.BitmapSourceToByteArray(image);
            string hash = Helpers.GetHashSHA1(byteArray);

            if (db.AnalyzedImages.Where(a => a.ImageHash == hash).Where(a => a.Image.SequenceEqual(byteArray)).Count() == 0)
            {
                db.AnalyzedImages.Add(new AnalyzedImage { Image = byteArray, BoundingBoxes = boundingBoxes, ImageHash = hash });
                db.SaveChanges();
            }
        }

        public void Clear()
        {
            db.AnalyzedImages.RemoveRange(db.AnalyzedImages.Include(e => e.BoundingBoxes));
            db.SaveChanges();
        }
    }
}
