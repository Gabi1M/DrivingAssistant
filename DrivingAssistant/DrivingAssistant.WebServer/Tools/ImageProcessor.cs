using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DrivingAssistant.Core.Enums;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Bitmap = System.Drawing.Bitmap;

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
        private static Image<Bgr, byte> ProcessCvImage(Image<Bgr, byte> image)
        {
            var processedImage = image.Clone();
            var cannyImage = processedImage.Canny(100, 150);
            var maskedImage = MaskImage(cannyImage, GetOverlayPoints(processedImage.Width, processedImage.Height)).Dilate(1);
            var houghLines = maskedImage.HoughLinesBinary(1, Math.PI / 180, 10, 5, 5)[0];
            foreach (var houghLine in houghLines)
            { 
                processedImage.Draw(houghLine, new Bgr(0, 255, 0), 2);
            }

            image.Dispose();
            return processedImage;
        }

        //======================================================//
        private static Bitmap ProcessBitmap(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var originalImage = new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
            return ProcessCvImage(originalImage).ToBitmap();
        }

        //======================================================//
        private static Bitmap ToBitmap(this Image<Bgr, byte> image)
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

        //======================================================//
        public static string ProcessImage(string filename, bool loadAsBitmap = false)
        {
            if (loadAsBitmap)
            {
                using var bitmap = Image.FromFile(filename) as Bitmap;
                using var processedBitmap = ProcessBitmap(bitmap);
                var processedFilename = Utils.GetRandomFilename(".jpg", MediaType.Image);
                processedBitmap.Save(processedFilename);
                return processedFilename;
            }
            else
            {
                using var image = new Image<Bgr, byte>(filename);
                using var processedImage = ProcessCvImage(image);
                var processedFilename = Utils.GetRandomFilename(".jpg", MediaType.Image);
                processedImage.Save(processedFilename);
                return processedFilename;
            }
        }

        //======================================================//
        public static string ProcessVideo(string filename, int framesToSkip = 0)
        {
            var processedVideoFilename = Utils.GetRandomFilename(".mkv", MediaType.Video);
            using var video = new VideoCapture(filename);
            var videoWriter = new VideoWriter(processedVideoFilename, VideoWriter.Fourcc('H', '2', '6', '4'), 30, new Size(video.Width,video.Height), true);
            var frameCount = 0;
            while (true)
            {
                try
                {
                    ++frameCount;
                    using var capturedImage = video.QueryFrame();
                    if (capturedImage == null)
                    {
                        break;
                    }

                    if (framesToSkip == 0)
                    {
                        using var bgrImage = capturedImage.ToImage<Bgr, byte>();

                        using var processedImage = ProcessCvImage(bgrImage);
                        videoWriter.Write(processedImage.Mat);
                    }
                    else if(frameCount % framesToSkip == 0)
                    {
                        using var bgrImage = capturedImage.ToImage<Bgr, byte>();

                        using var processedImage = ProcessCvImage(bgrImage);
                        videoWriter.Write(processedImage.Mat);
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }

            videoWriter.Dispose();
            return processedVideoFilename;
        }
    }
}
