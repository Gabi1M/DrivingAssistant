using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using Newtonsoft.Json;
using Uri = Android.Net.Uri;

namespace DrivingAssistant.AndroidApp.Activities.Video
{
    [Activity(Label = "VideoActivity", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideoActivity : Activity
    {
        private VideoView _videoView;
        private Core.Models.Video _video;
        private VideoActivityPresenter _presenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_video);
            SetupActivityFields();
            _presenter = new VideoActivityPresenter(this);
            _presenter.OnPropertyChanged += PresenterOnOnPropertyChanged;
            LoadVideo(_video);
        }

        //============================================================
        private void PresenterOnOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long)?.Show();
                return;
            }

            switch (e.Command)
            {
                case NotifyCommand.VideoActivity_LoadVideo:
                {
                    var mediaController = new MediaController(this);
                    mediaController.SetAnchorView(_videoView);
                    _videoView.SetVideoURI(e.Data as Uri);
                    _videoView.SetMediaController(mediaController);
                    _videoView.RequestFocus();
                    _videoView.Start();
                    break;
                }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
            _video = JsonConvert.DeserializeObject<Core.Models.Video>(Intent?.GetStringExtra("video")!);
        }

        //============================================================
        private void LoadVideo(Core.Models.Video video)
        {
            _presenter.LoadVideo(video);
        }
    }
}