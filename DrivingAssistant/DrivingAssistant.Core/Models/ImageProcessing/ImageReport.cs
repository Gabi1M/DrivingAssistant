using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models.ImageProcessing
{
    public class ImageReport
    {
        [JsonProperty("LeftSidePercent")]
        public double LeftSidePercent { get; set; }

        [JsonProperty("RightSidePercent")]
        public double RightSidePercent { get; set; }

        [JsonProperty("LeftSideLineLength")]
        public double LeftSideLineLength { get; set; }

        [JsonProperty("RightSideLineLength")]
        public double RightSideLineLength { get; set; }

        [JsonProperty("SpanLineAngle")]
        public double SpanLineAngle { get; set; }

        [JsonProperty("SpanLineLength")]
        public double SpanLineLength { get; set; }

        [JsonProperty("LeftSideLineNumber")]
        public int LeftSideLineNumber { get; set; }

        [JsonProperty("RightSideLineNumber")]
        public int RightSideLineNumber { get; set; }

        //======================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
