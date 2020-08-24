using DrivingAssistant.Core.Enums;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class RemoteCamera : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("DestinationSessionId")]
        public long DestinationSessionId { get; set; }

        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("VideoLength")]
        public int VideoLength { get; set; }

        [JsonProperty("AutoProcessSession")]
        public bool AutoProcessSession { get; set; }

        [JsonProperty("AutoProcessSessionType")]
        public ProcessingAlgorithmType AutoProcessSessionType { get; set; }

        //===========================================================//
        public static RemoteCamera Default(long userId)
        {
            return new RemoteCamera
            {
                Id = -1,
                UserId = userId,
                DestinationSessionId = -1,
                Host = string.Empty,
                Username = string.Empty,
                Password = string.Empty,
                VideoLength = 10,
                AutoProcessSession = false,
                AutoProcessSessionType = ProcessingAlgorithmType.Lane_Departure_Warning
            };
        }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
