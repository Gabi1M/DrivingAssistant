using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class VideoThumbnailViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Video> _videos;

        private readonly VideoService _videoService = new VideoService();

        //============================================================
        public VideoThumbnailViewModelAdapter(Activity activity, ICollection<Video> videos)
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_videoThumbnail_list, parent, false);
            var textType = view.FindViewById<TextView>(Resource.Id.videoThumbnailTextType);
            var textDescription = view.FindViewById<TextView>(Resource.Id.videoThumbnailTextDescription);
            var imageView = view.FindViewById<ImageView>(Resource.Id.videoThumbnailImage);
            imageView.SetScaleType(ImageView.ScaleType.Center);

            var currentVideo = _videos.ElementAt(position);
            textDescription.Text = "Name: " + currentVideo.Description;

            return view;
        }
    }
}