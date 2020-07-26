using DrivingAssistant.Core.Models.ImageProcessing;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Report : BaseEntity
    {
        [JsonProperty("MediaId")]
        public long MediaId { get; set; }

        [JsonProperty("ProcessedFrames")]
        public long ProcessedFrames { get; set; }

        [JsonProperty("SuccessFrames")]
        public long SuccessFrames { get; set; }

        [JsonProperty("FailFrames")]
        public long FailFrames { get; set; }

        [JsonProperty("SuccessRate")]
        public double SuccessRate { get; set; }

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
        public static Report FromImageReport(ImageReport imageReport, long mediaId)
        {
            return new Report
            {
                Id = -1,
                MediaId = mediaId,
                ProcessedFrames = 1,
                SuccessFrames = imageReport.Success ? 1 : 0,
                FailFrames = imageReport.Success ? 0 : 1,
                SuccessRate = imageReport.Success ? 100 : 0,
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
        public static Report FromVideoReport(VideoReport videoReport, long mediaId)
        {
            return new Report
            {
                Id = -1,
                MediaId = mediaId,
                ProcessedFrames = videoReport.NumberOfFrames,
                SuccessFrames = videoReport.SuccessFrames,
                FailFrames = videoReport.FailFrames,
                SuccessRate = (float) videoReport.SuccessFrames * 100 / videoReport.NumberOfFrames,
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
