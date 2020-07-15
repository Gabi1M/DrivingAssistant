using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Coordinates
    {
        [JsonProperty("Latitude")]
        public float Latitude { get; set; }

        [JsonProperty("Longitude")]
        public float Longitude { get; set; }

        //============================================================
        public Coordinates(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        //============================================================
        public override string ToString()
        {
            return Latitude + ", " + Longitude;
        }
    }
}
