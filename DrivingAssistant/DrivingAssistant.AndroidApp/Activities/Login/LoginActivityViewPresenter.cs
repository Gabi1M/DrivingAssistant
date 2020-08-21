using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Tools;

namespace DrivingAssistant.AndroidApp.Activities.Login
{
    public class LoginActivityViewPresenter : ViewPresenter
    {
        private readonly UserService _userService = new UserService();
        private readonly ServerService _serverService = new ServerService();

        //============================================================
        public LoginActivityViewPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public void TextServerClicked()
        {
            try
            {
                var servers = _serverService.GetAll();
                var alert = new AlertDialog.Builder(_context);
                alert.SetTitle("Choose a server");
                alert.SetItems(servers.Select(x => x.Name).ToArray(), (o, args) =>
                {
                    Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Server, servers.ElementAt(args.Which)));
                });

                alert.Create()?.Show();
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Server, ex));
            }
        }

        //============================================================
        public void RegisterButtonClicked()
        {
            Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Register, null));
        }

        //============================================================
        public async Task LoginButtonClick(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Login, new Exception("Username or password is empty!")));
                return;
            }

            if (!await Utils.CheckConnectionAsync(Constants.ServerUri))
            {
                Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Login, new Exception("Failed to connect to server!")));
                return;
            }

            try
            {
                var users = await _userService.GetAllAsync();
                if (users.Any(x =>
                    x.Username.Trim() == username && x.Password.Trim() == Crypto.EncryptSha256(password)))
                {
                    var user = users.First(x => x.Username.Trim() == username && x.Password.Trim() == Crypto.EncryptSha256(password));
                    Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Login, user));
                }
                else
                {
                    Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Login, new Exception("Username or password incorrect!")));
                }
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.LoginActivity_Login, ex));
            }
        }
    }
}