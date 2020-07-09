using PerpetualEngine.Storage;

namespace DrivingAssistant.AndroidApp.Tools
{
    public static class CacheManager
    {
        //============================================================
        public static void Set<T>(string key, T values)
        {
            var storage = SimpleStorage.EditGroup("settings");
            storage.Put(key, values);
        }

        //============================================================
        public static T Get<T>(string key)
        {
            var storage = SimpleStorage.EditGroup("settings");
            return storage.Get<T>(key) ?? default;
        }
    }
}