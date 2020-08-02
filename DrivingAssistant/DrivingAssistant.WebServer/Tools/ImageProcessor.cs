using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DrivingAssistant.Core.Models.ImageProcessing;
using DrivingAssistant.Core.Tools;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Bitmap = System.Drawing.Bitmap;
using Point = System.Drawing.Point;

namespace DrivingAssistant.WebServer.Tools
{
    public class ImageProcessor
    {
        private readonly Parameters _parameters;

        //======================================================//
        public ImageProcessor(Parameters parameters)
        {
            _parameters = parameters;
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
        private Image<Bgr, byte> ProcessCvImage(Image<Bgr, byte> image, out ImageReport report)
        {
            var processedImage = image.Clone();

            try
            {
                var cannyImage = processedImage.Canny(_parameters.CannyThreshold, _parameters.CannyThresholdLinking);
                var maskedImage = MaskImage(cannyImage, GetOverlayPoints(processedImage.Width, processedImage.Height)).Dilate(_parameters.DilateIterations);
                var lines = maskedImage.HoughLinesBinary(_parameters.HoughLinesRhoResolution,
                    _parameters.HoughLinesThetaResolution, _parameters.HoughLinesThreshold,
                    _parameters.HoughLinesMinimumLineWidth, _parameters.HoughLinesGapBetweenLines)[0].AsEnumerable();

                lines = lines.Select(x => x.OrientLine());
                var middleVerticalLine = new LineSegment2D(new Point(image.Width / 2, 0), new Point(image.Width / 2, image.Height));

                var leftSideLines = lines.Where(x =>
                    middleVerticalLine.Side(x.GetCenterPoint()) == 1 && x.GetAngle(middleVerticalLine) > 10 &&
                    x.GetAngle(middleVerticalLine) < 55).OrderBy(x => x.P1.Y);

                var rightSideLines = lines.Where(x =>
                    middleVerticalLine.Side(x.GetCenterPoint()) == -1 && x.GetAngle(middleVerticalLine) > 55 &&
                    x.GetAngle(middleVerticalLine) < 90).OrderBy(x => x.P1.Y);

                var connectingLine = new LineSegment2D(leftSideLines.Last().GetCenterPoint(), rightSideLines.Last().GetCenterPoint());
                var intersection = connectingLine.GetIntersection(middleVerticalLine);

                var intersectionLeft = new LineSegment2D(leftSideLines.Last().GetCenterPoint(), intersection);
                var intersectionRight = new LineSegment2D(rightSideLines.Last().GetCenterPoint(), intersection);


                var rightSidePercent = (100 * intersectionLeft.Length) / connectingLine.Length;
                var leftSidePercent = (100 * intersectionRight.Length) / connectingLine.Length;

                processedImage.Draw(intersectionLeft, new Bgr(255, 0, 0), 2);
                processedImage.Draw(intersectionRight, new Bgr(0, 0, 255), 2);

                lines.AsParallel().ForAll(x => processedImage.Draw(x, new Bgr(0, 255, 0), 2));

                report = new ImageReport
                {
                    Success = true,
                    LeftSidePercent = leftSidePercent,
                    LeftSideLineLength = intersectionLeft.Length,
                    RightSidePercent = rightSidePercent,
                    RightSideLineLength = intersectionRight.Length,
                    SpanLineAngle = connectingLine.GetAngle(middleVerticalLine),
                    SpanLineLength = connectingLine.Length,
                    LeftSideLineNumber = leftSideLines.Count(),
                    RightSideLineNumber = rightSideLines.Count()
                };

            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Warning);
                report = new ImageReport
                {
                    Success = false,
                    LeftSidePercent = default,
                    LeftSideLineLength = default,
                    RightSidePercent = default,
                    RightSideLineLength = default,
                    SpanLineAngle = default,
                    SpanLineLength = default,
                    LeftSideLineNumber = default,
                    RightSideLineNumber = default
                };
            }

            image.Dispose();
            return processedImage;
        }

        //======================================================//
        public string ProcessVideo(string filename, int framesToSkip, out VideoReport report)
        {
            Console.WriteLine("Processing " + filename);
            var processedVideoFilename = Utils.GetRandomFilename(".mkv");
            using var video = new VideoCapture(filename);
            using var videoWriter = new VideoWriter(processedVideoFilename, VideoWriter.Fourcc('H', '2', '6', '4'), 30, new Size(video.Width, video.Height), true);
            var imageResultList = new List<ImageReport>();
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
                        using var processedImage = ProcessCvImage(bgrImage, out var imageResult);
                        imageResultList.Add(imageResult);
                        videoWriter.Write(processedImage.Mat);
                    }
                    else if (frameCount % framesToSkip == 0)
                    {
                        using var bgrImage = capturedImage.ToImage<Bgr, byte>();
                        using var processedImage = ProcessCvImage(bgrImage, out var imageResult);
                        imageResultList.Add(imageResult);
                        videoWriter.Write(processedImage.Mat);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, LogType.Warning);
                    break;
                }
            }

            report = VideoReport.FromImageResultList(imageResultList);
            Console.WriteLine("Done");
            return processedVideoFilename;
        }

        //======================================================//
        public static string ExtractThumbnail(string filename)
        {
            using var video = new VideoCapture(filename);
            using var capturedImage = video.QueryFrame();
            var savedFilename = Utils.GetRandomFilename(".jpg");
            capturedImage.Save(savedFilename);
            return savedFilename;
        }
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
        public static Point GetCenterPoint(this LineSegment2D line)
        {
            return new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
        }

        //======================================================//
        public static LineSegment2D OrientLine(this LineSegment2D line)
        {
            return line.P1.Y > line.P2.Y ? new LineSegment2D(line.P2, line.P1) : line;
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
        public static Point GetIntersection(this LineSegment2D line1, LineSegment2D line2)
        {
            var a1 = line1.P2.Y - line1.P1.Y;
            var b1 = line1.P1.X - line1.P2.X;
            var c1 = a1 * (line1.P1.X) + b1 * (line1.P1.Y);

            var a2 = line2.P2.Y - line2.P1.Y;
            var b2 = line2.P1.X - line2.P2.X;
            var c2 = a2 * (line2.P1.X) + b2 * (line2.P1.Y);

            var determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                return new Point(int.MaxValue, int.MaxValue);
            }
            else
            {
                var x = (b2 * c1 - b1 * c2) / determinant;
                var y = (a1 * c2 - a2 * c1) / determinant;
                return new Point(x, y);
            }
        }
    }
}
