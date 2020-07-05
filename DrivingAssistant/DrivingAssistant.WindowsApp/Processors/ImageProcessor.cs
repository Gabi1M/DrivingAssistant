using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DrivingAssistant.WindowsApp.Processors
{
    public static class ImageProcessor
    {
        //======================================================//
        private static ICollection<Point> GetOverlayPoints(int width, int height)
        {
            return new List<Point>
            {
                new Point(5, height - 5),
                new Point(width - 5, height - 5),
                new Point(width - 5, height / 2),
                new Point(3 * width / 4, 2 * height / 4),
                new Point(width / 3, 2 * height / 4)
            };
        }

        //======================================================//
        private static Image<Gray, byte> MaskImage(Image<Gray, byte> image, ICollection<Point> regionOfInterestPoints)
        {
            var resultImage = new Image<Gray, byte>(image.Size);
            resultImage.FillConvexPoly(regionOfInterestPoints.ToArray(), new Gray(255));
            return image & resultImage;
        }

        //======================================================//
        private static Point GetCenterPoint(LineSegment2D line)
        {
            return new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
        }

        //======================================================//
        public static Bitmap ProcessImage(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var image = new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            var cannyImage = image.Canny(100, 150);
            var resultImage = MaskImage(cannyImage, GetOverlayPoints(image.Width, image.Height)).Dilate(1);

            var lines = resultImage.HoughLinesBinary(1, Math.PI / 180, 10, 5, 5)[0];
            foreach (var line in lines)
            {
                image.Draw(line, new Bgr(0, 255, 0), 2);
            }

            return image.AsBitmap();
        }
    }
}
