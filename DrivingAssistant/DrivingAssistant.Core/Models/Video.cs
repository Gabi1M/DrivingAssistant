using System;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Video : BaseEntity
    {
        [JsonProperty("Filepath")]
        public string Filepath { get; }

        [JsonProperty("Width")]
        public int Width { get; }

        [JsonProperty("Height")]
        public int Height { get; }

        [JsonProperty("Fps")]
        public int Fps { get; }

        [JsonProperty("Format")]
        public string Format { get; }

        [JsonProperty("Source")]
        public string Source { get; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; }

        //============================================================
        public Video(string filepath, int width, int height, int fps, string format, string source, DateTime dateTime, long id = -1)
        {
            Filepath = filepath;
            Width = width;
            Height = height;
            Fps = fps;
            Format = format;
            Source = source;
            DateTime = dateTime;
            Id = id;
        }
    }
}
