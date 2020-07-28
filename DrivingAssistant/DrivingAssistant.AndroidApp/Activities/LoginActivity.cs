﻿using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views.InputMethods;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;
using PerpetualEngine.Storage;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "Driving Assistant", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private TextView _textServer;
        private Button _loginButton;
        private Button _registerButton;

        private readonly UserService _userService = new UserService();
        private readonly ServerService _serverService = new ServerService();
        private HostServer _selectedServer;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SimpleStorage.SetContext(ApplicationContext);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_login);
            SetupActivityFields();

            _selectedServer = HostServer.Default;
            _textServer.Text = "Server: " + _selectedServer.Name;
            Constants.ServerUri = _selectedServer.Address;
        }

        //============================================================
        protected override void OnResume()
        {
            if (_textInputUsername != null && _textInputPassword != null)
            {
                _textInputUsername.Text = string.Empty;
                _textInputPassword.Text = string.Empty;
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
            var servers = _serverService.GetAll();
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Choose a server");
            var serverStringList = servers.Select(x => x.Name).ToArray();
            alert.SetItems(serverStringList, (o, args) =>
            {
                _selectedServer = servers.ElementAt(args.Which);
                _textServer.Text = "Server: " + _selectedServer.Name;
                Constants.ServerUri = _selectedServer.Address;
            });

            alert.Create().Show();
        }

        //============================================================
        private void OnRegisterButtonClick(object sender, EventArgs e)
        {
            var intent = new Intent(Application.Context, typeof(RegisterActivity));
            StartActivity(intent);
        }

        //============================================================
        private async void OnLoginButtonClick(object sender, EventArgs e)
        {
            var inputManager = GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputManager?.HideSoftInputFromWindow(_textInputPassword.WindowToken, 0);

            if (ValidateFields())
            {
                var progressDialog = ProgressDialog.Show(this, "Login", "Logging in...");
                if (!await Utils.CheckConnectionAsync(Constants.ServerUri))
                {
                    Toast.MakeText(Application.Context, "Failed to connect to server!", ToastLength.Short).Show();
                    progressDialog.Dismiss();
                    return;
                }

                var users = await _userService.GetAllAsync();
                if (users.Any(x =>
                    x.Username.Trim() == _textInputUsername.Text.Trim() &&
                    x.Password.Trim() == Encryptor.Encrypt_SHA256(_textInputPassword.Text.Trim())))
                {
                    var user = users.First(x =>
                        x.Username.Trim() == _textInputUsername.Text.Trim() && x.Password.Trim() ==
                        Encryptor.Encrypt_SHA256(_textInputPassword.Text.Trim()));
                    progressDialog.Dismiss();
                    var intent = new Intent(Application.Context, typeof(MainActivity));
                    intent.PutExtra("user", JsonConvert.SerializeObject(user));
                    StartActivity(intent);
                }
                else
                {
                    progressDialog.Dismiss();
                    Toast.MakeText(Application.Context, "Username or password incorrect!", ToastLength.Short).Show();
                }
            }
        }

        //============================================================
        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(_textInputUsername.Text))
            {
                Toast.MakeText(Application.Context, "Please enter your username!", ToastLength.Short).Show();
                return false;
            }

            if (string.IsNullOrEmpty(_textInputPassword.Text))
            {
                Toast.MakeText(Application.Context, "Please enter your password!", ToastLength.Short).Show();
                return false;
            }

            return true;
        }
    }
}