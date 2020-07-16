using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class SessionService
    {
        private readonly string _serverUri;

        //============================================================
        public SessionService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<Session>> GetAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/sessions"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<ICollection<Session>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(Session session)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/sessions"))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(session));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task UpdateAsync(Session session)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/sessions"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(session));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task SubmitAsync(Session session)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/process_session?id=" + session.Id))
            {
                Method = "GET"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/sessions?id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}