using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Thumbnail : BaseEntity
    {
        [JsonProperty("VideoId")]
        public long VideoId { get; set; }

        [JsonProperty("Filepath")]
        public string Filepath { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
