﻿using System;
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
    public class UserService
    {
        private static readonly string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<User>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<User> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(User user)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(user));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.UserEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}