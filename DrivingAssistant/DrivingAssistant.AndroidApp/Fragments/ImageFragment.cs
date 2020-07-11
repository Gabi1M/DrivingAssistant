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
    public class ImageFragment : Fragment
    {
        //============================================================
        public ImageFragment(ICollection<Media> images)
        {
            Arguments = new Bundle();
            Arguments.PutString("images", JsonConvert.SerializeObject(images));
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_images, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.imagesListView);

            var images = JsonConvert.DeserializeObject<ICollection<Media>>(Arguments.GetString("images"));
            listView.Adapter = new ImageViewModelAdapter(Activity, images);
            listView.ItemClick += OnItemClick;
            return view;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var image = JsonConvert.DeserializeObject<ICollection<Media>>(Arguments.GetString("images")).ElementAt(e.Position);
            var intent = new Intent(Context, typeof(GalleryActivity));
            intent.PutExtra("image", JsonConvert.SerializeObject(image));
            StartActivity(intent);
        }
    }
}