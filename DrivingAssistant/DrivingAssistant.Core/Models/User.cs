using System;
using DrivingAssistant.Core.Enums;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class User : BaseEntity
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Role")]
        public UserRole Role { get; set; }

        [JsonProperty("JoinDate")]
        public DateTime JoinDate { get; set; }

        //===========================================================//
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
