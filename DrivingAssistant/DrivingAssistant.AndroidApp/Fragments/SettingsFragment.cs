using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SettingsFragment : Fragment
    {
        private readonly User _user;
        private UserSettings _userSettings;

        private readonly UserSettingsService _userSettingsService = new UserSettingsService();
        private readonly SessionService _sessionService = new SessionService();

        private TextView _textCameraSession;
        private TextView _textCameraIp;
        private Button _buttonSave;

        //============================================================
        public SettingsFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);
            SetupFragmentFields(view);
            PopulateFields();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _textCameraSession = view.FindViewById<TextView>(Resource.Id.settingsTextCameraSession);
            _textCameraIp = view.FindViewById<TextView>(Resource.Id.settingsTextCameraIp);
            _buttonSave = view.FindViewById<Button>(Resource.Id.settingsButtonSave);

            _textCameraSession.Click += OnTextCameraSessionClick;
            _buttonSave.Click += OnButtonSaveClick;
        }

        //============================================================
        private async void PopulateFields()
        {
            _userSettings = await _userSettingsService.GetByUserAsync(_user.Id);
            _textCameraIp.Text = _userSettings.CameraIp;
            if (_userSettings.CameraSessionId != -1)
            {
                var cameraSession = await _sessionService.GetByIdAsync(_userSettings.CameraSessionId);
                _textCameraSession.Text = cameraSession.Name;
            }
            else
            {
                _textCameraSession.Text = "None";
            }
        }

        //============================================================
        private async void OnTextCameraSessionClick(object sender, EventArgs e)
        {
            var availableSessions = (await _sessionService.GetByUserAsync(_user.Id)).Where(x => x.Status == SessionStatus.Unprocessed);
            var sessionStringList = availableSessions.Select(x => x.Name).ToList();
            sessionStringList.Add("None");
            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Select Camera Session");
            alert.SetItems(sessionStringList.ToArray(), (o, args) =>
            {
                if (args.Which == sessionStringList.Count - 1)
                {
                    _userSettings.CameraSessionId = -1;
                    _textCameraSession.Text = "None";
                }
                else
                {
                    var selectedSession = availableSessions.ElementAt(args.Which);
                    _userSettings.CameraSessionId = selectedSession.Id;
                    _textCameraSession.Text = selectedSession.Name;
                }
            });

            alert.Create().Show();
        }

        //============================================================
        private async void OnButtonSaveClick(object sender, EventArgs e)
        {
            _userSettings.CameraIp = _textCameraIp.Text;
            await _userSettingsService.SetAsync(_userSettings);
            Toast.MakeText(Context, "Settings successfully saved!", ToastLength.Short).Show();
        }
    }
}