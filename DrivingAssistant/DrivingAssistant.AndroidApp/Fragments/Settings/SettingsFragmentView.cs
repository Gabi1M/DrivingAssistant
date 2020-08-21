using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Settings
{
    public sealed class SettingsFragmentView : Fragment
    {

        private TextView _textCameraSession;
        private TextView _textCameraHost;
        private TextView _textCameraUsername;
        private TextView _textCameraPassword;
        private TextView _textCameraStatus;

        private Button _buttonStartRecording;
        private Button _buttonStopRecording;
        private Button _buttonSave;

        private readonly Context _activityContext;
        private readonly SettingsFragmentViewPresenter _viewPresenter;

        //============================================================
        public SettingsFragmentView(Context activityContext, User user)
        {
            _activityContext = activityContext;
            _viewPresenter = new SettingsFragmentViewPresenter(activityContext, user);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnOnNotificationReceived;
        }

        //============================================================
        private void ViewPresenterOnOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(_activityContext, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.SettingsFragment_StartRecording:
                {
                    _textCameraStatus.Text = e.Data as string;
                    break;
                }
                case NotificationCommand.SettingsFragment_StopRecording:
                {
                    _textCameraStatus.Text = e.Data as string;
                    break;
                }
                case NotificationCommand.SettingsFragment_CameraSesstion:
                {
                    _textCameraSession.Text = e.Data as string;
                    break;
                }
                case NotificationCommand.SettingsFragment_Save:
                {
                    Utils.ShowToast(_activityContext, "Settings saved successfully!", true);
                    break;
                }
                case NotificationCommand.SettingsFragment_Refresh:
                {
                    var (settings, item2, item3) = (Tuple<UserSettings, string, string>) e.Data;
                    _textCameraHost.Text = settings.CameraHost;
                    _textCameraUsername.Text = settings.CameraUsername;
                    _textCameraPassword.Text = settings.CameraPassword;
                    _textCameraStatus.Text = item2;
                    _textCameraSession.Text = item3;
                    break;
                }
            }
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
            _textCameraHost = view.FindViewById<TextView>(Resource.Id.settingsTextCameraHost);
            _textCameraUsername = view.FindViewById<TextView>(Resource.Id.settingsTextCameraUsername);
            _textCameraPassword = view.FindViewById<TextView>(Resource.Id.settingsTextCameraPassword);
            _textCameraStatus = view.FindViewById<TextView>(Resource.Id.settingsTextRecordingStatus);

            _buttonStartRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStartRecording);
            _buttonStopRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStopRecording);
            _buttonSave = view.FindViewById<Button>(Resource.Id.settingsButtonSave);

            _textCameraSession.Click += OnTextCameraSessionClick;
            _buttonStartRecording.Click += OnStartRecordingClick;
            _buttonStopRecording.Click += OnStopRecordingClick;
            _buttonSave.Click += OnButtonSaveClick;
        }

        //============================================================
        private async void PopulateFields()
        {
            var progressDialog = Utils.ShowProgressDialog(_activityContext, "Settings", "Loading data ...");
            await _viewPresenter.RefreshData();
            progressDialog.Dismiss();
        }

        //============================================================
        private async void OnTextCameraSessionClick(object sender, EventArgs e)
        {
            await _viewPresenter.CameraSessionClick();
        }

        //============================================================
        private async void OnButtonSaveClick(object sender, EventArgs e)
        {
            await _viewPresenter.SaveClick(_textCameraHost.Text, _textCameraUsername.Text, _textCameraPassword.Text);
        }

        //============================================================
        private async void OnStartRecordingClick(object sender, EventArgs e)
        { 
            await _viewPresenter.StartRecordingClick();
        }

        //============================================================
        private async void OnStopRecordingClick(object sender, EventArgs e)
        {
            await _viewPresenter.StopRecordingClick();
        }
    }
}