using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities.Video;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Tools;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace DrivingAssistant.AndroidApp.Activities.VideoList
{
    [Activity(Label = "VideoListActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class VideoListActivity : Activity
    {
        private ListView _videoListView;
        private Button _buttonView;

        private VideoListActivityViewPresenter _viewPresenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_videos);
            SetupActivityFields();

            _viewPresenter = new VideoListActivityViewPresenter(this,
                JsonConvert.DeserializeObject<IEnumerable<Core.Models.Video>>(Intent?.GetStringExtra("videos")!));
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;

            SetupListAdapter();
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(this, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.VideoListActivity_View:
                {
                    var intent = new Intent(this, typeof(VideoActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(e.Data as Core.Models.Video));
                    StartActivity(intent);
                    break;
                }
                case NotificationCommand.VideoListActivity_Item:
                {
                    var view = e.Data as View;
                    view?.SetBackgroundResource(Resource.Drawable.list_element_border);
                    break;
                }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _videoListView = FindViewById<ListView>(Resource.Id.videosListView);
            _buttonView = FindViewById<Button>(Resource.Id.videoListButtonView);
            _buttonView.Click += OnButtonViewClick;
        }

        //============================================================
        private void OnButtonViewClick(object sender, EventArgs e)
        {
            _viewPresenter.ButtonViewClick();
        }

        //============================================================
        private void SetupListAdapter()
        {
            _videoListView.ChoiceMode = ChoiceMode.Single;
            _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, _viewPresenter._videos.ToList());
            _videoListView.ItemClick += OnItemClick;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _viewPresenter.ItemClick(e.Position, e.View);
        }
    }
}