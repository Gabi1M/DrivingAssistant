using System;
using DrivingAssistant.Core.Enums;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Media : BaseEntity
    {
        [JsonProperty("ProcessedId")]
        public long ProcessedId { get; set; }

        [JsonProperty("SessionId")]
        public long SessionId { get; set; }

        [JsonProperty("Type")]
        public MediaType Type { get; set; }

        [JsonProperty("Filepath")]
        public string Filepath { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("DateAdded")]
        public DateTime DateAdded { get; set; }

        //===========================================================//
        [JsonConstructor]
        public Media(MediaType type, string filepath, string source, DateTime dateAdded, long id = default, long processedId = default, long sessionId = default)
        {
            Type = type;
            Filepath = filepath;
            Source = source;
            DateAdded = dateAdded;
            Id = id;
            ProcessedId = processedId;
            SessionId = sessionId;
        }

        //===========================================================//
        public Media(string type, string filepath, string source, DateTime dateAdded, long id = default, long processedId = default, long sessionId = default)
        {
            Filepath = filepath;
            Source = source;
            DateAdded = dateAdded;
            Id = id;
            ProcessedId = processedId;
            SessionId = sessionId;
            if (Enum.TryParse(type, true, out MediaType _type))
            {
                Type = _type;
            }
        }
    }
}
