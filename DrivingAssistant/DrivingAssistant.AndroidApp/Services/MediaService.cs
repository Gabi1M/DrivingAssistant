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
        public async Task<ICollection<Media>> GetImagesAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream());
            return JsonConvert.DeserializeObject<ICollection<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<ICollection<Media>> GetVideosAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/videos"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream());
            return JsonConvert.DeserializeObject<ICollection<Media>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetImageBase64Async(byte[] base64Bytes)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images_base64"))
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
        public async Task<long> SetMediaStreamAsync(Stream mediaStream, MediaType type)
        {
            var request = type == MediaType.Image
                ? (HttpWebRequest) new HttpWebRequest(new Uri(_serverUri + "/image_stream"))
                : (HttpWebRequest) new HttpWebRequest(new Uri(_serverUri + "/video_stream"));
            request.Method = "POST";

            await using var requestStream = await request.GetRequestStreamAsync();
            await mediaStream.CopyToAsync(requestStream);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream());
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task UpdateImageAsync(Media image)
        {
            if (image.Type != MediaType.Image)
            {
                throw new Exception("Media is not of type Image");
            }

            var request = new HttpWebRequest(new Uri(_serverUri + "/images"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(image));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task UpdateVideoAsync(Media video)
        {
            if (video.Type != MediaType.Video)
            {
                throw new Exception("Media is not of type Video");
            }

            var request = new HttpWebRequest(new Uri(_serverUri + "/videos"))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(video));
            await streamWriter.FlushAsync();
            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteImageAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images?id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task DeleteVideoAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/videos?id=" + id))
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