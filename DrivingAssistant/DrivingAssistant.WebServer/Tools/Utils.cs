using System;
using System.Drawing;
using System.IO;

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
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory));
                }
                path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                }
            }
            else if (type == "video")
            {
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory));
                }
                path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
                }
            }

            return path;
        }
    }
}
