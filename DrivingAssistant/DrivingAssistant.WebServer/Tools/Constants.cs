using System.Runtime.InteropServices;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Constants
    {
        //============================================================
        public static class ServerConstants
        {
            private const string LinuxThumbnailStoragePath = @"/mnt/hdd/CloudStorage/Thumbnails";
            private const string LinuxVideoStoragePath = @"/mnt/hdd/CloudStorage/Videos";
            private const string LinuxPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=driving_assistant;Username=pi;Password=1234";
            private const string LinuxMSSQLConnectionString = @"";

            private const string WindowsThumbnailStoragePath = @"E:\CloudStorage\Thumbnails";
            private const string WindowsVideoStoragePath = @"E:\CloudStorage\Videos";
            private const string WindowsPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=driving_assistant;Username=postgres;Password=1234";
            private const string WindowsMSSQLConnectionString = @"Data Source=DESKTOP-KLAJVKV;Initial Catalog=DrivingAssistant;Persist Security Info=True;User ID=sa;Password=pxd";

            public static bool UsePostgresql = true;

            public const string MailHost = "smtp.gmail.com";
            public const int MailPort = 465;
            public const string SenderAddress = "noreply.drivingassistant@gmail.com";
            public const string SenderPassword = "driving.0.assistant";

            //============================================================
            public static string GetImageStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxThumbnailStoragePath : WindowsThumbnailStoragePath;
            }

            //============================================================
            public static string GetVideoStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxVideoStoragePath : WindowsVideoStoragePath;
            }

            //============================================================
            public static string GetPsqlConnectionString()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? LinuxPsqlConnectionString
                    : WindowsPsqlConnectionString;
            }

            //============================================================
            public static string GetMssqlConnectionString()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? LinuxMSSQLConnectionString
                    : WindowsMSSQLConnectionString;
            }
        }
    }
}