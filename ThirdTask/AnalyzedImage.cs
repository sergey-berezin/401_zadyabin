using System.Collections.Generic;

namespace ThirdTask
{
    public class AnalyzedImage
    {
        public int AnalyzedImageId { get; set; }
        virtual public ICollection<BoundingBox> BoundingBoxes { get; set; }
        public byte[] Image { get; set; }
        public string ImageHash { get; set; }
    }
    public class BoundingBox
    {
        public int BoundingBoxId { get; set; }
        public float x1 { get; set; }
        public float y1 { get; set; }
        public float x2 { get; set; }
        public float y2 { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
        public int AnalyzedImageId { get; set; }
    }
}
