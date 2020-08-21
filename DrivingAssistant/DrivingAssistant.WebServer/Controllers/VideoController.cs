using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Processing;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class VideoController : ControllerBase
    {
        private static readonly IVideoService _videoService = IVideoService.CreateNew();
        private static readonly IUserSettingsService _userSettingsService = IUserSettingsService.CreateNew();
        private static readonly IThumbnailService _thumbnailService = IThumbnailService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var videos = (await _videoService.GetAsync());
                return Ok(JsonConvert.SerializeObject(videos, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var video = await _videoService.GetById(id);
                return Ok(JsonConvert.SerializeObject(video, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.GetByProcessedId)]
        public async Task<IActionResult> GetByProcessedIdAsync()
        {
            try
            {
                var processedId = Convert.ToInt64(Request.Query["ProcessedId"].First());
                var video = await _videoService.GetByProcessedId(processedId);
                return Ok(JsonConvert.SerializeObject(video, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.GetBySessionId)]
        public async Task<IActionResult> GetBySessionAsync()
        {
            try
            {
                var sessionId = Convert.ToInt64(Request.Query["SessionId"].First());
                var videos = await _videoService.GetBySession(sessionId);
                return Ok(JsonConvert.SerializeObject(videos, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var videos = await _videoService.GetByUser(userId);
                return Ok(JsonConvert.SerializeObject(videos, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.VideoEndpoints.Download)]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var video = await _videoService.GetById(id);
                return File(System.IO.File.Open(video.Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route(Endpoints.VideoEndpoints.UploadVideoStream)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostVideoStreamAsync()
        {
            try
            {
                var sessionId = -1L;
                if (Request.Query.ContainsKey("UserId"))
                {
                    var userSettings = await _userSettingsService.GetByUser(Convert.ToInt64(Request.Query["UserId"].First()));
                    if (Request.HttpContext.Connection.RemoteIpAddress.ToString() == userSettings.CameraHost &&
                        userSettings.CameraSessionId != -1)
                    {
                        sessionId = userSettings.CameraSessionId;
                    }
                }

                var encoding = Request.Query["Encoding"].First();
                var filepath = await Utils.SaveVideoStreamToFileAsync(Request.Body, encoding);
                if (encoding.ToLower() == "h264")
                {
                    var newFilepath = Common.ConvertH264ToMkv(filepath);
                    System.IO.File.Delete(filepath);
                    filepath = newFilepath;
                }
                var video = new VideoRecording
                {
                    Filepath = filepath,
                    Source = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = Request.Query.ContainsKey("Description") ? Request.Query["Description"].ToString() : string.Empty,
                    DateAdded = DateTime.Now,
                    Id = -1,
                    ProcessedId = -1,
                    SessionId = sessionId,
                };
                video.Id = await _videoService.SetAsync(video);

                var thumbnail = new Thumbnail
                {
                    Id = -1,
                    VideoId = video.Id,
                    Filepath = Common.ExtractThumbnail(video.Filepath)
                };
                await _thumbnailService.SetAsync(thumbnail);
                return Ok(video.Id);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route(Endpoints.VideoEndpoints.Update)]
        public async Task<IActionResult> UpdateAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var video = JsonConvert.DeserializeObject<VideoRecording>(await streamReader.ReadToEndAsync());
                return Ok(await _videoService.SetAsync(video));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.VideoEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var video = await _videoService.GetById(id);
                await _videoService.DeleteAsync(video);
                try
                {
                    var originalVideo = await _videoService.GetByProcessedId(video.Id);
                    originalVideo.ProcessedId = -1;
                    await _videoService.SetAsync(originalVideo);
                }
                catch (Exception)
                {
                    // ignored
                }

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
