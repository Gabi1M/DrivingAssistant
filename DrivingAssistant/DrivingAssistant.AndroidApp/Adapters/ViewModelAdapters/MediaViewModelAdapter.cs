using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class MediaViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Media> _images;

        //============================================================
        public MediaViewModelAdapter(Activity activity, ICollection<Media> images)
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_media_list, parent, false);
            var textType = view.FindViewById<TextView>(Resource.Id.mediaTextType);
            var textDescription = view.FindViewById<TextView>(Resource.Id.mediaTextDescription);
            var textSource = view.FindViewById<TextView>(Resource.Id.mediaTextSource);
            var textDateTime = view.FindViewById<TextView>(Resource.Id.mediaTextDateTime);
            var textStatus = view.FindViewById<TextView>(Resource.Id.mediaTextStatus);

            var currentImage = _images.ElementAt(position);

            textType.Text = currentImage.Type.ToString();
            textDescription.Text = "Description: " + currentImage.Description;
            textSource.Text = "Source: " + currentImage.Source;
            textDateTime.Text = "Date added: " + currentImage.DateAdded.ToString("dd.MM.yyyy HH:mm:ss");
            textStatus.Text = currentImage.ProcessedId == default ? "Status: Media not yet processed!" : "Status: Media processed!";

            return view;
        }
    }
}