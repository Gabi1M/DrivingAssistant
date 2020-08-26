using System;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Tools
{
    public class RemoteCameraController : IDisposable
    {
        private readonly RemoteCamera _remoteCamera;
        private readonly SshHelper _sshHelper;

        //============================================================
        public RemoteCameraController(RemoteCamera remoteCamera)
        {
            _remoteCamera = remoteCamera;
            _sshHelper = new SshHelper(_remoteCamera.Host, _remoteCamera.Username, _remoteCamera.Password);
            _sshHelper.Connect();
        }

        //============================================================
        public void StartRecording()
        {
            var url = Constants.ServerUri + "/upload_video_stream?Encoding=h264&UserId=" + _remoteCamera.UserId;
            var arguments = "\'" + url + "\' \'" + _remoteCamera.VideoLength + "\'";
            _sshHelper.SendCommand("nohup python camera_script.py " + arguments + " &");
        }

        //============================================================
        public async Task StartRecordingAsync()
        {
            await Task.Run(StartRecording);
        }

        //============================================================
        public void StopRecording()
        {
            var processes = _sshHelper.SendCommand("ps -A -eo pid,args | grep python").Split('\n');
            var process = processes.First(x => x.Contains("camera_script.py"));
            var pid = process.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            _sshHelper.SendCommand("kill " + pid.Trim());
        }

        //============================================================
        public async Task StopRecordingAsync()
        {
            await Task.Run(StopRecording);
        }

        //============================================================
        public RemoteCameraStatus GetStatus()
        {
            try
            {
                var processes = _sshHelper.SendCommand("ps -A -eo pid,args | grep python").Split('\n');
                return processes.Any(x => x.Contains("camera_script.py")) ? RemoteCameraStatus.Running : RemoteCameraStatus.Stopped;
            }
            catch (Exception)
            {
                return RemoteCameraStatus.Failed_to_retrieve;
            }
        }

        //============================================================
        public async Task<RemoteCameraStatus> GetStatusAsync()
        {
            return await Task.Run(GetStatus);
        }

        //============================================================
        public void Dispose()
        {
            _sshHelper.Dispose();
        }
    }
}