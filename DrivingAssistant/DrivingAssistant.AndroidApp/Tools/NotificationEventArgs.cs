using System;

namespace DrivingAssistant.AndroidApp.Tools
{
    public class NotificationEventArgs : EventArgs
    {
        //============================================================
        public NotificationEventArgs(NotificationCommand command, object data)
        {
            Command = command;
            Data = data;
        }

        public object Data { get; set; }
        public NotificationCommand Command { get; set; }
    }
}