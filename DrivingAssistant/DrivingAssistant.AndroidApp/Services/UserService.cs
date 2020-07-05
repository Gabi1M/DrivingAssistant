using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class UserService : IDisposable
    {
        private readonly string _serverUri;

        //============================================================
        public UserService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<User>> GetAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/users"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<List<User>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/users"))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(user));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<long>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/users?id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public void Dispose()
        {

        }
    }
}