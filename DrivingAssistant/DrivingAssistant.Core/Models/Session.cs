using System;
using System.Collections.Generic;
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

        [JsonProperty("StartPoint")]
        public Point StartPoint { get; set; }

        [JsonProperty("EndPoint")]
        public Point EndPoint { get; set; }

        [JsonProperty("IntermediatePoints")]
        public ICollection<Point> IntermediatePoints { get; set; }

        [JsonProperty("Processed")]
        public bool Processed { get; set; }

        [JsonProperty("DateAdded")]
        public DateTime DateAdded { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
