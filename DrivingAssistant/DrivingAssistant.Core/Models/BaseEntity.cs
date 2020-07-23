using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public abstract class BaseEntity
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        public abstract override string ToString();
    }
}
