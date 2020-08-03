using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Newtonsoft.Json;
using Uri = System.Uri;

namespace DrivingAssistant.AndroidApp.Services
{
    public class VideoService
    {
        private static readonly string _serverUri = Constants.ServerUri;

        //============================================================
        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Video>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Video> GetVideoByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Video>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<Video> GetVideoByProcessedIdAsync(long processedId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.GetByProcessedId + "?ProcessedId=" + processedId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<Video>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Video>> GetVideoBySessionAsync(long sessionId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.GetBySessionId + "?SessionId=" + sessionId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Video>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<Video>> GetVideoByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<Video>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetVideoStreamAsync(Stream videoStream, string description)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.UploadVideoStream + "?Encoding=mp4&Description=" + description))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await videoStream.CopyToAsync(requestStream);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> UpdateVideoAsync(Video video)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.Update))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(video));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteVideoAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/" + Endpoints.VideoEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}