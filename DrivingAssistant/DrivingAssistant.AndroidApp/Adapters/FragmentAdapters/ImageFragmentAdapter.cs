using System.Collections.Generic;
using Android.Support.V4.App;
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace DrivingAssistant.AndroidApp.Adapters.FragmentAdapters
{
    public class ImageFragmentAdapter : FragmentPagerAdapter
    {
        private readonly ICollection<Image> _images;

        //============================================================
        public ImageFragmentAdapter(FragmentManager fragmentManager, ICollection<Image> images) : base(fragmentManager)
        {
            _images = images;
        }

        //============================================================
        public override int Count => 1;

        //============================================================
        public override Fragment GetItem(int position)
        {
            return new ImageFragment(_images);
        }
    }
}