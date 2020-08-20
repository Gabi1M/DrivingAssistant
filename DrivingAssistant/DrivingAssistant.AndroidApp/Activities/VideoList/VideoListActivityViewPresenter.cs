using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using DrivingAssistant.AndroidApp.Tools;

namespace DrivingAssistant.AndroidApp.Activities.VideoList
{
    public class VideoListActivityViewPresenter : ViewPresenter
    {
        public readonly IEnumerable<Core.Models.Video> _videos;
        private int _selectedPosition = -1;
        private View _selectedView;

        //============================================================
        public VideoListActivityViewPresenter(Context context, IEnumerable<Core.Models.Video> videos)
        {
            _context = context;
            _videos = videos;
        }

        //============================================================
        public void ButtonViewClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_View, new Exception("No video selected!")));
                return;
            }

            try
            {
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_View, _videos.ElementAt(_selectedPosition)));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_View, ex));
            }
        }

        //============================================================
        public void ItemClick(int position, View view)
        {
            _selectedView?.SetBackgroundResource(0);
            _selectedPosition = position;
            _selectedView = view;
            Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_Item, view));
        }
    }
}