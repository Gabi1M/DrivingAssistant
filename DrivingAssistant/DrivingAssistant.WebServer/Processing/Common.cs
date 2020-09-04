using System;
using System.Drawing;
using System.Drawing.Imaging;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Tools;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace DrivingAssistant.WebServer.Processing
{
    public static class Common
    {
        //======================================================//
        /// <summary>
        /// Converts an OpenCV image to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="image">The OpenCV image to be converted.</param>
        /// <returns>The converted image.</returns>
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
        /// <summary>
        /// Returns the center point of a given line segment.
        /// </summary>
        /// <param name="line">The line whose center is required.</param>
        /// <returns>The center point of the line.</returns>
        public static Point GetCenterPoint(this LineSegment2D line)
        {
            return new Point((line.P1.X + line.P2.X) / 2, (line.P1.Y + line.P2.Y) / 2);
        }

        //======================================================//
        /// <summary>
        /// Orients the given line if with respect to the top of the image.
        /// </summary>
        /// <param name="line">The line that needs to be reoriented.</param>
        /// <returns>The oriented line.</returns>
        public static LineSegment2D OrientLine(this LineSegment2D line)
        {
            return line.P1.Y > line.P2.Y ? new LineSegment2D(line.P2, line.P1) : line;
        }

        //======================================================//
        /// <summary>
        /// Gets the angle of the line with respect to a given reference line.
        /// </summary>
        /// <param name="line">The current line.</param>
        /// <param name="referenceLine">The given reference line.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the intersection point of 2 lines.
        /// </summary>
        /// <param name="line1">The first line.</param>
        /// <param name="line2">The second line.</param>
        /// <returns>The intersection point of the 2 lines.</returns>
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

            var x = (b2 * c1 - b1 * c2) / determinant;
            var y = (a1 * c2 - a2 * c1) / determinant;
            return new Point(x, y);
        }

        //======================================================//
        public static bool ApproximateLine(this LineSegment2D line, LineSegment2D compareLine, int threshold)
        {
            return line.P1.X > compareLine.P1.X - threshold && line.P1.X < compareLine.P1.X + threshold &&
                   line.P2.X > compareLine.P2.X - threshold && line.P2.X < compareLine.P2.X + threshold
                   && line.P1.Y > compareLine.P1.Y - threshold && line.P1.Y < compareLine.P1.Y + threshold &&
                   line.P2.Y > compareLine.P2.Y - threshold && line.P2.Y < compareLine.P2.Y + threshold;
        }

        //======================================================//
        /// <summary>
        /// Converts the file to Matroska mkv format.
        /// </summary>
        /// <param name="filename">The video file's filename.</param>
        /// <returns>The filename for the newly converted file.</returns>
        public static string ConvertH264ToMkv(string filename)
        {
            var newFilename = Utils.GetRandomFilename(".mkv", FileType.Video);
            using var video = new VideoCapture(filename);
            using var videoWriter = new VideoWriter(newFilename, VideoWriter.Fourcc('H', '2', '6', '4'), 30, new Size(video.Width, video.Height), true);
            while (true)
            {
                try
                {
                    using var frame = video.QueryFrame();
                    if (frame == null)
                    {
                        break;
                    }
                    videoWriter.Write(frame);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            return newFilename;
        }

        //======================================================//
        /// <summary>
        /// Extracts the first frame from the video as a thumbnail.
        /// </summary>
        /// <param name="filename">The video file's filename.</param>
        /// <returns>The filename for the thumbnail.</returns>
        public static string ExtractThumbnail(string filename)
        {
            using var video = new VideoCapture(filename);
            using var capturedImage = video.QueryFrame();
            var savedFilename = Utils.GetRandomFilename(".jpg", FileType.Thumbnail);
            capturedImage.Save(savedFilename);
            return savedFilename;
        }
    }
}
