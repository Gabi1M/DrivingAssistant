using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class SessionService
    {
        //============================================================
        public async Task<IEnumerable<DrivingSession>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<DrivingSession>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<DrivingSession> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<DrivingSession>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<DrivingSession>> GetByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<DrivingSession>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(DrivingSession drivingSession)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(drivingSession));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task SubmitAsync(long id, ProcessingAlgorithmType algorithmType)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.SessionEndpoints.Submit + "?Id=" + id + "&Type=" + algorithmType))
            {
                Method = "GET"
            };

            await request.GetResponseAsync();
        }
    }
}