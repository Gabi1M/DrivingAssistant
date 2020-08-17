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
    public class LoginActivity : Activity
    {
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private TextView _textServer;
        private Button _loginButton;
        private Button _registerButton;
        private ProgressDialog _progressDialog;

        private LoginActivityPresenter _presenter;
        private HostServer _selectedServer;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SimpleStorage.SetContext(ApplicationContext);
            _presenter = new LoginActivityPresenter(this);
            _presenter.OnPropertyChanged += PresenterOnPropertyChanged;
            SetContentView(Resource.Layout.activity_login);
            SetupActivityFields();

            _selectedServer = HostServer.Default;
            _textServer.Text = _selectedServer.Name;
            Constants.ServerUri = _selectedServer.Address;
        }

        //============================================================
        private void PresenterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_progressDialog.IsShowing)
            {
                _progressDialog.Dismiss();
            }

            if (e.Data is Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long)?.Show();
                return;
            }

            switch (e.Command)
            {
                case NotifyCommand.LoginActivity_Server:
                {
                    _selectedServer = e.Data as HostServer;
                    _textServer.Text = _selectedServer?.Name;
                    Constants.ServerUri = _selectedServer?.Address;
                    break;
                }
                case NotifyCommand.LoginActivity_Register:
                {
                    var intent = new Intent(this, typeof(RegisterActivity));
                    StartActivity(intent);
                    break;
                }
                case NotifyCommand.LoginActivity_Login:
                {
                    var user = e.Data as User;
                    var intent = new Intent(Application.Context, typeof(MainActivity));
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

            if (_presenter == null)
            {
                _presenter = new LoginActivityPresenter(this);
                _presenter.OnPropertyChanged += PresenterOnPropertyChanged;
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
            _presenter.TextServerClicked();
        }

        //============================================================
        private void OnRegisterButtonClick(object sender, EventArgs e)
        {
            _presenter.RegisterButtonClicked();
        }

        //============================================================
        private async void OnLoginButtonClick(object sender, EventArgs e)
        {
            _progressDialog = ProgressDialog.Show(this, "Login", "Logging in ...");
            await _presenter.LoginButtonClick(_textInputUsername.Text, _textInputPassword.Text);
        }
    }
}