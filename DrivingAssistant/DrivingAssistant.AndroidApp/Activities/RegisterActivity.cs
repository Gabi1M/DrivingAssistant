using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views.InputMethods;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private TextInputEditText _textInputFirstName;
        private TextInputEditText _textInputLastName;
        private TextInputEditText _textInputEmail;
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private Button _registerButton;

        private readonly UserService _userService = new UserService();

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            SetupActivityFields();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _textInputFirstName = FindViewById<TextInputEditText>(Resource.Id.registerInputFirstName);
            _textInputLastName = FindViewById<TextInputEditText>(Resource.Id.registerInputLastName);
            _textInputUsername = FindViewById<TextInputEditText>(Resource.Id.registerInputUsername);
            _textInputPassword = FindViewById<TextInputEditText>(Resource.Id.registerInputPassword);
            _textInputEmail = FindViewById<TextInputEditText>(Resource.Id.registerInputEmail);
            _registerButton = FindViewById<Button>(Resource.Id.registerButton);

            _registerButton.Click += OnRegisterButtonClick;
        }

        //============================================================
        private async void OnRegisterButtonClick(object sender, EventArgs e)
        {
            var inputManager = GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputManager?.HideSoftInputFromWindow(_textInputPassword.WindowToken, 0);

            if (ValidateFields())
            {
                var progressDialog = ProgressDialog.Show(this, "Register", "Registering user...");
                if (!await Utils.CheckConnectionAsync(Constants.ServerUri))
                {
                    Toast.MakeText(Application.Context, "Failed to connect to server!", ToastLength.Short).Show();
                    progressDialog.Dismiss();
                    return;
                }

                var users = await _userService.GetAllAsync();
                if (users.Any(x => x.Username.Trim() == _textInputUsername.Text.Trim()))
                {
                    Toast.MakeText(Application.Context, "There is already a user with the same username!", ToastLength.Short).Show();
                    progressDialog.Dismiss();
                    return;
                }

                var user = new User
                {
                    Id = -1,
                    Username = _textInputUsername.Text.Trim(),
                    Password = Encryptor.Encrypt_SHA256(_textInputPassword.Text.Trim()),
                    Email = _textInputEmail.Text.Trim(),
                    FirstName = _textInputFirstName.Text.Trim(),
                    LastName = _textInputLastName.Text.Trim(),
                    Role = UserRole.Standard,
                    JoinDate = DateTime.Now
                };

                await _userService.SetAsync(user);
                progressDialog.Dismiss();
                Toast.MakeText(Application.Context, "Register successful!", ToastLength.Short).Show();
                await Task.Delay(1000);
                Finish();
            }
        }

        //============================================================
        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(_textInputFirstName.Text))
            {
                Toast.MakeText(Application.Context, "Please enter your first name!", ToastLength.Short).Show();
                return false;
            }

            if (string.IsNullOrEmpty(_textInputLastName.Text))
            {
                Toast.MakeText(Application.Context, "Please enter your last name!", ToastLength.Short).Show();
                return false;
            }

            if (string.IsNullOrEmpty(_textInputEmail.Text))
            {
                Toast.MakeText(Application.Context, "Please enter your email!", ToastLength.Short).Show();
                return false;
            }

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