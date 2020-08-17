using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using DrivingAssistant.AndroidApp.Tools;

namespace DrivingAssistant.AndroidApp.Activities.VideoList
{
    public class VideoListActivityPresenter
    {
        private readonly Context _context;
        public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;

        public readonly IEnumerable<Core.Models.Video> _videos;
        private int _selectedPosition = -1;
        private View _selectedView;

        //============================================================
        public VideoListActivityPresenter(Context context, IEnumerable<Core.Models.Video> videos)
        {
            _context = context;
            _videos = videos;
        }

        //============================================================
        public void ButtonViewClick()
        {
            if (_selectedPosition == -1)
            {
                OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.VideoListActivity_View, new Exception("No video selected!")));
                return;
            }

            try
            {
                OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.VideoListActivity_View, _videos.ElementAt(_selectedPosition)));
            }
            catch (Exception ex)
            {
                OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.VideoListActivity_View, ex));
            }
        }

        //============================================================
        public void ItemClick(int position, View view)
        {
            _selectedView?.SetBackgroundResource(0);
            _selectedPosition = position;
            _selectedView = view;
            OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.VideoListActivity_Item, view));
        }
    }
}