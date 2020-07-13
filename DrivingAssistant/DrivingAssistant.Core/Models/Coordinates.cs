using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Coordinates
    {
        [JsonProperty("X")]
        public decimal X { get; set; }

        [JsonProperty("Y")]
        public decimal Y { get; set; }

        //============================================================
        public Coordinates(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        //============================================================
        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
