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
        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetAllVideosAsync()
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.GetAll))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<VideoRecording>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<VideoRecording> GetVideoByIdAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.GetById + "?Id=" + id))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<VideoRecording>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<VideoRecording> GetVideoByProcessedIdAsync(long processedId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.GetByProcessedId + "?ProcessedId=" + processedId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<VideoRecording>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetVideoBySessionAsync(long sessionId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.GetBySessionId + "?SessionId=" + sessionId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<VideoRecording>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<IEnumerable<VideoRecording>> GetVideoByUserAsync(long userId)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.GetByUserId + "?UserId=" + userId))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return JsonConvert.DeserializeObject<IEnumerable<VideoRecording>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetVideoStreamAsync(Stream videoStream, string description)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.UploadVideoStream + "?Encoding=mp4&Description=" + description))
            {
                Method = "POST",
                Timeout = 120000
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await videoStream.CopyToAsync(requestStream);
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> UpdateVideoAsync(VideoRecording videoRecording)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.Update))
            {
                Method = "PUT"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(videoRecording));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response?.GetResponseStream()!);
            return Convert.ToInt64(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteVideoAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(Constants.ServerUri + "/" + Endpoints.VideoEndpoints.Delete + "?Id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }
    }
}