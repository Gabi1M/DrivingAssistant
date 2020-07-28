using System;
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
        private static readonly IMediaService _mediaService = new MssqlMediaService();
        private static readonly IUserSettingsService _userSettingsService = new MssqlUserSettingsService();

        //============================================================
        [HttpGet]
        [Route(Endpoints.MediaEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var medias = (await _mediaService.GetAsync());
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
        [Route(Endpoints.MediaEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var media = await _mediaService.GetById(id);
                return Ok(JsonConvert.SerializeObject(media, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.MediaEndpoints.GetByProcessedId)]
        public async Task<IActionResult> GetByProcessedIdAsync()
        {
            try
            {
                var processedId = Convert.ToInt64(Request.Query["ProcessedId"].First());
                var media = await _mediaService.GetByProcessedId(processedId);
                return Ok(JsonConvert.SerializeObject(media, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.MediaEndpoints.GetBySessionId)]
        public async Task<IActionResult> GetBySessionAsync()
        {
            try
            {
                var sessionId = Convert.ToInt64(Request.Query["SessionId"].First());
                var medias = await _mediaService.GetBySession(sessionId);
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
        [Route(Endpoints.MediaEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var medias = await _mediaService.GetByUser(userId);
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
        [Route(Endpoints.MediaEndpoints.Download)]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var media = await _mediaService.GetById(id);
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
        [Route(Endpoints.MediaEndpoints.UploadImageStream)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostImageStreamAsync()
        {
            try
            {
                var filepath = await Utils.SaveImageStreamToFileAsync(Request.Body);
                var media = new Media
                {
                    Type = MediaType.Image,
                    Filepath = filepath,
                    Source = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = string.Empty,
                    DateAdded = DateTime.Now,
                    Id = -1,
                    ProcessedId = -1,
                    SessionId = -1,
                };
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
        [Route(Endpoints.MediaEndpoints.UploadVideoStream)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostVideoStreamAsync()
        {
            try
            {
                var sessionId = -1L;
                if (Request.Query.ContainsKey("UserId"))
                {
                    var userSettings = await _userSettingsService.GetByUser(Convert.ToInt64(Request.Query["UserId"].First()));
                    if (Request.HttpContext.Connection.RemoteIpAddress.ToString() == userSettings.CameraIp &&
                        userSettings.CameraSessionId != -1)
                    {
                        sessionId = userSettings.CameraSessionId;
                    }
                }

                var encoding = Request.Query["Encoding"].First();
                var filepath = await Utils.SaveVideoStreamToFileAsync(Request.Body, encoding);
                var media = new Media
                {
                    Type = MediaType.Video,
                    Filepath = filepath,
                    Source = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = string.Empty,
                    DateAdded = DateTime.Now,
                    Id = -1,
                    ProcessedId = -1,
                    SessionId = sessionId,
                };
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
        [Route(Endpoints.MediaEndpoints.Update)]
        public async Task<IActionResult> UpdateAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var media = JsonConvert.DeserializeObject<Media>(await streamReader.ReadToEndAsync());
                return Ok(await _mediaService.SetAsync(media));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.MediaEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var media = await _mediaService.GetById(id);
                await _mediaService.DeleteAsync(media);
                try
                {
                    var originalMedia = await _mediaService.GetByProcessedId(media.Id);
                    originalMedia.ProcessedId = -1;
                    await _mediaService.SetAsync(originalMedia);
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
