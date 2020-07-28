using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class MediaThumbnailViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Media> _medias;

        //============================================================
        public MediaThumbnailViewModelAdapter(Activity activity, ICollection<Media> medias)
        {
            _activity = activity;
            _medias = medias;
        }

        //============================================================
        public override int Count => _medias.Count;

        //============================================================
        public override Object GetItem(int position)
        {
            return null;
        }

        //============================================================
        public override long GetItemId(int position)
        {
            return _medias.ElementAt(position).Id;
        }

        //============================================================
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_mediaThumbnail_list, parent, false);
            var textType = view.FindViewById<TextView>(Resource.Id.mediaThumbnailTextType);
            var textDescription = view.FindViewById<TextView>(Resource.Id.mediaThumbnailTextDescription);
            var imageView = view.FindViewById<ImageView>(Resource.Id.mediaThumbnailImage);
            imageView.SetScaleType(ImageView.ScaleType.Center);

            var currentMedia = _medias.ElementAt(position);
            if (currentMedia.Type == MediaType.Image)
            {
                var mediaService = new MediaService();
                var bitmap = mediaService.DownloadImage(currentMedia.Id);
                bitmap = Bitmap.CreateScaledBitmap(bitmap, 128, 128, false);
                imageView.SetImageBitmap(bitmap);
            }

            textType.Text = currentMedia.Type.ToString();
            textDescription.Text = "Name: " + currentMedia.Description;

            return view;
        }
    }
}