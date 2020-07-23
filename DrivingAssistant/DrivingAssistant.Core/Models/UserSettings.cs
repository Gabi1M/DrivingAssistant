using DrivingAssistant.Core.Models.ImageProcessing;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class UserSettings : BaseEntity
    {
        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("Parameters")]
        public Parameters Parameters { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
