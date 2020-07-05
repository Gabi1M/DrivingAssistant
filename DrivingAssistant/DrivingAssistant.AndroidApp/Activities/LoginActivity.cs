using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Tools;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "LoginActivity", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private TextView _labelUsername;
        private TextView _labelPassword;
        private ProgressBar _progressBar;
        private Button _loginButton;
        private Button _registerButton;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            _textInputUsername = FindViewById<TextInputEditText>(Resource.Id.loginInputUsername);
            _textInputPassword = FindViewById<TextInputEditText>(Resource.Id.loginInputPassword);
            _labelUsername = FindViewById<TextView>(Resource.Id.loginLabelUsername);
            _labelPassword = FindViewById<TextView>(Resource.Id.loginLabelPassword);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            _loginButton = FindViewById<Button>(Resource.Id.loginButton);
            _registerButton = FindViewById<Button>(Resource.Id.loginRegisterButton);

            _progressBar.Visibility = ViewStates.Invisible;

            _loginButton.Click += OnLoginButtonClick;
            _registerButton.Click += OnRegisterButtonClick;
            _textInputPassword.EditorAction += OnTextInputPasswordEditorAction;
        }

        //============================================================
        private void OnTextInputPasswordEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            try
            {
                if (e.Event.KeyCode == Keycode.Enter)
                {
                    _loginButton.PerformClick();
                }
            }
            catch (Exception)
            {
                // ignored
            }
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
            inputManager.HideSoftInputFromWindow(_textInputPassword.WindowToken, 0);

            _loginButton.Enabled = false;
            _registerButton.Enabled = false;

            if (ValidateFields())
            {
                _progressBar.Visibility = ViewStates.Visible;

                if (!await Utils.CheckConnectionAsync("http://192.168.100.246:3287"))
                {
                    Toast.MakeText(Application.Context, "Failed to connect to server!", ToastLength.Short).Show();
                    _progressBar.Visibility = ViewStates.Invisible;
                    _loginButton.Enabled = true;
                    _registerButton.Enabled = true;
                    return;
                }

                using var userService = new UserService("http://192.168.100.246:3287");
                var users = await userService.GetAsync();
                _loginButton.Enabled = true;
                _registerButton.Enabled = true;
                if (users.Any(x =>
                    x.Username.Trim() == _textInputUsername.Text.Trim() &&
                    x.Password.Trim() == Encryptor.Encrypt_SHA256(_textInputPassword.Text.Trim())))
                {
                    _progressBar.Visibility = ViewStates.Invisible;
                    var intent = new Intent(Application.Context, typeof(MainActivity));
                    StartActivity(intent);
                }
                else
                {
                    _progressBar.Visibility = ViewStates.Invisible;
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