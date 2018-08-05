using System.Collections.Generic;
using System.Drawing;

namespace OsrsColorBot
{
    public class OsrsImage
    {
        public Bitmap ImageBitmap { get; set; }
        public string ImageName { get; set; }
        public Point CenterOfImage { get; set; }

        public OsrsImage(Bitmap imageBitmap, string imageName)
        {
            ImageBitmap = imageBitmap;
            ImageName = imageName;
            CenterOfImage = new Point(imageBitmap.Size.Width / 2, imageBitmap.Size.Height / 2);
        }
    }

    public class OsrsColor
    {
        public string ImageName { get; set; }
        public string ColorName { get; set; }
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    public class OsrsScanData
    {
        public OsrsImage ImageData { get; set; }
        public List<Point> MatchLocations { get; set; }

        public OsrsScanData()
        {
            MatchLocations = new List<Point>();
        }
    }

    public class ScanBoundaries
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
    }
}
