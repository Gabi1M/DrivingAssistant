using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("images")]
        public async Task<IActionResult> GetImagesAsync()
        {
            try
            {
                Logger.Log("Received GET images from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                return Ok((await mediaService.GetAsync()).Where(x => x.Type == MediaType.Image));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route("videos")]
        public async Task<IActionResult> GetVideosAsync()
        {
            try
            {
                Logger.Log("Received GET videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                return Ok((await mediaService.GetAsync()).Where(x => x.Type == MediaType.Video));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route("media_download")]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                Logger.Log("Received GET images_download from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var media = (await mediaService.GetAsync()).First(x => x.Id == id);
                return File(System.IO.File.Open(media.Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("images_base64")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostImageBase64Async()
        {
            try
            {
                Logger.Log("Received POST images_base64 from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var filepath = await Utils.SaveImageBase64ToFile(Request.Body);
                var media = new Media(MediaType.Image, filepath, Request.HttpContext.Connection.RemoteIpAddress.ToString(), DateTime.Now);
                return Ok(await mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("image_stream")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostImageStreamAsync()
        {
            try
            {
                Logger.Log("Received POST images_stream from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var filepath = await Utils.SaveImageStreamToFileAsync(Request.Body);
                var media = new Media(MediaType.Image, filepath, Request.HttpContext.Connection.RemoteIpAddress.ToString(), DateTime.Now);
                return Ok(await mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("video_stream")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostVideoStreamAsync()
        {
            try
            {
                Logger.Log("Received POST videos_stream from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var filepath = await Utils.SaveVideoStreamToFileAsync(Request.Body);
                var media = new Media(MediaType.Video, filepath, Request.HttpContext.Connection.RemoteIpAddress.ToString(), DateTime.Now);
                return Ok(await mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("images")]
        public async Task<IActionResult> UpdateImagesAsync()
        {
            try
            {
                Logger.Log("Received PUT images from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var media = JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
                await mediaService.UpdateAsync(media);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("videos")]
        public async Task<IActionResult> UpdateVideoAsync()
        {
            try
            {
                Logger.Log("Received PUT videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var media = JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
                await mediaService.UpdateAsync(media);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route("images")]
        public async Task<IActionResult> DeleteImageAsync()
        {
            try
            {
                Logger.Log("Received DELETE images from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                await mediaService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route("videos")]
        public async Task<IActionResult> DeleteVideosAsync()
        {
            try
            {
                Logger.Log("Received DELETE videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                await mediaService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("process_media")]
        public async Task<IActionResult> ProcessMedia()
        {
            try
            {
                Logger.Log("Received POST process_media from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var mediaService = new MediaService(Constants.ServerConstants.GetConnectionString());
                var media = (await mediaService.GetAsync()).First(x => x.Id == id);
                Media processedMedia;
                if (media.Type == MediaType.Image)
                {
                    var processedFilename = ImageProcessor.ProcessImage(media.Filepath);
                    processedMedia = new Media(MediaType.Image, processedFilename, media.Source, DateTime.Now);
                }
                else
                {
                    var processedFilename = ImageProcessor.ProcessVideo(media.Filepath);
                    processedMedia = new Media(MediaType.Video, processedFilename, media.Source, DateTime.Now);
                }

                processedMedia.Id = await mediaService.SetAsync(processedMedia);
                media.ProcessedId = processedMedia.Id;
                await mediaService.UpdateAsync(media);

                return Ok(processedMedia.Id);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }
    }
}
