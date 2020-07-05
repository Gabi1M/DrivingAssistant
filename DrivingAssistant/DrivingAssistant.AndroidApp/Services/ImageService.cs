
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Services
{
    public class ImageService : IDisposable
    {
        private readonly string _serverUri;

        //============================================================
        public ImageService(string serverUri)
        {
            _serverUri = serverUri;
        }

        //============================================================
        public async Task<ICollection<Image>> GetAsync()
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images"))
            {
                Method = "GET"
            };

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<List<Image>>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task<long> SetAsync(Image image)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images"))
            {
                Method = "POST"
            };

            await using var requestStream = await request.GetRequestStreamAsync();
            await using var streamWriter = new StreamWriter(requestStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(image));
            await streamWriter.FlushAsync();
            var response = await request.GetResponseAsync() as HttpWebResponse;
            using var streamReader = new StreamReader(response.GetResponseStream());
            return JsonConvert.DeserializeObject<long>(await streamReader.ReadToEndAsync());
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images?id=" + id))
            {
                Method = "DELETE"
            };

            await request.GetResponseAsync();
        }

        //============================================================
        public async Task<Bitmap> DownloadImageAsync(long id)
        {
            var request = new HttpWebRequest(new Uri(_serverUri + "/images_download?id=" + id))
            {
                Method = "GET"
            };

            var response = request.GetResponse() as HttpWebResponse;
            return await BitmapFactory.DecodeStreamAsync(response.GetResponseStream());
        }

        //============================================================
        public void Dispose()
        {

        }
    }
}