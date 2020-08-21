using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;

namespace DrivingAssistant.AndroidApp.Activities.Register
{
    public class RegisterActivityViewPresenter : ViewPresenter
    {
        private readonly UserService _userService = new UserService();

        //============================================================
        public RegisterActivityViewPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public async Task RegisterButtonClick(string firstName, string lastName, string username, string password, string email)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                Notify(new NotificationEventArgs(NotificationCommand.RegisterActivity_Register, new Exception("Some fields were left empty!")));
                return;
            }

            if (!await Utils.CheckConnectionAsync(Constants.ServerUri))
            {
                Notify(new NotificationEventArgs(NotificationCommand.RegisterActivity_Register, new Exception("Failed to connect to server!")));
                return;
            }

            try
            {
                var users = await _userService.GetAllAsync();
                if (users.Any(x => x.Username.Trim() == username.Trim()))
                {
                    Notify(new NotificationEventArgs(NotificationCommand.RegisterActivity_Register, new Exception("There is already an user with the same username!")));
                    return;
                }

                var user = new User
                {
                    Id = -1,
                    Username = username.Trim(),
                    Password = Crypto.EncryptSha256(password.Trim()),
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    Email = email.Trim(),
                    JoinDate = DateTime.Now
                };

                await _userService.SetAsync(user);
                Notify(new NotificationEventArgs(NotificationCommand.RegisterActivity_Register, user));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.RegisterActivity_Register, ex));
            }
        }
    }
}