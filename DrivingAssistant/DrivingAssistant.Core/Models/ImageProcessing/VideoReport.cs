using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models.ImageProcessing
{
    public class VideoReport
    {
        [JsonProperty("AverageLeftSidePercent")]
        public double AverageLeftSidePercent { get; set; }

        [JsonProperty("AverageRightSidePercent")]
        public double AverageRightSidePercent { get; set; }

        [JsonProperty("AverageLeftSideLineLength")]
        public double AverageLeftSideLineLength { get; set; }

        [JsonProperty("AverageRightSideLineLength")]
        public double AverageRightSideLineLength { get; set; }

        [JsonProperty("AverageSpanLineAngle")]
        public double AverageSpanLineAngle { get; set; }

        [JsonProperty("AverageSpanLineLength")]
        public double AverageSpanLineLength { get; set; }

        [JsonProperty("AverageLeftSideLineNumber")]
        public int AverageLeftSideLineNumber { get; set; }

        [JsonProperty("AverageRightSideLineNumber")]
        public int AverageRightSideLineNumber { get; set; }

        //======================================================//
        public static VideoReport FromImageResultList(IEnumerable<ImageReport> imageResults)
        {
            return new VideoReport
            {
                AverageLeftSidePercent = imageResults.Average(x => x.LeftSidePercent),
                AverageRightSidePercent = imageResults.Average(x => x.RightSidePercent),
                AverageLeftSideLineLength = imageResults.Average(x => x.LeftSideLineLength),
                AverageRightSideLineLength = imageResults.Average(x => x.RightSideLineLength),
                AverageSpanLineAngle = imageResults.Average(x => x.SpanLineAngle),
                AverageSpanLineLength = imageResults.Average(x => x.SpanLineLength),
                AverageLeftSideLineNumber = Convert.ToInt32(imageResults.Average(x => x.LeftSideLineNumber)),
                AverageRightSideLineNumber = Convert.ToInt32(imageResults.Average(x => x.RightSideLineNumber))
            };
        }

        //======================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
