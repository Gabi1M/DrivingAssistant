using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "VideoListActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class VideoListActivity : AppCompatActivity
    {
        private IEnumerable<Video> _videos;
        private ListView _videoListView;

        private int _selectedPosition = -1;
        private View _selectedView;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_videos);
            SetupActivityFields();
            SetupListAdapter();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _videoListView = FindViewById<ListView>(Resource.Id.videosListView);
            _videos = JsonConvert.DeserializeObject<IEnumerable<Video>>(Intent.GetStringExtra("videos"));
        }

        //============================================================
        private void SetupListAdapter()
        {
            _videoListView.ChoiceMode = ChoiceMode.Single;
            _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, _videos.ToList());
            _videoListView.ItemClick += OnItemClick;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedPosition = e.Position;
            _selectedView = e.View;
        }
    }
}