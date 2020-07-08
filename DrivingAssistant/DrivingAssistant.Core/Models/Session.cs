using System;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Session : BaseEntity
    {
        [JsonProperty("StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [JsonProperty("EndDateTime")]
        public DateTime EndDateTime { get; set; }

        [JsonProperty("StartCoordinates")]
        public Coordinates StartCoordinates { get; set; }

        [JsonProperty("EndCoordinates")]
        public Coordinates EndCoordinates { get; set; }

        //============================================================
        public Session(DateTime startDateTime, DateTime endDateTime, Coordinates startCoordinates, Coordinates endCoordinates, long id = -1)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            StartCoordinates = startCoordinates;
            EndCoordinates = endCoordinates;
            Id = id;
        }
    }
}
