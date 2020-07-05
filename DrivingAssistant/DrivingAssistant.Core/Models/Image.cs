using System;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Image : BaseEntity
    {
        [JsonProperty("Filepath")]
        public string Filepath { get; }

        [JsonProperty("Width")]
        public int Width { get; }

        [JsonProperty("Height")]
        public int Height { get; }

        [JsonProperty("Format")]
        public string Format { get; }

        [JsonProperty("Source")]
        public string Source { get; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; }

        //============================================================
        public Image(string filepath, int width, int height, string format, string source, DateTime dateTime, long id = -1)
        {
            Filepath = filepath;
            Width = width;
            Height = height;
            Format = format;
            Source = source;
            DateTime = dateTime;
            Id = id;
        }
    }
}
