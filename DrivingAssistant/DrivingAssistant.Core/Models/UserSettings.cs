using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class UserSettings : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("ImageProcessorParameters")]
        public ImageProcessorParameters ImageProcessorParameters { get; set; }
    }
}
