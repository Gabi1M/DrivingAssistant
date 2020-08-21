using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class ThumbnailController : ControllerBase
    {
        private static readonly IThumbnailService _thumbnailService = IThumbnailService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.ThumbnailEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var thumbnails = await _thumbnailService.GetAsync();
                return Ok(JsonConvert.SerializeObject(thumbnails, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ThumbnailEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var thumbnail = await _thumbnailService.GetById(id);
                return Ok(JsonConvert.SerializeObject(thumbnail, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ThumbnailEndpoints.GetByVideoId)]
        public async Task<IActionResult> GetByVideoAsync()
        {
            try
            {
                var videoId = Convert.ToInt64(Request.Query["VideoId"].First());
                var thumbnail = await _thumbnailService.GetByVideo(videoId);
                return Ok(JsonConvert.SerializeObject(thumbnail, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ThumbnailEndpoints.Download)]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var thumbnail = await _thumbnailService.GetById(id);
                return File(System.IO.File.Open(thumbnail.Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route(Endpoints.ThumbnailEndpoints.AddOrUpdate)]
        public async Task<IActionResult> AddOrUpdate()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var thumbnail = JsonConvert.DeserializeObject<Thumbnail>(await streamReader.ReadToEndAsync());
                return Ok(await _thumbnailService.SetAsync(thumbnail));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.ThumbnailEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var thumbnail = await _thumbnailService.GetById(id);
                await _thumbnailService.DeleteAsync(thumbnail);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }
    }
}
