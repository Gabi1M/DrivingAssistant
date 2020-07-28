using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class UserSettings : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("CameraSessionId")]
        public long CameraSessionId { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
