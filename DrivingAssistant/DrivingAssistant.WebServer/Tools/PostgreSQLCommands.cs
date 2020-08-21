namespace DrivingAssistant.WebServer.Tools
{
    public static class PostgreSQLCommands
    {
        public const string GetAllUsers = @"select * from users;";
        public const string GetUserById = @"select * from users where id = @id;";
        public const string SetUser = @"insert into users(username, password, first_name, last_name, email, join_date) values (@username, @password, @first_name, @last_name, @email, @join_date) returning id;";
        public const string UpdateUser = @"update users set username = @username, password = @password, first_name = @first_name, last_name = @last_name, email = @email, join_date = @join_date where id = @id;";
        public const string DeleteUser = @"delete from users where id = @id;";

        public const string GetAllUserSettings = @"select * from user_settings;";
        public const string GetUserSettingsById = @"select * from user_settings where id = @id;";
        public const string GetUserSettingsByUser = @"select * from user_settings s where s.user_id = @user_id;";
        public const string SetUserSettings = @"insert into user_settings(user_id, camera_session_id, camera_host, camera_username, camera_password) values (@user_id, @camera_session_id, @camera_host, @camera_username, @camera_password) returning id;";
        public const string UpdateUserSettings = @"update user_settings set user_id = @user_id, camera_session_id = @camera_session_id, camera_host = @camera_host, camera_username = @camera_username, camera_password = @camera_password where id = @id;";
        public const string DeleteUserSettings = @"delete from user_settings where id = @id;";

        public const string GetAllSessions = @"select * from driving_session;";
        public const string GetSessionById = @"select * from driving_session where id = @id;";
        public const string GetSessionsByUser = @"select * from driving_session where user_id = @user_id;";
        public const string SetSession = @"insert into driving_session(user_id, name, start_date_time, end_date_time, start_location, end_location, waypoints, status, date_added) values (@user_id, @name, @start_date_time, @end_date_time, @start_location, @end_location, @waypoints, @status, @date_added) returning id;";
        public const string UpdateSession = @"update driving_session set user_id = @user_id, name = @name, start_date_time = @start_date_time, end_date_time = @end_date_time, start_location = @start_location, end_location = @end_location, waypoints = @waypoints, status = @status, date_added = @date_added;";
        public const string DeleteSession = @"delete from driving_session where id = @id";

        public const string GetAllVideos = @"select * from video;";
        public const string GetVideoById = @"select * from video where id = @id;";
        public const string GetVideoByProcessedId = @"select * from video where processed_id = @processed_id;";
        public const string GetVideosBySession = @"select * from video where session_id = @session_id";
        public const string GetVideosByUser = @"select * from video v inner join driving_session s on v.session_id = s.id where s.user_id = @user_id";
        public const string SetVideo = @"insert into video(processed_id, session_id, filepath, source, description, date_added) values (@processed_id, @session_id, @filepath, @source, @description, @date_added) returning id;";
        public const string UpdateVideo = @"update video set processed_id = @processed_id, session_id = @session_id, filepath = @filepath, source = @source, description = @description, date_added = @date_added where id = @id;";
        public const string DeleteVideo = @"delete from video where id = @id;";

        public const string GetAllThumbnails = @"select * from thumbnail;";
        public const string GetThumbnailById = @"select * from thumbnail where id = @id;";
        public const string GetThumbnailByVideo = @"select * from thumbnail where video_id = @video_id;";
        public const string SetThumbnail = @"insert into thumbnail(video_id, filepath) values (@video_id, @filepath) returning id;";
        public const string UpdateThumbnail = @"update thumbnail set video_id = @video_id, filepath = @filepath where id = @id;";
        public const string DeleteThumbnail = @"delete from thumbnail where id = @id;";

        public const string GetAllReports = @"select * from report;";
        public const string GetReportById = @"select * from report where id = @id;";
        public const string GetReportBySession = @"select * from report r inner join video v on r.video_id = v.id inner join driving_session s on v.session_id = s.id where s.id = @id;";
        public const string GetReportByUser = @"select * from report r inner join video v on r.video_id = v.id inner join driving_session s on v.session_id = s.id where s.user_id = @user_id;";
        public const string GetReportByVideo = @"select * from report where video_id = @video_id;";
        public const string SetReport = @"insert into report(video_id, processed_frames, success_frames, fail_frames, success_rate, left_side_percent, right_side_percent, left_side_line_length, right_side_line_length, span_line_angle, span_line_length, left_side_line_number, right_side_line_number) values (@video_id, @processed_frames, @success_frames, @fail_frames, @success_rate, @left_side_percent, @right_side_percent, @left_side_line_length, @right_side_line_length, @span_line_angle, @span_line_length, @left_side_line_number, @right_side_line_number) returning id;";
        public const string UpdateReport = @"update report set video_id = @video_id, processed_frames = @processed_frames, success_frames = @success_frames, fail_frames = @fail_frames, success_rate = @success_rate, left_side_percent = @left_side_percent, right_side_percent = @right_side_percent, left_side_line_length = @left_side_line_length, right_side_line_length = @right_side_line_length, span_line_angle = @span_line_angle, span_line_length = @span_line_length, left_side_line_number = @left_side_line_number, right_side_line_number = @right_side_line_number where id = @id;";
        public const string DeleteReport = @"delete from report where id = @id;";
    }
}
