using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "VideoActivity", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideoActivity : AppCompatActivity
    {
        private VideoView _videoView;
        private Media _video;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_video);
            SetupActivityFields();
            LoadVideo();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
            _video = JsonConvert.DeserializeObject<Media>(Intent.GetStringExtra("video"));
        }

        //============================================================
        private void LoadVideo()
        {
            var mediaController = new MediaController(this);
            mediaController.SetAnchorView(_videoView);
            _videoView.SetVideoURI(Uri.Parse(Constants.ServerUri + "/" + Endpoints.MediaEndpoints.Download + "?Id=" + _video.Id));
            _videoView.SetMediaController(mediaController);
            _videoView.RequestFocus();
            _videoView.Start();
        }
    }
}