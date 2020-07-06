using System;
using System.Drawing;
using System.IO;
using DrivingAssistant.Core.Tools;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Utils
    {
        //============================================================
        public static Bitmap Base64ToBitmap(byte[] data)
        {
            return Image.FromStream(new MemoryStream(data)) as Bitmap;
        }

        //============================================================
        public static string GetRandomFilename(string format, string type)
        {
            var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            var path = string.Empty;
            if (type == "image")
            {
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.ImageStoragePath, directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.ImageStoragePath, directory));
                }
                path = Path.Combine(Constants.ServerConstants.ImageStoragePath, directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.ImageStoragePath, directory, Path.GetRandomFileName()) + format;
                }
            }
            else if (type == "video")
            {
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.VideoStoragePath, directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.VideoStoragePath, directory));
                }
                path = Path.Combine(Constants.ServerConstants.VideoStoragePath, directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.VideoStoragePath, directory, Path.GetRandomFileName()) + format;
                }
            }

            return path;
        }
    }
}
