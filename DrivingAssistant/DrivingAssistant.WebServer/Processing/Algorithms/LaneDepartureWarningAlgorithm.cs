using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Tools;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Processing.Algorithms
{
    public class LaneDepartureWarningAlgorithm : IProcessingAlgorithm
    {
        private readonly LaneDepartureWarningParameters _parameters;

        //======================================================//
        public LaneDepartureWarningAlgorithm(LaneDepartureWarningParameters parameters)
        {
            _parameters = parameters;
        }

        //======================================================//
        private static IEnumerable<Point> GetOverlayPoints(int width, int height)
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
        private static Image<Gray, byte> MaskImage(Image<Gray, byte> image, IEnumerable<Point> roi)
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
                var grayImage = processedImage.Convert<Gray, byte>();
                var maskedImage = MaskImage(grayImage, GetOverlayPoints(processedImage.Width, processedImage.Height)).Dilate(_parameters.DilateIterations);
                var cannyImage = maskedImage.Canny(_parameters.CannyThreshold, _parameters.CannyThresholdLinking);
                var lines = cannyImage.HoughLinesBinary(_parameters.HoughLinesRhoResolution,
                    _parameters.HoughLinesThetaResolution, _parameters.HoughLinesThreshold,
                    _parameters.HoughLinesMinimumLineWidth, _parameters.HoughLinesGapBetweenLines)[0].AsEnumerable();

                lines = lines.Select(x => x.OrientLine());
                var middleVerticalLine = new LineSegment2D(new Point(image.Width / 2, 0), new Point(image.Width / 2, image.Height));

                var leftSideLines = lines.Where(x =>
                    middleVerticalLine.Side(x.GetCenterPoint()) == 1 && x.GetAngle(middleVerticalLine) >= 0 &&
                    x.GetAngle(middleVerticalLine) < 80);

                var rightSideLines = lines.Where(x =>
                    middleVerticalLine.Side(x.GetCenterPoint()) == -1 && x.GetAngle(middleVerticalLine) >= 0 &&
                    x.GetAngle(middleVerticalLine) < 80);

                leftSideLines = leftSideLines.OrderBy(x => x.P2.Y).ThenByDescending(x => x.P2.X);
                rightSideLines = rightSideLines.OrderBy(x => x.P2.Y).ThenBy(x => x.P2.X);

                var connectingLine = new LineSegment2D(leftSideLines.Last().GetCenterPoint(), rightSideLines.Last().GetCenterPoint());
                var intersection = connectingLine.GetIntersection(middleVerticalLine);

                var intersectionLeft = new LineSegment2D(leftSideLines.Last().GetCenterPoint(), intersection);
                var intersectionRight = new LineSegment2D(rightSideLines.Last().GetCenterPoint(), intersection);

                var rightSidePercent = (100 * intersectionLeft.Length) / connectingLine.Length;
                var leftSidePercent = (100 * intersectionRight.Length) / connectingLine.Length;

                processedImage.Draw(intersectionLeft, new Bgr(255, 0, 0), 2);
                processedImage.Draw(intersectionRight, new Bgr(0, 0, 255), 2);

                leftSideLines.ToList().ForEach(x => processedImage.Draw(x, new Bgr(0, 255, 0), 2));
                rightSideLines.ToList().ForEach(x => processedImage.Draw(x, new Bgr(0, 255, 0), 2));

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
            var processedVideoFilename = Utils.GetRandomFilename(".mkv", FileType.Video);
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
            return processedVideoFilename;
        }

        //======================================================//
        public class LaneDepartureWarningParameters
        {
            [JsonProperty("CannyThreshold")]
            public double CannyThreshold { get; set; }

            [JsonProperty("CannyThresholdLinking")]
            public double CannyThresholdLinking { get; set; }

            [JsonProperty("HoughLinesRhoResolution")]
            public double HoughLinesRhoResolution { get; set; }

            [JsonProperty("HoughLinesThetaResolution")]
            public double HoughLinesThetaResolution { get; set; }

            [JsonProperty("HoughLinesMinimumLineWidth")]
            public double HoughLinesMinimumLineWidth { get; set; }

            [JsonProperty("HoughLinesGapBetweenLines")]
            public double HoughLinesGapBetweenLines { get; set; }

            [JsonProperty("HoughLinesThreshold")]
            public int HoughLinesThreshold { get; set; }

            [JsonProperty("DilateIterations")]
            public int DilateIterations { get; set; }

            //======================================================//
            public static LaneDepartureWarningParameters Default()
            {
                return new LaneDepartureWarningParameters
                {
                    CannyThreshold = 100,
                    CannyThresholdLinking = 150,
                    HoughLinesRhoResolution = 1,
                    HoughLinesThetaResolution = Math.PI / 100,
                    HoughLinesMinimumLineWidth = 5,
                    HoughLinesGapBetweenLines = 5,
                    HoughLinesThreshold = 10
                };
            }
        }
    }
}
