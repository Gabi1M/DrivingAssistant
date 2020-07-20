﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {
        private MediaService _mediaService;

        //============================================================
        [HttpGet]
        [Route("media")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET media from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var medias = (await _mediaService.GetAsync()).Where(x => x.UserId == userId);
                return Ok(JsonConvert.SerializeObject(medias, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received GET media_download from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var media = (await _mediaService.GetAsync()).First(x => x.Id == id);
                return File(System.IO.File.Open(media.Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received POST images_base64 from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var filepath = await Utils.SaveImageBase64ToFile(Request.Body);
                var media = new Media(MediaType.Image, filepath,
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(), string.Empty, DateTime.Now, default,
                    default, default, userId);
                return Ok(await _mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received POST images_stream from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var description = Request.Query["Description"].First();
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var filepath = await Utils.SaveImageStreamToFileAsync(Request.Body);
                var media = new Media(MediaType.Image, filepath,
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(), description, DateTime.Now, default,
                    default, default, userId);
                return Ok(await _mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received POST videos_stream from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var description = Request.Query["Description"].First();
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var filepath = await Utils.SaveVideoStreamToFileAsync(Request.Body);
                var media = new Media(MediaType.Video, filepath,
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(), description, DateTime.Now, default,
                    default, default, userId);
                return Ok(await _mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("media")]
        public async Task<IActionResult> UpdateAsync()
        {
            try
            {
                Logger.Log(
                    "Received PUT media from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                using var streamReader = new StreamReader(Request.Body);
                var media = JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
                await _mediaService.UpdateAsync(media);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route("media")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE media from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var media = await _mediaService.GetByIdAsync(id);
                await _mediaService.DeleteAsync(media);
                if ((await _mediaService.GetAsync()).Any(x => x.ProcessedId == media.Id))
                {
                    var originalMedia = (await _mediaService.GetAsync()).First(x => x.ProcessedId == media.Id);
                    originalMedia.ProcessedId = default;
                    await _mediaService.UpdateAsync(originalMedia);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received POST process_media from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var media = await _mediaService.GetByIdAsync(id);
                var imageProcessor = new ImageProcessor(ImageProcessorParameters.Default());
                Media processedMedia;
                if (media.Type == MediaType.Image)
                {
                    var processedFilename = imageProcessor.ProcessImage(media.Filepath);
                    processedMedia = new Media(MediaType.Image, processedFilename, media.Source, media.Description,
                        DateTime.Now, default, default, media.SessionId, media.UserId);
                }
                else
                {
                    var processedFilename = imageProcessor.ProcessVideo(media.Filepath, 10);
                    processedMedia = new Media(MediaType.Video, processedFilename, media.Source, media.Description,
                        DateTime.Now, default, default, media.SessionId, media.UserId);
                }

                processedMedia.Id = await _mediaService.SetAsync(processedMedia);
                media.ProcessedId = processedMedia.Id;
                await _mediaService.UpdateAsync(media);

                return Ok(processedMedia.Id);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }
    }
}
