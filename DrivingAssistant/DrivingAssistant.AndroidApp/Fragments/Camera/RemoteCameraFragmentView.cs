using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Camera
{
    public sealed class RemoteCameraFragmentView : Fragment
    {

        private TextView _textCameraSession;
        private TextView _textCameraHost;
        private TextView _textCameraUsername;
        private TextView _textCameraPassword;
        private EditText _textCameraVideoLength;
        private TextView _textCameraStatus;
        private CheckBox _checkAutoProcess;
        private TextView _textAutoProcessType;

        private Button _buttonStartRecording;
        private Button _buttonStopRecording;
        private Button _buttonSave;

        private readonly Context _activityContext;
        private readonly RemoteCameraFragmentViewPresenter _viewPresenter;

        //============================================================
        public RemoteCameraFragmentView(Context activityContext, User user)
        {
            _activityContext = activityContext;
            _viewPresenter = new RemoteCameraFragmentViewPresenter(activityContext, user);
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
                case NotificationCommand.SettingsFragment_AutoProcessType:
                {
                    _textAutoProcessType.Text = e.Data.ToString();
                    break;
                }
                case NotificationCommand.SettingsFragment_Save:
                {
                    Utils.ShowToast(_activityContext, "Settings saved successfully!", true);
                    break;
                }
                case NotificationCommand.SettingsFragment_Refresh:
                {
                    var (remoteCamera, item2, item3) = (Tuple<Core.Models.RemoteCamera, string, string>) e.Data;
                    _textCameraHost.Text = remoteCamera.Host;
                    _textCameraUsername.Text = remoteCamera.Username;
                    _textCameraPassword.Text = remoteCamera.Password;
                    _textCameraVideoLength.Text = remoteCamera.VideoLength.ToString();
                    _checkAutoProcess.Checked = remoteCamera.AutoProcessSession;
                    _textAutoProcessType.Text = remoteCamera.AutoProcessSessionType.ToString();
                    _textCameraStatus.Text = item2;
                    _textCameraSession.Text = item3;
                    break;
                }
            }
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_camera, container, false);
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
            _textCameraVideoLength = view.FindViewById<EditText>(Resource.Id.settingsTextRecordingLength);
            _textCameraStatus = view.FindViewById<TextView>(Resource.Id.settingsTextRecordingStatus);
            _checkAutoProcess = view.FindViewById<CheckBox>(Resource.Id.settingsCheckAutoProcess);
            _textAutoProcessType = view.FindViewById<TextView>(Resource.Id.settingsTextAutoProcessType);

            _buttonStartRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStartRecording);
            _buttonStopRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStopRecording);
            _buttonSave = view.FindViewById<Button>(Resource.Id.settingsButtonSave);

            _textCameraSession.Click += OnTextCameraSessionClick;
            _textAutoProcessType.Click += OnTextAutoProcessTypeClick;
            _buttonStartRecording.Click += OnStartRecordingClick;
            _buttonStopRecording.Click += OnStopRecordingClick;
            _buttonSave.Click += OnButtonSaveClick;
        }

        //============================================================
        private async void PopulateFields()
        {
            var progressDialog = Utils.ShowProgressDialog(_activityContext, "Remote Camera", "Loading settings ...");
            await _viewPresenter.RefreshData();
            progressDialog.Dismiss();
        }

        //============================================================
        private async void OnTextCameraSessionClick(object sender, EventArgs e)
        {
            await _viewPresenter.CameraSessionClick();
        }

        //============================================================
        private void OnTextAutoProcessTypeClick(object sender, EventArgs e)
        {
            _viewPresenter.AutoProcessTypeClick();
        }

        //============================================================
        private async void OnButtonSaveClick(object sender, EventArgs e)
        {
            var dialog = Utils.ShowProgressDialog(_activityContext, "Remote Camera", "Saving settings ...");
            await _viewPresenter.SaveClick(_textCameraHost.Text, _textCameraUsername.Text, _textCameraPassword.Text, _textCameraVideoLength.Text, _checkAutoProcess.Checked);
            dialog.Dismiss();
        }

        //============================================================
        private async void OnStartRecordingClick(object sender, EventArgs e)
        {
            var dialog = Utils.ShowProgressDialog(_activityContext, "Remote Camera", "Start recording ...");
            await _viewPresenter.StartRecordingClick();
            dialog.Dismiss();
        }

        //============================================================
        private async void OnStopRecordingClick(object sender, EventArgs e)
        {
            var dialog = Utils.ShowProgressDialog(_activityContext, "Remote Camera", "Stop recording ...");
            await _viewPresenter.StopRecordingClick();
            dialog.Dismiss();
        }
    }
}