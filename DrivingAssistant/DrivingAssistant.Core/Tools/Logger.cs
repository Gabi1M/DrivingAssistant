using System;
using System.IO;

namespace DrivingAssistant.Core.Tools
{
    public static class Logger
    {
        private static StreamWriter _file = StreamWriter.Null;
        private static string _filename = string.Empty;

        //===========================================================//
        public static void SetFilename(string filename)
        {
            _filename = filename;
            OpenLogFile(filename);
        }

        //===========================================================//
        public static void Log(string message, LogType type)
        {
            var fullMessage = string.Empty;
            switch (type)
            {
                case LogType.Error:
                {
                    fullMessage += "( E ) ";
                    break;
                }
                case LogType.Warning:
                {
                    fullMessage += "( W ) ";
                    break;
                }
                case LogType.Info:
                {
                    fullMessage += "( I ) ";
                    break;
                }
            }

            fullMessage += DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " + message;
            _file.WriteLine(fullMessage);
            _file.Flush();
        }

        //===========================================================//
        public static void LogException(Exception ex, LogType type = LogType.Error)
        {
            var fullMessage = string.Empty;
            switch (type)
            {
                case LogType.Error:
                {
                    fullMessage += "( E ) ";
                    break;
                }
                case LogType.Warning:
                {
                    fullMessage += "( W ) ";
                    break;
                }
                case LogType.Info:
                {
                    fullMessage += "( I ) ";
                    break;
                }
            }

            fullMessage += DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ": " + ex.Message + "\n" + ex.StackTrace;
            _file.WriteLine(fullMessage);
            _file.Flush();
        }

        //===========================================================//
        private static void OpenLogFile(string filename)
        {
            var fullFilename = Path.Combine(AppContext.BaseDirectory, filename);
            _file = new StreamWriter(File.Open(fullFilename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }
    }

    //===========================================================//
    public enum LogType
    {
        Error,
        Warning,
        Info
    }
}
