using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Uri = Android.Net.Uri;

namespace DrivingAssistant.AndroidApp.Activities.Video
{
    [Activity(Label = "VideoActivity", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideoActivityView : Activity
    {
        private VideoView _videoView;

        private VideoRecording _videoRecording;
        private VideoActivityViewPresenter _viewPresenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_video);
            SetupActivityFields();

            _viewPresenter = new VideoActivityViewPresenter(this);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnOnNotificationReceived;

            LoadVideo(_videoRecording);
        }

        //============================================================
        private void ViewPresenterOnOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(this, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.VideoActivity_LoadVideo:
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
            _videoRecording = JsonConvert.DeserializeObject<VideoRecording>(Intent?.GetStringExtra("video")!);
        }

        //============================================================
        private void LoadVideo(VideoRecording videoRecording)
        {
            _viewPresenter.LoadVideo(videoRecording);
        }
    }
}