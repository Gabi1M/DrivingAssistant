using System;
using Android.Content;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Tools;
using Uri = Android.Net.Uri;

namespace DrivingAssistant.AndroidApp.Activities.Video
{
    public class VideoActivityPresenter
    {
        private readonly Context _context;
        public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;

        //============================================================
        public VideoActivityPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public void LoadVideo(Core.Models.Video video)
        {
            OnPropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(NotifyCommand.VideoActivity_LoadVideo,
                    Uri.Parse(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.Download + "?Id=" + video.Id)));
        }
    }
}