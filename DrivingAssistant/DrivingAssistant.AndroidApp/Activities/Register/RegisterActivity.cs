using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;

namespace DrivingAssistant.AndroidApp.Activities.Register
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
        private ProgressDialog _progressDialog;

        private RegisterActivityPresenter _presenter;
        private readonly UserService _userService = new UserService();

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            SetupActivityFields();
            _presenter = new RegisterActivityPresenter(this);
            _presenter.OnPropertyChanged += PresenterOnPropertyChanged;
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
                case NotifyCommand.RegisterActivity_Register:
                {
                    Toast.MakeText(this, "Registration successful!", ToastLength.Short)?.Show();
                    Finish();
                    break;
                }
            }
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
            _progressDialog = ProgressDialog.Show(this, "Register", "Registering user...");
            await _presenter.RegisterButtonClick(_textInputFirstName.Text, _textInputLastName.Text,
                _textInputUsername.Text, _textInputPassword.Text, _textInputEmail.Text);
        }
    }
}