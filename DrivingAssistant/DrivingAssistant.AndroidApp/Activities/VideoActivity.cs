using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "VideoActivity")]
    public class VideoActivity : AppCompatActivity
    {
        private VideoView _videoView;
        private Video _video;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_video);
            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
            _video = JsonConvert.DeserializeObject<Video>(Intent.GetStringExtra("video"));
            LoadVideo();
        }

        //============================================================
        private void LoadVideo()
        {
            var mediaController = new MediaController(this);
            mediaController.SetAnchorView(_videoView);
            _videoView.SetVideoURI(Android.Net.Uri.Parse("http://192.168.100.246:3287/videos_download?id=" + _video.Id));
            _videoView.SetMediaController(mediaController);
            _videoView.RequestFocus();
            _videoView.Start();
        }
    }
}