using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities.Main;
using DrivingAssistant.AndroidApp.Activities.Register;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using PerpetualEngine.Storage;

namespace DrivingAssistant.AndroidApp.Activities.Login
{
    [Activity(Label = "Driving Assistant", MainLauncher = true)]
    public class LoginActivityView : Activity
    {
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private TextView _textServer;
        private Button _loginButton;
        private Button _registerButton;

        private LoginActivityViewPresenter _viewPresenter;
        private HostServer _selectedServer;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SimpleStorage.SetContext(ApplicationContext);
            SetContentView(Resource.Layout.activity_login);
            SetupActivityFields();

            _viewPresenter = new LoginActivityViewPresenter(this);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;

            _selectedServer = HostServer.Default;
            _textServer.Text = _selectedServer.Name;
            Constants.ServerUri = _selectedServer.Address;
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(this, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.LoginActivity_Server:
                {
                    _selectedServer = e.Data as HostServer;
                    _textServer.Text = _selectedServer?.Name;
                    Constants.ServerUri = _selectedServer?.Address;
                    break;
                }
                case NotificationCommand.LoginActivity_Register:
                {
                    var intent = new Intent(this, typeof(RegisterActivityView));
                    StartActivity(intent);
                    break;
                }
                case NotificationCommand.LoginActivity_Login:
                {
                    var user = e.Data as User;
                    var intent = new Intent(Application.Context, typeof(MainActivityView));
                    intent.PutExtra("user", JsonConvert.SerializeObject(user));
                    StartActivity(intent);
                    break;
                }
            }
        }

        //============================================================
        protected override void OnResume()
        {
            if (_textInputUsername != null && _textInputPassword != null)
            {
                _textInputUsername.Text = string.Empty;
                _textInputPassword.Text = string.Empty;
            }

            if (_viewPresenter == null)
            {
                _viewPresenter = new LoginActivityViewPresenter(this);
                _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
            }
            base.OnResume();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _textInputUsername = FindViewById<TextInputEditText>(Resource.Id.loginInputUsername);
            _textInputPassword = FindViewById<TextInputEditText>(Resource.Id.loginInputPassword);
            _textServer = FindViewById<TextView>(Resource.Id.loginServer);
            _loginButton = FindViewById<Button>(Resource.Id.loginButton);
            _registerButton = FindViewById<Button>(Resource.Id.loginRegisterButton);

            _textServer.Click += OnTextServerClick;
            _loginButton.Click += OnLoginButtonClick;
            _registerButton.Click += OnRegisterButtonClick;
        }

        //============================================================
        private void OnTextServerClick(object sender, EventArgs e)
        {
            _viewPresenter.TextServerClicked();
        }

        //============================================================
        private void OnRegisterButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.RegisterButtonClicked();
        }

        //============================================================
        private async void OnLoginButtonClick(object sender, EventArgs e)
        {
            var progressDialog = Utils.ShowProgressDialog(this, "Login", "Logging in...");
            await _viewPresenter.LoginButtonClick(_textInputUsername.Text, _textInputPassword.Text);
            progressDialog.Dismiss();
        }
    }
}