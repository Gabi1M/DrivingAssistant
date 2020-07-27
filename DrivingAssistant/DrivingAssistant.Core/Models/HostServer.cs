using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class HostServer
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        public static HostServer Default = new HostServer
        {
            Name = "Default",
            Address = "http://192.168.100.234:3287"
        };
    }
}
