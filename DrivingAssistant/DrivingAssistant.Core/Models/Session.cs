using System;
using System.Collections.Generic;
using DrivingAssistant.Core.Enums;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Session : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [JsonProperty("EndDateTime")]
        public DateTime EndDateTime { get; set; }

        [JsonProperty("StartLocation")]
        public Point StartLocation { get; set; }

        [JsonProperty("EndLocation")]
        public Point EndLocation { get; set; }

        [JsonProperty("Waypoints")]
        public ICollection<Point> Waypoints { get; set; }

        [JsonProperty("Status")]
        public SessionStatus Status { get; set; }

        [JsonProperty("DateAdded")]
        public DateTime DateAdded { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
