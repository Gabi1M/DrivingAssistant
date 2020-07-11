using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class ImageViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Media> _images;

        //============================================================
        public ImageViewModelAdapter(Activity activity, ICollection<Media> images)
        {
            _activity = activity;
            _images = images;
        }

        //============================================================
        public override int Count => _images.Count;

        //============================================================
        public override Object GetItem(int position)
        {
            return null;
        }

        //============================================================
        public override long GetItemId(int position)
        {
            return _images.ElementAt(position).Id;
        }

        //============================================================
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_image_list, parent, false);
            var textSource = view.FindViewById<TextView>(Resource.Id.imageTextSource);
            var textDateTime = view.FindViewById<TextView>(Resource.Id.imageTextDateTime);
            var textStatus = view.FindViewById<TextView>(Resource.Id.imageTextStatus);

            var currentImage = _images.ElementAt(position);

            textSource.Text = "Source: " + currentImage.Source;
            textDateTime.Text = "Date added: " + currentImage.DateAdded.ToString("dd.MM.yyyy HH:mm:ss");
            textStatus.Text = currentImage.ProcessedId == 0 ? "Status: Image not yet processed!" : "Status: Image processed!";

            return view;
        }
    }
}