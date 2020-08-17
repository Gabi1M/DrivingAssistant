using System;

namespace DrivingAssistant.AndroidApp.Tools
{
    public class PropertyChangedEventArgs : EventArgs
    {
        //============================================================
        public PropertyChangedEventArgs(NotifyCommand command, object data)
        {
            Command = command;
            Data = data;
        }

        public object Data { get; set; }
        public NotifyCommand Command { get; set; }
    }
}