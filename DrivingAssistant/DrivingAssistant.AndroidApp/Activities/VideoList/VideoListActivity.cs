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

        private VideoListActivityPresenter _presenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_videos);
            SetupActivityFields();
            SetupListAdapter();

            _presenter = new VideoListActivityPresenter(this,
                JsonConvert.DeserializeObject<IEnumerable<Core.Models.Video>>(Intent?.GetStringExtra("videos")!));
            _presenter.OnPropertyChanged += PresenterOnPropertyChanged;
        }

        //============================================================
        private void PresenterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long)?.Show();
                return;
            }

            switch (e.Command)
            {
                case NotifyCommand.VideoListActivity_View:
                {
                    var intent = new Intent(this, typeof(VideoActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(e.Data as Core.Models.Video));
                    StartActivity(intent);
                    break;
                }
                case NotifyCommand.VideoListActivity_Item:
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
            _presenter.ButtonViewClick();
        }

        //============================================================
        private void SetupListAdapter()
        {
            _videoListView.ChoiceMode = ChoiceMode.Single;
            _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, _presenter._videos.ToList());
            _videoListView.ItemClick += OnItemClick;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _presenter.ItemClick(e.Position, e.View);
        }
    }
}