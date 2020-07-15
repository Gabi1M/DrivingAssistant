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

        [JsonProperty("Role")]
        public UserRole Role { get; set; }

        [JsonProperty("JoinDate")]
        public DateTime JoinDate { get; set; }

        //============================================================
        [JsonConstructor]
        public User(string username, string password, string firstName,
            string lastName, UserRole role, DateTime joinDate, long id = -1)
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            JoinDate = joinDate;
            Id = id;
        }

        //============================================================
        public User(string username, string password, string firstName,
            string lastName, string role, DateTime joinDate, long id = -1)
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            JoinDate = joinDate;
            Id = id;
            try
            {
                Role = (UserRole) Enum.Parse(typeof(UserRole), role, true);
            }
            catch (Exception)
            {
                throw new Exception("Role " + role + " not valid!");
            }
        }
    }
}
