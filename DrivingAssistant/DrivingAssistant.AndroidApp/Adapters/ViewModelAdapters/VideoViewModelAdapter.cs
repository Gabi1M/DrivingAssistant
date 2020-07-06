using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class VideoViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Video> _videos;

        //============================================================
        public VideoViewModelAdapter(Activity activity, ICollection<Video> videos)
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
            var textWidth = view.FindViewById<TextView>(Resource.Id.videoTextWidth);
            var textHeight = view.FindViewById<TextView>(Resource.Id.videoTextHeight);
            var textFps = view.FindViewById<TextView>(Resource.Id.videoTextFps);
            var textFormat = view.FindViewById<TextView>(Resource.Id.videoTextFormat);
            var textSource = view.FindViewById<TextView>(Resource.Id.videoTextSource);
            var textDateTime = view.FindViewById<TextView>(Resource.Id.videoTextDateTime);

            var currentVideo = _videos.ElementAt(position);

            textWidth.Text = "Width: " + currentVideo.Width;
            textHeight.Text = "Height: " + currentVideo.Height;
            textFps.Text = "FPS: " + currentVideo.Fps;
            textFormat.Text = "Format: " + currentVideo.Format;
            textSource.Text = "Source: " + currentVideo.Source;
            textDateTime.Text = "Date taken: " + currentVideo.DateTime.ToString("dd.MM.yyy HH:mm:ss");

            return view;
        }
    }
}