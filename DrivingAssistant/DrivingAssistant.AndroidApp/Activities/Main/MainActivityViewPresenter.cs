using System;
using Android.Content;
using DrivingAssistant.AndroidApp.Fragments.Home;
using DrivingAssistant.AndroidApp.Fragments.Server;
using DrivingAssistant.AndroidApp.Fragments.Session;
using DrivingAssistant.AndroidApp.Fragments.Settings;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Activities.Main
{
    public class MainActivityViewPresenter : ViewPresenter
    {
        //============================================================
        public MainActivityViewPresenter(Context context)
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
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new HomeFragment(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_sessions:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new SessionFragment(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_servers:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new ServerFragment(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_settings:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new SettingsFragment(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_logout:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, null));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, ex));
            }
        }
    }
}