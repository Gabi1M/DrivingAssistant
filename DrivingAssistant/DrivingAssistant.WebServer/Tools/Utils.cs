using System;
using System.IO;
using System.Threading.Tasks;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Utils
    {
        //============================================================
        public static string GetRandomFilename(string format)
        {
            var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory)))
            {
                Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory));
            }
            var path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
            while (File.Exists(path))
            {
                path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
            }

            return path;
        }

        //============================================================
        public static async Task<string> SaveVideoStreamToFileAsync(Stream videoStream, string encoding)
        {
            var filepath = GetRandomFilename("." + encoding);
            await SaveStreamToFileAsync(videoStream, filepath);
            return filepath;
        }

        //============================================================
        private static async Task SaveStreamToFileAsync(Stream stream, string filepath)
        {
            await using var file = File.Create(filepath);
            await stream.CopyToAsync(file);
            file.Close();
        }
    }
}
