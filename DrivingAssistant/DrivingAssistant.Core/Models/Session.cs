using System;
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

        [JsonProperty("StartCoordinates")]
        public Coordinates StartCoordinates { get; set; }

        [JsonProperty("EndCoordinates")]
        public Coordinates EndCoordinates { get; set; }

        //============================================================
        public Session(string description, DateTime startDateTime, DateTime endDateTime, Coordinates startCoordinates,
            Coordinates endCoordinates, long id = default, long userid = default)
        {
            Description = description;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            StartCoordinates = startCoordinates;
            EndCoordinates = endCoordinates;
            Id = id;
            UserId = userid;
        }
    }
}
