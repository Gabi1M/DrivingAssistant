using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class ReportService
    {
        private readonly string _serverUri;

        //============================================================
        public ReportService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<Report>> GetAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/reports"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<ICollection<Report>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(Report report)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/reports"))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(report));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task UpdateAsync(Report report)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/reports"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(report));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/reports?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}