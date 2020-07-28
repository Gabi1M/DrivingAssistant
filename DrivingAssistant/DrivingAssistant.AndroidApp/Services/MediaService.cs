using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;
using Uri = System.Uri;

namespace DrivingAssistant.AndroidApp.Services
{
    public class MediaService
    {
        private static readonly string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<Media>> GetAllMediaAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Media> GetMediaByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Media> GetMediaByProcessedIdAsyn(long processedId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.GetByProcessedId + "?ProcessedId=" + processedId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Media>> GetMediaBySessionAsync(long sessionId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.GetBySessionId + "?SessionId=" + sessionId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Media>> GetMediaByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Bitmap> DownloadImageAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.Download + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = request.GetResponse() as HttpWebResponse;
            return await BitmapFactory.DecodeStreamAsync(response?.GetResponseStream());
        }

        //============================================================
        public Bitmap DownloadImage(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.Download + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = request.GetResponse() as HttpWebResponse;
            return BitmapFactory.DecodeStream(response?.GetResponseStream());
        }

        //============================================================
        public async Task<long> SetMediaStreamAsync(Stream mediaStream, MediaType type)
        {
            var request = type == MediaType.Image
                ? new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.UploadImageStream + "?Encoding=jpg"))
                : new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.UploadVideoStream + ">Encoding= mp4"));
            request.Method = "POST";

            await using var requestStream = await request.GetRequestStreamAsync();
            await mediaStream.CopyToAsync(requestStream);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> UpdateMediaAsync(Media media)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.Update))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(media));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteMediaAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.MediaEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}