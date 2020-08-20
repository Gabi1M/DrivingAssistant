using System;
using Android.Content;

namespace DrivingAssistant.AndroidApp.Tools
{
    public abstract class ViewPresenter
    {
        protected Context _context;
        protected internal event EventHandler<NotificationEventArgs> OnNotificationReceived;

        //============================================================
        protected void Notify(NotificationEventArgs args)
        {
            OnNotificationReceived?.Invoke(this, args);
        }
    }
}