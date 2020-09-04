using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Environment = Android.OS.Environment;

namespace DrivingAssistant.AndroidApp.Services
{
    public class ReportService
    {
        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<LaneDepartureWarningReport>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<LaneDepartureWarningReport> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<LaneDepartureWarningReport>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<LaneDepartureWarningReport> GetByVideoAsync(long videoId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.GetByVideoId + "?VideoId=" + videoId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<LaneDepartureWarningReport>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetBySessionAsync(long sessionId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.GetBySessionId + "?SessionId=" + sessionId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<LaneDepartureWarningReport>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<LaneDepartureWarningReport>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(LaneDepartureWarningReport laneDepartureWarningReport)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(laneDepartureWarningReport));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task<string> DownloadReport(long videoId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.ReportEndpoints.DownloadReport + "?VideoId=" + videoId))
            {
                Method = "GET"
            };

            if ((await Permissions.CheckStatusAsync<Permissions.StorageWrite>()) != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if ((await Permissions.CheckStatusAsync<Permissions.StorageRead>()) != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            using var response = await request.GetResponseAsync() as HttpWebResponse;
            await using var responseStream = response?.GetResponseStream();
            var filename = Path.Combine(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).Path, videoId + ".html");
            await using var file = File.Create(filename);
            await responseStream.CopyToAsync(file);
            return Path.GetFileName(filename);
        }
    }
}