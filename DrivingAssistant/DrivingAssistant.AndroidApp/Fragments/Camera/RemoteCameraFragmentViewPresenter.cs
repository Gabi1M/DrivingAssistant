using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Fragments.Camera
{
    public class RemoteCameraFragmentViewPresenter : ViewPresenter
    {
        private readonly User _user;
        private readonly RemoteCameraService _remoteCameraService = new RemoteCameraService();
        private readonly SessionService _sessionService = new SessionService();

        private RemoteCamera _remoteCamera;

        //============================================================
        public RemoteCameraFragmentViewPresenter(Context context, User user)
        {
            _context = context;
            _user = user;
        }

        //============================================================
        public async Task RefreshData()
        {
            _remoteCamera = await _remoteCameraService.GetByUserAsync(_user.Id);
            var status = RemoteCameraStatus.Failed_to_retrieve;
            try
            {
                await Task.Run(async () =>
                {
                    using var remoteCameraController = new RemoteCameraController(_remoteCamera);
                    status = await remoteCameraController.GetStatusAsync();
                });
            }
            catch (Exception)
            {
                //Ignored
            }

            try
            {
                var cameraSessionName = _remoteCamera.DestinationSessionId == -1 ? "None" : (await _sessionService.GetByIdAsync(_remoteCamera.DestinationSessionId)).Name;
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Refresh, new Tuple<RemoteCamera, RemoteCameraStatus, string>(_remoteCamera, status, cameraSessionName)));
            }
            catch (Exception)
            {
                _remoteCamera.DestinationSessionId = -1;
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Refresh, new Tuple<RemoteCamera, RemoteCameraStatus, string>(_remoteCamera, status, "None")));
            }
        }

        //============================================================
        public async Task CameraSessionClick()
        {
            var availableSessions = (await _sessionService.GetByUserAsync(_user.Id)).Where(x => x.Status == SessionStatus.Unprocessed);
            var sessionStringList = availableSessions.Select(x => x.Name).ToList();
            sessionStringList.Add("None");
            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Select Camera Session");
            alert.SetItems(sessionStringList.ToArray(), (o, args) =>
            {
                if (args.Which == sessionStringList.Count - 1)
                {
                    _remoteCamera.DestinationSessionId = -1;
                    Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_CameraSesstion, "None"));
                }
                else
                {
                    var selectedSession = availableSessions.ElementAt(args.Which);
                    _remoteCamera.DestinationSessionId = selectedSession.Id;
                    Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_CameraSesstion, selectedSession.Name));
                }
            });

            alert.Create()?.Show();
        }

        //============================================================
        public void AutoProcessTypeClick()
        {
            try
            {
                var algorithms = Enum.GetNames(typeof(ProcessingAlgorithmType));
                var alert = new AlertDialog.Builder(_context);
                alert.SetItems(algorithms, (sender, args) =>
                {
                    _remoteCamera.AutoProcessSessionType = Enum.Parse<ProcessingAlgorithmType>(algorithms.ElementAt(args.Which));
                    Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_AutoProcessType, _remoteCamera.AutoProcessSessionType));
                });
                alert.Create()?.Show();
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_AutoProcessType, ex));
            }
        }

        //============================================================
        public async Task StartRecordingClick()
        {
            try
            {
               using var remoteCameraController = new RemoteCameraController(_remoteCamera);
               await remoteCameraController.StartRecordingAsync();
               var status = await remoteCameraController.GetStatusAsync();
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
                using var remoteCameraController = new RemoteCameraController(_remoteCamera);
                await remoteCameraController.StopRecordingAsync();
                var status = await remoteCameraController.GetStatusAsync();
                if (_remoteCamera.AutoProcessSession)
                {
                    await _sessionService.SubmitAsync(_remoteCamera.DestinationSessionId,
                        ProcessingAlgorithmType.Lane_Departure_Warning);
                }
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StopRecording, status));
            }
            catch (Exception)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_StopRecording, new Exception("Failed to stop camera recording!")));
            }
        }

        //============================================================
        public async Task SaveClick(string cameraHost, string cameraUsername, string cameraPassword, string videoLength, bool autoProcess)
        {
            if (string.IsNullOrEmpty(cameraHost) || string.IsNullOrEmpty(cameraUsername) ||
                string.IsNullOrEmpty(cameraPassword))
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, new Exception("The data is not valid!")));
                return;
            }

            _remoteCamera.Host = cameraHost;
            _remoteCamera.Username = cameraUsername;
            _remoteCamera.Password = cameraPassword;
            _remoteCamera.VideoLength = Convert.ToInt32(videoLength);
            _remoteCamera.AutoProcessSession = autoProcess;
            try
            {
                await _remoteCameraService.SetAsync(_remoteCamera);
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, null));
            }
            catch (Exception)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SettingsFragment_Save, new Exception("Failed to save settings!")));
            }
        }
    }
}