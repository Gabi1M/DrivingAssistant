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
    public class RemoteCameraService
    {
        //============================================================
        public async Task<IEnumerable<RemoteCamera>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.RemoteCameraEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<RemoteCamera>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<RemoteCamera> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.RemoteCameraEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<RemoteCamera>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<RemoteCamera> GetByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.RemoteCameraEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<RemoteCamera>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(RemoteCamera remoteCamera)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.RemoteCameraEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(remoteCamera));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.RemoteCameraEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}