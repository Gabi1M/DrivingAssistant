using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Coordinates
    {
        [JsonProperty("X")]
        public double X { get; set; }

        [JsonProperty("Y")]
        public double Y { get; set; }

        //============================================================
        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
