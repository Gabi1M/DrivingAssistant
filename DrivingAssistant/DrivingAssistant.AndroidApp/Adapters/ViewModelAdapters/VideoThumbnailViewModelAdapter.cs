using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
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
        private readonly ThumbnailService _thumbnailService = new ThumbnailService();

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
            var textDescription = view.FindViewById<TextView>(Resource.Id.videoThumbnailTextDescription);
            var imageView = view.FindViewById<ImageView>(Resource.Id.videoThumbnailImage);
            imageView.SetScaleType(ImageView.ScaleType.Center);

            var currentVideo = _videos.ElementAt(position);
            var thumbnail = _thumbnailService.GetByVideoAsync(currentVideo.Id).Result;
            var thumbnailImage = Bitmap.CreateScaledBitmap(_thumbnailService.DownloadAsync(thumbnail.Id).Result, 128, 128, false);

            textDescription.Text = "Name: " + currentVideo.Description;
            imageView.SetImageBitmap(thumbnailImage);

            return view;
        }
    }
}