using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class VideoFragment : Fragment
    {
        //============================================================
        public VideoFragment(ICollection<Video> videos)
        {
            Arguments = new Bundle();
            Arguments.PutString("videos", JsonConvert.SerializeObject(videos));
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_videos, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.videosListView);

            var videos = JsonConvert.DeserializeObject<ICollection<Video>>(Arguments.GetString("videos"));
            listView.Adapter = new VideoViewModelAdapter(Activity, videos);
            listView.ItemClick += OnItemClick;
            return view;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var video = JsonConvert.DeserializeObject<ICollection<Video>>(Arguments.GetString("videos")).ElementAt(e.Position);
            var intent = new Intent(Context, typeof(VideoActivity));
            intent.PutExtra("video", JsonConvert.SerializeObject(video));
            StartActivity(intent);
        }
    }
}