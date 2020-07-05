using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class ImageFragment : Fragment
    {
        //============================================================
        public ImageFragment(ICollection<Image> images)
        {
            Arguments = new Bundle();
            Arguments.PutString("images", JsonConvert.SerializeObject(images));
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_images, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.imagesListView);

            var images = JsonConvert.DeserializeObject<ICollection<Image>>(Arguments.GetString("images"));
            listView.Adapter = new ImageViewModelAdapter(Activity, images);
            listView.ItemClick += ListViewOnItemClick;
            return view;
        }

        //============================================================
        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}