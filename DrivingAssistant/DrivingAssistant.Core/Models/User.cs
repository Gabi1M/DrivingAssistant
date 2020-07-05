using System;
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

        [JsonProperty("JoinDate")]
        public DateTime JoinDate { get; set; }

        //============================================================
        public User(string username, string password, string firstName, string lastName, DateTime joinDate, long id = -1)
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            JoinDate = joinDate;
            Id = id;
        }
    }
}
