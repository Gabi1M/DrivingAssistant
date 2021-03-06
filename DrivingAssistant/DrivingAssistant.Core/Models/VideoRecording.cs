﻿using System;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class VideoRecording : BaseEntity
    {
        [JsonProperty("ProcessedId")]
        public long ProcessedId { get; set; }

        [JsonProperty("SessionId")]
        public long SessionId { get; set; }

        [JsonProperty("Filepath")]
        public string Filepath { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("DateAdded")]
        public DateTime DateAdded { get; set; }

        //===========================================================//
        public bool IsProcessed()
        {
            return ProcessedId != -1;
        }

        //===========================================================//
        public bool IsInSession()
        {
            return SessionId != -1;
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
