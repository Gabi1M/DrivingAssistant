﻿using System;
using Android.Content;
using DrivingAssistant.AndroidApp.Fragments.Camera;
using DrivingAssistant.AndroidApp.Fragments.Home;
using DrivingAssistant.AndroidApp.Fragments.Server;
using DrivingAssistant.AndroidApp.Fragments.Session;
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
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new HomeFragmentView(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_sessions:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new SessionFragmentView(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_servers:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new ServerFragmentView(_context, user)));
                        break;
                    }
                    case Resource.Id.nav_settings:
                    {
                        Notify(new NotificationEventArgs(NotificationCommand.MainActivity_Navigation, new RemoteCameraFragmentView(_context, user)));
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