namespace DrivingAssistant.Core.Tools
{
    public static class Endpoints
    {
        //============================================================
        public static class MediaEndpoints
        {
            public const string GetAll = "media_all";
            public const string GetById = "media_id";
            public const string GetByProcessedId = "media_processed_id";
            public const string GetBySessionId = "media_session_id";
            public const string GetByUserId = "media_user_id";
            public const string Download = "media_download";
            public const string UploadImageBase64 = "media_image_base64";
            public const string UploadImageStream = "media_image_stream";
            public const string UploadVideoStream = "media_video_stream";
            public const string Update = "media_update";
            public const string Delete = "media_delete";

        }

        //============================================================
        public static class ReportEndpoints
        {
            public const string GetAll = "reports_all";
            public const string GetById = "reports_id";
            public const string GetByMediaId = "reports_media_id";
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
