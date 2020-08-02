using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class ThumbnailService
    {
        private static readonly string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<Thumbnail>> GetAllAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Thumbnail>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Thumbnail> GetByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Thumbnail>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Thumbnail> GetByVideoAsync(long videoId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.GetByVideoId + "?VideoId=" + videoId))
            {
                Method = "GET"
            };

            var response = (await request.GetResponseAsync().ConfigureAwait(false)) as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Thumbnail>((await streamReader.ReadToEndAsync().ConfigureAwait(false)));
        }

        //============================================================
        public async Task<Bitmap> DownloadAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.Download + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = (await request.GetResponseAsync().ConfigureAwait(false)) as HttpWebResponse;
            return await BitmapFactory.DecodeStreamAsync(response?.GetResponseStream()!).ConfigureAwait(false);
        }

        //============================================================
        public async Task<long> SetAsync(Thumbnail thumbnail)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.AddOrUpdate))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(thumbnail));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.ThumbnailEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}