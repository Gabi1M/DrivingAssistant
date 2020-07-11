using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class VideoViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Media> _videos;

        //============================================================
        public VideoViewModelAdapter(Activity activity, ICollection<Media> videos)
        {
            _activity = activity;
            _videos = videos;
        }

        //============================================================
        public override int Count => _videos.Count;

        //============================================================
        public override Object GetItem(int position)
        {
            return null;
        }

        //============================================================
        public override long GetItemId(int position)
        {
            return _videos.ElementAt(position).Id;
        }

        //============================================================
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_video_list, parent, false);
            var textSource = view.FindViewById<TextView>(Resource.Id.videoTextSource);
            var textDateTime = view.FindViewById<TextView>(Resource.Id.videoTextDateTime);
            var textStatus = view.FindViewById<TextView>(Resource.Id.videoTextStatus);

            var currentVideo = _videos.ElementAt(position);

            textSource.Text = "Source: " + currentVideo.Source;
            textDateTime.Text = "Date added: " + currentVideo.DateAdded.ToString("dd.MM.yyy HH:mm:ss");
            textStatus.Text = currentVideo.ProcessedId == 0 ? "Status: Video not yet processed!" : "Status: Video processed!";

            return view;
        }
    }
}