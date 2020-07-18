using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Session : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

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

        //============================================================
        public Session(string description, DateTime startDateTime, DateTime endDateTime, Point startPoint,
            Point endPoint, ICollection<Point> intermediatePoints, long id = default, long userid = default)
        {
            Description = description;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            StartPoint = startPoint;
            EndPoint = endPoint;
            IntermediatePoints = intermediatePoints;
            Id = id;
            UserId = userid;
        }
    }
}
