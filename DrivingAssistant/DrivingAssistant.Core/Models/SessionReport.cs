using DrivingAssistant.Core.Enums;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class SessionReport : BaseEntity
    {
        [JsonProperty("SessionId")]
        public long SessionId { get; set; }

        [JsonProperty("ReportStatus")]
        public ReportStatus ReportStatus { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Details")]
        public string Details { get; set; }

        [JsonProperty("TotalMediaNumber")]
        public int TotalMediaNumber { get; set; }

        [JsonProperty("ProcessedMediaNumber")]
        public int ProcessedMediaNumber { get; set; }
    }
}
