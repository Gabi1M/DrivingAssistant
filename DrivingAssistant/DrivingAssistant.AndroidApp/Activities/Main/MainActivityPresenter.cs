using System;
using Android.Content;
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.AndroidApp.Fragments.Home;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Activities.Main
{
    public class MainActivityPresenter
    {
        private readonly Context _context;
        public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;

        //============================================================
        public MainActivityPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public void NavigationItemSelect(int itemId, User user)
        {
            try
            {
                switch (itemId)
                {
                    case Resource.Id.nav_home:
                    {
                        OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, new HomeFragment(user)));
                        break;
                    }
                    case Resource.Id.nav_sessions:
                    {
                        OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, new SessionFragment(user)));
                        break;
                    }
                    case Resource.Id.nav_servers:
                    {
                        OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, new ServerFragment(user)));
                        break;
                    }
                    case Resource.Id.nav_settings:
                    {
                        OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, new SettingsFragment(user)));
                        break;
                    }
                    case Resource.Id.nav_logout:
                    {
                        OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, null));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OnPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NotifyCommand.MainActivity_Navigation, ex));
            }
        }
    }
}