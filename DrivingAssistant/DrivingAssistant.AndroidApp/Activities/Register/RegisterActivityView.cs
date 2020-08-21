using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;

namespace DrivingAssistant.AndroidApp.Activities.Register
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivityView : Activity
    {
        private TextInputEditText _textInputFirstName;
        private TextInputEditText _textInputLastName;
        private TextInputEditText _textInputEmail;
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private Button _registerButton;

        private RegisterActivityViewPresenter _viewPresenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            SetupActivityFields();

            _viewPresenter = new RegisterActivityViewPresenter(this);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
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
                case NotificationCommand.RegisterActivity_Register:
                {
                    Utils.ShowToast(this, "Registration successful!");
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
            var progressDialog = Utils.ShowProgressDialog(this, "Register", "Registering user ...");
            await _viewPresenter.RegisterButtonClick(_textInputFirstName.Text, _textInputLastName.Text,
                _textInputUsername.Text, _textInputPassword.Text, _textInputEmail.Text);
            progressDialog.Dismiss();
        }
    }
}