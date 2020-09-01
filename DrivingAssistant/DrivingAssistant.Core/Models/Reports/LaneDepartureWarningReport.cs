using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models.Reports
{
    public class LaneDepartureWarningReport : BaseEntity
    {
        [JsonProperty("VideoId")]
        public long VideoId { get; set; }

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

        [JsonProperty("PdfPath")]
        public string PdfPath { get; set; }

        //===========================================================//
        public static LaneDepartureWarningReport FromImageReport(ImageReport imageReport, long videoId)
        {
            return new LaneDepartureWarningReport
            {
                Id = -1,
                VideoId = videoId,
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
        public static LaneDepartureWarningReport FromVideoReport(VideoReport videoReport, long videoId)
        {
            return new LaneDepartureWarningReport
            {
                Id = -1,
                VideoId = videoId,
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
        public string ToPdfString()
        {
            var pdfText = string.Empty;
            pdfText += "Report ID: " + Id + "\n";
            pdfText += "Video ID: " + VideoId + "\n";
            pdfText += "Total frames: " + ProcessedFrames + "\n";
            pdfText += "Successfully processed frames: " + SuccessFrames + "\n";
            pdfText += "Unsuccessfully processed frames: " + FailFrames + "\n";
            pdfText += "Success rate: " + SuccessRate + "\n";
            pdfText += "Left side percent: " + LeftSidePercent + "\n";
            pdfText += "Right side percent: " + RightSidePercent + "\n";
            pdfText += "Average left side line length: " + LeftSideLineLength + "\n";
            pdfText += "Average right side line length: " + RightSideLineLength + "\n";
            pdfText += "Average span line length: " + SpanLineLength + "\n";
            pdfText += "Average span line angle: " + SpanLineAngle + "\n";
            pdfText += "Average left side line number: " + LeftSideLineNumber + "\n";
            pdfText += "Average right side line number: " + RightSideLineNumber;
            return pdfText;
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
