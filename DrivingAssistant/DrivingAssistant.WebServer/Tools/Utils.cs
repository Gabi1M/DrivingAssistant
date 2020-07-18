using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using Point = DrivingAssistant.Core.Models.Point;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Utils
    {
        //============================================================
        public static string GetRandomFilename(string format, MediaType type)
        {
            var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            switch (type)
            {
                case MediaType.Image:
                {
                    if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory)))
                    {
                        Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory));
                    }
                    var path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                    while (File.Exists(path))
                    {
                        path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                    }

                    return path;
                }
                case MediaType.Video:
                {
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
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Specified file type not supported!");
                }
            }
        }

        //============================================================
        public static async Task<string> SaveImageBase64ToFile(string base64String)
        {
            return await Task.Run(() =>
            {
                var filepath = GetRandomFilename(".jpeg", MediaType.Image);
                var bytes = Convert.FromBase64String(base64String);
                using var image = Image.FromStream(new MemoryStream(bytes));
                image.Save(filepath, ImageFormat.Jpeg);
                return filepath;
            });
        }

        //============================================================
        public static async Task<string> SaveImageBase64ToFile(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            return await SaveImageBase64ToFile(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public static async Task<ICollection<string>> SaveImagesBase64ToFile(string spacedBase64String)
        {
            var filepaths = new List<string>();
            foreach (var base64String in spacedBase64String.Split(' '))
            {
                filepaths.Add(await SaveImageBase64ToFile(base64String));
            }

            return filepaths;
        }

        //============================================================
        public static async Task<ICollection<string>> SaveImagesBase64ToFile(Stream base64Stream)
        {
            using var streamReader = new StreamReader(base64Stream);
            return await SaveImagesBase64ToFile(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public static async Task<string> SaveImageStreamToFileAsync(Stream imageStream)
        {
            var filepath = GetRandomFilename(".jpeg", MediaType.Image);
            await SaveStreamToFileAsync(imageStream, filepath);
            return filepath;
        }

        //============================================================
        public static async Task<string> SaveVideoStreamToFileAsync(Stream videoStream)
        {
            var filepath = GetRandomFilename(".mp4", MediaType.Video);
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
