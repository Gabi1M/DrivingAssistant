using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DrivingAssistant.Core.Models;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;

namespace DrivingAssistant.WindowsApp
{
    public class ImageProcessor
    {
        private readonly ImageProcessorParameters _parameters;

        //======================================================//
        public ImageProcessor()
        {
            _parameters = ImageProcessorParameters.Default();
        }

        //======================================================//
        private static ICollection<Point> GetOverlayPoints(int width, int height)
        {
            return new List<Point>
            {
                new Point(5, height - 50),
                new Point(width - 5, height - 50),
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
        private Image<Bgr, byte> ProcessCvImage(Image<Bgr, byte> image)
        {
            var processedImage = image.Clone();
            var cannyImage = processedImage.Canny(_parameters.CannyThreshold, _parameters.CannyThresholdLinking);
            var maskedImage = MaskImage(cannyImage, GetOverlayPoints(processedImage.Width, processedImage.Height)).Dilate(_parameters.DilateIterations);
            var houghLines = maskedImage.HoughLinesBinary(_parameters.HoughLinesRhoResolution,
                _parameters.HoughLinesThetaResolution, _parameters.HoughLinesThreshold,
                _parameters.HoughLinesMinimumLineWidth, _parameters.HoughLinesGapBetweenLines)[0].AsEnumerable();

            houghLines = houghLines.Select(x => x.OrientLine());
            var referenceLine = new LineSegment2D(new Point(image.Width / 2, 5), new Point(image.Width / 2, image.Height - 5));
            processedImage.Draw(referenceLine, new Bgr(255, 0, 0), 2);


            var leftLines = houghLines.Where(x => x.Side(x.GetCenterPoint()) == 1 && x.GetAngle(referenceLine) > 10 && x.GetAngle(referenceLine) < 55).OrderBy(x => x.P1.Y);
            var rightLines = houghLines.Where(x => x.Side(x.GetCenterPoint()) == -1 && x.GetAngle(referenceLine) > 55 && x.GetAngle(referenceLine) < 90).OrderBy(x => x.P1.Y);

            processedImage.Draw(leftLines.Last(), new Bgr(0, 255, 255), 2);
            processedImage.Draw(rightLines.Last(), new Bgr(0, 0, 255), 2);

            var connectingLine = new LineSegment2D(leftLines.Last().GetCenterPoint(), rightLines.Last().GetCenterPoint());
            processedImage.Draw(connectingLine, new Bgr(255,255,255), 2);

            var intersection = connectingLine.GetIntersection(referenceLine);
            Console.WriteLine(intersection);

            /*foreach (var line in leftLines)
            {
                processedImage.Draw(line, new Bgr(0, 255, 0) , 2);
            }

            foreach (var line in rightLines)
            {
                processedImage.Draw(line, new Bgr(0, 0, 255), 2);
            }*/

            image.Dispose();
            return processedImage;
        }

        //======================================================//
        public Bitmap ProcessBitmap(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var originalImage = new Image<Bgr, byte>(bitmapData.Width, bitmapData.Height, bitmapData.Stride, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            //bitmap.Dispose();
            return ProcessCvImage(originalImage).ToBitmap();
        }

        /*//======================================================//
        public string ProcessImage(string filename, bool loadAsBitmap = false)
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
        public string ProcessVideo(string filename, int framesToSkip = 0)
        {
            var processedVideoFilename = Utils.GetRandomFilename(".mkv", MediaType.Video);
            using var video = new VideoCapture(filename);
            var videoWriter = new VideoWriter(processedVideoFilename, VideoWriter.Fourcc('H', '2', '6', '4'), 30, new Size(video.Width, video.Height), true);
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
                    else if (frameCount % framesToSkip == 0)
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
        }*/
    }

    public static class ImageProcessorExtender
    {
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

        //======================================================//
        public static LineSegment2D OrientLine(this LineSegment2D line)
        {
            return line.P1.Y > line.P2.Y ? new LineSegment2D(line.P2, line.P1) : line;
        }

        //======================================================//
        public static Point GetCenterPoint(this LineSegment2D line)
        {
            return new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
        }

        //======================================================//
        public static double GetAngle(this LineSegment2D line, LineSegment2D referenceLine)
        {
            var angle = line.GetExteriorAngleDegree(referenceLine);
            if (angle < 0)
            {
                angle *= -1;
            }

            return angle;
        }

        //======================================================//
        public static PointF GetIntersection(this LineSegment2D line1, LineSegment2D line2)
        {

            var a1 = (line1.P1.Y - line1.P2.Y) / (double)(line1.P1.X - line1.P2.X);
            var b1 = line1.P1.Y - a1 * line1.P1.X;

            var a2 = (line2.P1.Y - line2.P2.Y) / (double)(line2.P1.X - line2.P2.X);
            var b2 = line2.P1.Y - a2 * line2.P1.X;

            if (Math.Abs(a1 - a2) < double.Epsilon)
                throw new InvalidOperationException();

            var x = (b2 - b1) / (a1 - a2);
            var y = a1 * x + b1;
            return new PointF((float)x, (float)y);
        }
    }
}
