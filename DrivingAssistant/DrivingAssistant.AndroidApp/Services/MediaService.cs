using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Uri = System.Uri;

namespace DrivingAssistant.AndroidApp.Services
{
    public class MediaService
    {
        private readonly string _serverUri;

        //============================================================
        public MediaService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<Media>> GetMediaAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/media?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<ICollection<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetImageBase64Async(byte[] base64Bytes, long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images_base64?UserId=" + userId))
            {
                Method = "POST"
            };

            var base64String = Convert.ToBase64String(base64Bytes);
            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(base64String);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetMediaStreamAsync(Stream mediaStream, MediaType type, long userId, string description)
        {
            var request = type == MediaType.Image
                ? new HttpWebRequest(new Uri(_serverUri + "/image_stream?UserId=" + userId + "&Description=" + description))
                : new HttpWebRequest(new Uri(_serverUri + "/video_stream?UserId=" + userId + "&Description=" + description));
            request.Method = "POST";

            await using var requestStream = await request.GetRequestStreamAsync();
            await mediaStream.CopyToAsync(requestStream);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task UpdateMediaAsync(Media media)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/media"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(media));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteMediaAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/media?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task<Bitmap> DownloadImageAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/media_download?id=" + id))
            {
                Method = "GET"
            };

            var response = request.GetResponse() as HttpWebResponse;
            return await BitmapFactory.DecodeStreamAsync(response?.GetResponseStream());
        }
    }
}