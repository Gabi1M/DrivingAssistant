using DrivingAssistant.Core.Models.ImageProcessing;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Report : BaseEntity
    {
        [JsonProperty("MediaId")]
        public long MediaId { get; set; }

        [JsonProperty("SessionId")]
        public long SessionId { get; set; }

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

        //===========================================================//
        public static Report FromImageReport(ImageReport imageReport, long mediaId, long sessionId)
        {
            return new Report
            {
                Id = -1,
                MediaId = mediaId,
                SessionId = sessionId,
                LeftSidePercent = imageReport.LeftSidePercent,
                RightSidePercent = imageReport.RightSidePercent,
                LeftSideLineLength = imageReport.LeftSideLineLength,
                RightSideLineLength = imageReport.RightSideLineLength,
                SpanLineLength = imageReport.SpanLineLength,
                SpanLineAngle = imageReport.SpanLineAngle,
                LeftSideLineNumber = imageReport.LeftSideLineNumber,
                RightSideLineNumber = imageReport.RightSideLineNumber
            };
        }

        //===========================================================//
        public static Report FromVideoReport(VideoReport videoReport, long mediaId, long sessionId)
        {
            return new Report
            {
                Id = -1,
                MediaId = mediaId,
                SessionId = sessionId,
                LeftSidePercent = videoReport.AverageLeftSidePercent,
                RightSidePercent = videoReport.AverageRightSidePercent,
                LeftSideLineLength = videoReport.AverageLeftSideLineLength,
                RightSideLineLength = videoReport.AverageRightSideLineLength,
                SpanLineLength = videoReport.AverageSpanLineLength,
                SpanLineAngle = videoReport.AverageSpanLineAngle,
                LeftSideLineNumber = videoReport.AverageLeftSideLineNumber,
                RightSideLineNumber = videoReport.AverageRightSideLineNumber
            };
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
