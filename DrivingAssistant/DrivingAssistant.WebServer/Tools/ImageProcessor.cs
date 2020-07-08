using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace DrivingAssistant.WebServer.Tools
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
        private static Image<Gray, byte> MaskImage(Image<Gray, byte> image, ICollection<Point> roi)
        {
            var maskedImage = new Image<Gray, byte>(image.Size);
            maskedImage.FillConvexPoly(roi.ToArray(), new Gray(255));
            return image & maskedImage;
        }

        //======================================================//
        private static Point GetCenterPoint(LineSegment2D line)
        {
            return new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
        }

        //======================================================//
        public static Bitmap ProcessImage(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                bitmap.PixelFormat);
            var originalImage =
                new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            var processedImage = originalImage.Clone();

            var cannyImage = originalImage.Canny(100, 150);
            var maskedImage = MaskImage(cannyImage, GetOverlayPoints(originalImage.Width, originalImage.Height))
                .Dilate(1);

            var houghLines = maskedImage.HoughLinesBinary(1, Math.PI / 180, 10, 5, 5)[0];
            foreach (var line in houghLines)
            {
                processedImage.Draw(line, new Bgr(0, 255, 0), 2);
            }

            bitmap.Dispose();

            return processedImage.ToBitmap();
        }

        //======================================================//
        public static Bitmap ToBitmap(this Image<Bgr, byte> image)
        {
            var size = image.Size;
            var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            var bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            using var mat = new Mat(size.Height, size.Width, DepthType.Cv8U, image.NumberOfChannels, bitmapData.Scan0, bitmapData.Stride);
            image.Mat.CopyTo(mat);
            bitmap.UnlockBits(bitmapData);
            image.Dispose();
            return bitmap;
        }
    }
}
