using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class UserSettingsService
    {
        private readonly string _serverUri;

        //============================================================
        public UserSettingsService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<UserSettings>> GetAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/user_settings"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<List<UserSettings>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(UserSettings userSettings)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/user_settings"))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(userSettings));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task UpdateAsync(UserSettings userSettings)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/user_settings"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(userSettings));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/user_settings?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}