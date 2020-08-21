using Android.Content;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Uri = Android.Net.Uri;

namespace DrivingAssistant.AndroidApp.Activities.Video
{
    public class VideoActivityViewPresenter : ViewPresenter
    {
        //============================================================
        public VideoActivityViewPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public void LoadVideo(VideoRecording videoRecording)
        {
            Notify(new NotificationEventArgs(NotificationCommand.VideoActivity_LoadVideo,
                Uri.Parse(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.Download + "?Id=" + videoRecording.Id)));
        }
    }
}