using System.Collections.Generic;
using Android.Support.V4.App;
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace DrivingAssistant.AndroidApp.Adapters.FragmentAdapters
{
    public class VideoFragmentAdapter : FragmentPagerAdapter
    {
        private readonly ICollection<Video> _videos;

        //============================================================
        public VideoFragmentAdapter(FragmentManager fragmentManager, ICollection<Video> videos) : base(fragmentManager)
        {
            _videos = videos;
        }

        //============================================================
        public override int Count => 1;

        //============================================================
        public override Fragment GetItem(int position)
        {
            return new VideoFragment(_videos);
        }
    }
}