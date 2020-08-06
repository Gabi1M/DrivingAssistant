using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class UserSettingsService
    {
        private static readonly string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<UserSettings>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<UserSettings>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<UserSettings> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<UserSettings>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<UserSettings> GetByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<UserSettings>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(UserSettings userSettings)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(userSettings));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task StartRecordingAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.StartRecording + "?UserId=" + userId))
            {
                Method = "GET"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task StopRecordingAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.StopRecording + "?UserId=" + userId))
            {
                Method = "GET"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task<string> GetRecordingStatus(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserSettingsEndpoints.RecordingStatus + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return await streamReader.ReadToEndAsync();
        }
    }
}