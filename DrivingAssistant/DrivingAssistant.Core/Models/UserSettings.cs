using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class UserSettings : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("CameraSessionId")]
        public long CameraSessionId { get; set; }

        [JsonProperty("CameraHost")]
        public string CameraHost { get; set; }

        [JsonProperty("CameraUsername")]
        public string CameraUsername { get; set; }

        [JsonProperty("CameraPassword")]
        public string CameraPassword { get; set; }

        //===========================================================//
        public static UserSettings Default(long userId)
        {
            return new UserSettings
            {
                Id = -1,
                UserId = userId,
                CameraSessionId = -1,
                CameraHost = string.Empty,
                CameraUsername = string.Empty,
                CameraPassword = string.Empty
            };
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
