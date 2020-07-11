using System.Runtime.InteropServices;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Constants
    {
        //============================================================
        public static class ServerConstants
        {
            private const string LinuxImageStoragePath = @"/mnt/hdd/CloudStorage/Images";
            private const string LinuxVideoStoragePath = @"/mnt/hdd/CloudStorage/Videos";

            private const string LinuxConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=pi;Password=1234";

            private const string WindowsImageStoragePath = @"E:\CloudStorage\Images";
            private const string WindowsVideoStoragePath = @"E:\CloudStorage\Videos";
            private const string WindowsConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=postgres;Password=1234";

            //============================================================
            public static string GetImageStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxImageStoragePath : WindowsImageStoragePath;
            }

            //============================================================
            public static string GetVideoStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxVideoStoragePath : WindowsVideoStoragePath;
            }

            //============================================================
            public static string GetConnectionString()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxConnectionString : WindowsConnectionString;
            }
        }

        //============================================================
        public static class DatabaseConstants
        {
            public const string GetMediaCommand = @"select * from media;";
            public const string AddMediaCommand = @"insert into media(processed_id, session_id, type, filepath, source, datetime) values (@processed_id, @session_id, @type, @filepath, @source, @datetime) returning id;";
            public const string UpdateMediaCommand = @"update media set processed_id = @processed_id, session_id = @session_id, type = @type, filepath = @filepath, source = @source, datetime = @datetime where id = @id";
            public const string DeleteMediaCommand = @"delete from media where id = @id";

            public const string GetUsersCommand = @"select * from users;";
            public const string AddUserCommand = @"insert into users(username, password, firstname, lastname, datetime) values (@username, @password, @firstname, @lastname, @datetime) returning id;";
            public const string UpdateUserCommand = @"update users set username = @username, password = @password, firstname = @firstname, lastname = @lastname, datetime = @datetime where id = @id";
            public const string DeleteUserCommand = @"delete from users where id = @id";

            public const string GetSessionsCommand = @"select * from sessions";
            public const string AddSessionCommand = @"insert into sessions(startdatetime, enddatetime, startx, starty, endx, endy) values (@startdatetime, @enddatetime, @startx, @starty, @endx, @endy) returning id;";
            public const string UpdateSessionCommand = @"update sessions set startdatetime = @startdatetime, enddatetime = @enddatetime, startx = @startx, starty = @starty, endx = @endx, endy = @endy where id = @id";
            public const string DeleteSessionCommand = @"delete from sessions where id = @id";
        }
    }
}