using PerpetualEngine.Storage;

namespace DrivingAssistant.AndroidApp.Tools
{
    public static class CacheManager
    {
        //============================================================
        public static void Set(string key, string value)
        {
            var storage = SimpleStorage.EditGroup("settings");
            storage.Put(key, value);
        }

        //============================================================
        public static string Get(string key)
        {
            var storage = SimpleStorage.EditGroup("settings");
            return storage.Get(key);
        }
    }
}