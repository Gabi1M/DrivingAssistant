using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models.ImageProcessing
{
    public class VideoReport
    {
        [JsonProperty("SuccessFrames")]
        public long SuccessFrames { get; set; }

        [JsonProperty("FailFrames")]
        public long FailFrames { get; set; }

        [JsonProperty("NumberOfFrames")]
        public long NumberOfFrames { get; set; }

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
            var successfulImageResults = imageResults.Where(x => x.Success);

            return new VideoReport
            {
                NumberOfFrames = imageResults.Count(),
                SuccessFrames = successfulImageResults.Count(),
                FailFrames = imageResults.Count() - successfulImageResults.Count(),
                AverageLeftSidePercent = successfulImageResults.Average(x => x.LeftSidePercent),
                AverageRightSidePercent = successfulImageResults.Average(x => x.RightSidePercent),
                AverageLeftSideLineLength = successfulImageResults.Average(x => x.LeftSideLineLength),
                AverageRightSideLineLength = successfulImageResults.Average(x => x.RightSideLineLength),
                AverageSpanLineAngle = successfulImageResults.Average(x => x.SpanLineAngle),
                AverageSpanLineLength = successfulImageResults.Average(x => x.SpanLineLength),
                AverageLeftSideLineNumber = Convert.ToInt32(successfulImageResults.Average(x => x.LeftSideLineNumber)),
                AverageRightSideLineNumber = Convert.ToInt32(successfulImageResults.Average(x => x.RightSideLineNumber))
            };
        }

        //======================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
