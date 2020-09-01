using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Activities.VideoList
{
    public class VideoListActivityViewPresenter : ViewPresenter
    {
        public readonly IEnumerable<VideoRecording> _videos;
        private int _selectedPosition = -1;
        private View _selectedView;

        //============================================================
        public VideoListActivityViewPresenter(Context context, IEnumerable<VideoRecording> videos)
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
        public void ButtonViewReportClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_ViewReport, new Exception("No video selected!")));
                return;
            }

            try
            {
                var video = _videos.ElementAt(_selectedPosition);
                /*if (!video.IsProcessed())
                {
                    Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_ViewReport, new Exception("The selected video was not processed yet!")));
                    return;
                }*/
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_ViewReport, _videos.ElementAt(_selectedPosition)));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.VideoListActivity_ViewReport, ex));
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