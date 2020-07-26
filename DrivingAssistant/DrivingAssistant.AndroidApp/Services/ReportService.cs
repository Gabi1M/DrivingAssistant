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
    public class ReportService
    {
        private const string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Report>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Report> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Report>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Report> GetByMediaAsync(long mediaId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.GetByMediaId + "?MediaId=" + mediaId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Report>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Report>> GetBySessionAsync(long sessionId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.GetBySessionId + "?SessionId=" + sessionId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Report>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Report>> GetByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Report>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(Report report)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(report));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ReportEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}