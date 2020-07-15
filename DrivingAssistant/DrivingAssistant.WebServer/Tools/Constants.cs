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
            private const string LinuxPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=pi;Password=1234";
            private const string LinuxMSSQLConnectionString = @"";

            private const string WindowsImageStoragePath = @"E:\CloudStorage\Images";
            private const string WindowsVideoStoragePath = @"E:\CloudStorage\Videos";
            private const string WindowsPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=postgres;Password=1234";
            private const string WindowsMSSQLConnectionString = @"Data Source=DESKTOP-KLAJVKV;Initial Catalog=DrivingAssistant;Persist Security Info=True;User ID=sa;Password=pxd";

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

        //============================================================
        public static class DatabaseConstants
        {
            public const string GetMediaCommand = @"select * from media;";
            public const string GetMediaByIdCommand = @"select * from media where id = @id;";
            public const string AddMediaCommand = @"insert into media(processed_id, session_id, user_id, type, filepath, source, description, dateadded) values (@processed_id, @session_id, @user_id, @type, @filepath, @source, @description, @dateadded) returning id;";
            public const string UpdateMediaCommand = @"update media set processed_id = @processed_id, session_id = @session_id, user_id = @user_id, type = @type, filepath = @filepath, source = @source, description = @description, dateadded = @dateadded where id = @id";
            public const string DeleteMediaCommand = @"delete from media where id = @id";

            public const string GetUsersCommand = @"select * from users;";
            public const string GetUserByIdCommand = @"select * from users where id = @id";
            public const string AddUserCommand = @"insert into users(username, password, firstname, lastname, role, joindate) values (@username, @password, @firstname, @lastname, @role, @joindate) returning id;";
            public const string UpdateUserCommand = @"update users set username = @username, password = @password, firstname = @firstname, lastname = @lastname, role = @role, joindate = @joindate where id = @id";
            public const string DeleteUserCommand = @"delete from users where id = @id";

            public const string GetSessionsCommand = @"select * from sessions";
            public const string GetSessionByIdCommand = @"select * from sessions where id = @id";
            public const string AddSessionCommand = @"insert into sessions(user_id, description, startdatetime, enddatetime, startx, starty, endx, endy) values (@user_id, @description, @startdatetime, @enddatetime, @startx, @starty, @endx, @endy) returning id;";
            public const string UpdateSessionCommand = @"update sessions set user_id = @user_id, description = @description, startdatetime = @startdatetime, enddatetime = @enddatetime, startx = @startx, starty = @starty, endx = @endx, endy = @endy where id = @id";
            public const string DeleteSessionCommand = @"delete from sessions where id = @id";
        }
    }
}