using System.Collections.Generic;

namespace ThirdTask.Database
{
    public class AnalyzedImage
    {
        public int AnalyzedImageId { get; set; }
        virtual public ICollection<BoundingBox> BoundingBoxes { get; set; }
        public byte[] Image { get; set; }
        public string ImageHash { get; set; }
    }
}
