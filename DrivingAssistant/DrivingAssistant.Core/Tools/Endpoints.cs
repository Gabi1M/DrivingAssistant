namespace DrivingAssistant.Core.Tools
{
    public static class Endpoints
    {
        //============================================================
        public static class VideoEndpoints
        {
            public const string GetAll = "video_all";
            public const string GetById = "video_id";
            public const string GetByProcessedId = "video_processed_id";
            public const string GetBySessionId = "video_session_id";
            public const string GetByUserId = "video_user_id";
            public const string Download = "video_download";
            public const string UploadVideoStream = "upload_video_stream";
            public const string Update = "video_update";
            public const string Delete = "video_delete";

        }

        //============================================================
        public static class ReportEndpoints
        {
            public const string GetAll = "reports_all";
            public const string GetById = "reports_id";
            public const string GetByVideoId = "reports_video_id";
            public const string GetBySessionId = "reports_session_id";
            public const string GetByUserId = "reports_user_id";
            public const string AddOrUpdate = "reports_set";
            public const string Delete = "reports_delete";
        }

        //============================================================
        public static class SessionEndpoints
        {
            public const string GetAll = "sessions_all";
            public const string GetById = "sessions_id";
            public const string GetByUserId = "sessions_user_id";
            public const string AddOrUpdate = "sessions_set";
            public const string Delete = "sessions_delete";
            public const string Submit = "sessions_submit";
        }

        //============================================================
        public static class UserEndpoints
        {
            public const string GetAll = "users_all";
            public const string GetById = "users_id";
            public const string AddOrUpdate = "users_set";
            public const string Delete = "users_delete";
        }

        //============================================================
        public static class UserSettingsEndpoints
        {
            public const string GetAll = "user_settings_all";
            public const string GetById = "user_settings_id";
            public const string GetByUserId = "user_settings_user_id";
            public const string AddOrUpdate = "user_settings_set";
            public const string Delete = "user_settings_delete";
        }
    }
}
