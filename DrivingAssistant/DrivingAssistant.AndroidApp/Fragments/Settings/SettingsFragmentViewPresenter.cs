using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Fragments.Settings
{
    public class SettingsFragmentViewPresenter : ViewPresenter
    {
        private readonly User _user;
        private readonly UserSettingsService _userSettingsService = new UserSettingsService();
        private readonly SessionService _sessionService = new SessionService();

        private UserSettings _userSettings;

        //============================================================
        public SettingsFragmentViewPresenter(Context context, User user)
        {
            _context = context;
            _user = user;
        }

        //============================================================
        public async Task RefreshData()
        {
            _userSettings = await _userSettingsService.GetByUserAsync(_user.Id);
            string cameraStatus;
            try
            {
                cameraStatus = await _userSettingsService.GetRecordingStatus(_user.Id);
            }
            catch (Exception)
            {
                cameraStatus = "Failed to retrieve camera status!";
            }
            var cameraSessionName = _userSettings.CameraSessionId == -1 ? "None" : (await _sessionService.GetByIdAsync(_userSettings.CameraSessionId)).Name;
            Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Refresh, new Tuple<UserSettings, string, string>(_userSettings, cameraStatus, cameraSessionName)));
        }

        //============================================================
        public async Task CameraSessionClick()
        {
            var availableSessions = (await _sessionService.GetByUserAsync(_user.Id))
                .Where(x => x.Status == SessionStatus.Unprocessed);
            var sessionStringList = availableSessions.Select(x => x.Name).ToList();
            sessionStringList.Add("None");
            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Select Camera Session");
            alert.SetItems(sessionStringList.ToArray(), (o, args) =>
            {
                if (args.Which == sessionStringList.Count - 1)
                {
                    _userSettings.CameraSessionId = -1;
                    Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_CameraSesstion, "None"));
                }
                else
                {
                    var selectedSession = availableSessions.ElementAt(args.Which);
                    _userSettings.CameraSessionId = selectedSession.Id;
                    Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_CameraSesstion, selectedSession.Name));
                }
            });

            alert.Create()?.Show();
        }

        //============================================================
        public async Task StartRecordingClick()
        {
            try
            {
                await _userSettingsService.StartRecordingAsync(_user.Id);
                var status = await _userSettingsService.GetRecordingStatus(_user.Id);
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StartRecording, status));
            }
            catch (Exception)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StartRecording, new Exception("Failed to start camera recording!")));
            }
        }

        //============================================================
        public async Task StopRecordingClick()
        {
            try
            {
                await _userSettingsService.StopRecordingAsync(_user.Id);
                var status = await _userSettingsService.GetRecordingStatus(_user.Id);
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StopRecording, status));
            }
            catch (Exception)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StopRecording, "Failed to stop camera recording!"));
            }
        }

        //============================================================
        public async Task SaveClick(string cameraHost, string cameraUsername, string cameraPassword)
        {
            if (string.IsNullOrEmpty(cameraHost) || string.IsNullOrEmpty(cameraUsername) ||
                string.IsNullOrEmpty(cameraPassword))
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, new Exception("The data is not valid!")));
                return;
            }

            _userSettings.CameraHost = cameraHost;
            _userSettings.CameraUsername = cameraUsername;
            _userSettings.CameraPassword = cameraPassword;
            try
            {
                await _userSettingsService.SetAsync(_userSettings);
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, null));
            }
            catch (Exception)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, new Exception("Failed to save settings!")));
            }
        }
    }
}