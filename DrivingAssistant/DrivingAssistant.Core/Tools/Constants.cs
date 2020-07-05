namespace DrivingAssistant.Core.Tools
{
    public static class Constants
    {
        //============================================================
        public static class ServerConstants
        {
            public const string ImageStoragePath = "/mnt/hdd/CloudStorage/Images";
            public const string VideoStoragePath = "/mnt/hdd/CloudStorage/Videos";
            public const string ConnectionString = "Host=127.0.0.1;Port=5432;Database=cloud;Username=pi;Password=1234";
        }

        //============================================================
        public static class DatabaseConstants
        {
            public const string GetImagesCommand = @"select * from image;";
            public const string AddImageCommand = @"insert into image(filepath, width, height, format, source, datetime) values (@filepath, @width, @height, @format, @source, @datetime) returning id;";
            public const string DeleteImageCommand = @"delete from image where id = @id";

            public const string GetUsersCommand = @"select * from users;";
            public const string AddUserCommand = @"insert into users(username, password, firstname, lastname, datetime) values (@username, @password, @firstname, @lastname, @datetime) returning id;";
            public const string DeleteUserCommand = @"delete from users where id = @id";
        }
    }
}
