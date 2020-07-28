using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class UserSettings : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("CameraSessionId")]
        public long CameraSessionId { get; set; }

        [JsonProperty("CameraIp")]
        public string CameraIp { get; set; }

        //===========================================================//
        public static UserSettings Default(long userId)
        {
            return new UserSettings
            {
                Id = -1,
                UserId = userId,
                CameraSessionId = -1,
                CameraIp = string.Empty
            };
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
