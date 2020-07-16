using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
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
        private TextInputEditText _textInputUsername;
        private TextInputEditText _textInputPassword;
        private TextInputEditText _textInputConfirmPassword;
        private TextView _labelFirstname;
        private TextView _labelLastName;
        private TextView _labelUsername;
        private TextView _labelPassword;
        private TextView _labelConfirmPassword;
        private TextView _labelSelectedRole;
        private Button _registerButton;

        private UserRole _selectedRole = UserRole.None;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_register);

            _textInputFirstName = FindViewById<TextInputEditText>(Resource.Id.registerInputFirstName);
            _textInputLastName = FindViewById<TextInputEditText>(Resource.Id.registerInputLastName);
            _textInputUsername = FindViewById<TextInputEditText>(Resource.Id.registerInputUsername);
            _textInputPassword = FindViewById<TextInputEditText>(Resource.Id.registerInputPassword);
            _textInputConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.registerInputConfirmPassword);
            _labelFirstname = FindViewById<TextView>(Resource.Id.registerLabelFirstName);
            _labelLastName = FindViewById<TextView>(Resource.Id.registerLabelLastName);
            _labelUsername = FindViewById<TextView>(Resource.Id.registerLabelUsername);
            _labelPassword = FindViewById<TextView>(Resource.Id.registerLabelPassword);
            _labelConfirmPassword = FindViewById<TextView>(Resource.Id.registerLabelConfirmPassword);
            _labelSelectedRole = FindViewById<TextView>(Resource.Id.registerSelectRole);
            _registerButton = FindViewById<Button>(Resource.Id.registerButton);

            _textInputConfirmPassword.EditorAction += OnTextInputConfirmPasswordEditorAction;
            _labelSelectedRole.Click += LabelSelectedRoleOnClick;
            _registerButton.Click += OnRegisterButtonClick;
        }

        //============================================================
        private void LabelSelectedRoleOnClick(object sender, EventArgs e)
        {
            var enumItems = Enum.GetNames(typeof(UserRole)).ToList();
            enumItems.Remove(UserRole.Administrator.ToString());
            enumItems.Remove(UserRole.None.ToString());

            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Choose user role");
            alert.SetItems(enumItems.ToArray(), (o, args) =>
            {
                _selectedRole = (UserRole)Enum.Parse(typeof(UserRole), enumItems[args.Which]);
                _labelSelectedRole.Text = "Role: " + _selectedRole;
            });

            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnTextInputConfirmPasswordEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            try
            {
                if (e.Event.KeyCode == Keycode.Enter)
                {
                    _registerButton.PerformClick();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //============================================================
        private async void OnRegisterButtonClick(object sender, EventArgs e)
        {
            var inputManager = GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputManager.HideSoftInputFromWindow(_textInputPassword.WindowToken, 0);

            if (ValidateFields())
            {
                var progressDialog = ProgressDialog.Show(this, "Register", "Registering user...");
                if (!await Utils.CheckConnectionAsync("http://192.168.100.234:3287"))
                {
                    Toast.MakeText(Application.Context, "Failed to connect to server!", ToastLength.Short).Show();
                    progressDialog.Dismiss();
                    return;
                }

                var userService = new UserService("http://192.168.100.234:3287");
                var users = await userService.GetAsync();
                if (users.Any(x => x.Username.Trim() == _textInputUsername.Text.Trim()))
                {
                    Toast.MakeText(Application.Context, "There is already a user with the same username!", ToastLength.Short).Show();
                    progressDialog.Dismiss();
                    return;
                }

                var user = new User(
                    _textInputUsername.Text.Trim(),
                    Encryptor.Encrypt_SHA256(_textInputPassword.Text.Trim()),
                    _textInputFirstName.Text.Trim(),
                    _textInputLastName.Text.Trim(),
                    _selectedRole,
                    DateTime.Now);

                await userService.SetAsync(user);
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

            if (string.IsNullOrEmpty(_textInputConfirmPassword.Text))
            {
                Toast.MakeText(Application.Context, "Please confirm your password!", ToastLength.Short).Show();
                return false;
            }

            if (_textInputPassword.Text.Trim() != _textInputConfirmPassword.Text.Trim())
            {
                Toast.MakeText(Application.Context, "Passwords don't match!", ToastLength.Short).Show();
                return false;
            }

            if (_selectedRole == UserRole.None)
            {
                Toast.MakeText(Application.Context, "Please select your role!", ToastLength.Short).Show();
                return false;
            }

            return true;
        }
    }
}