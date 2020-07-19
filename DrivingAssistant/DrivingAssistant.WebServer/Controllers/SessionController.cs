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
    public class SessionController : ControllerBase
    {
        private SessionService _sessionService;

        //============================================================
        [HttpGet]
        [Route("sessions")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _sessionService = SessionService.NewInstance(typeof(MssqlSessionService));
                var sessions = await _sessionService.GetAsync();
                return Ok(JsonConvert.SerializeObject(sessions, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("sessions")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log(
                    "Received POST sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = SessionService.NewInstance(typeof(MssqlSessionService));
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                return Ok(await _sessionService.SetAsync(session));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("sessions")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log(
                    "Received PUT sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = SessionService.NewInstance(typeof(MssqlSessionService));
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                await _sessionService.UpdateAsync(session);
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
        [Route("sessions")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _sessionService = SessionService.NewInstance(typeof(MssqlSessionService));
                var mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var session = await _sessionService.GetByIdAsync(id);
                foreach (var media in (await mediaService.GetAsync()).Where(x => x.SessionId == session.Id))
                {
                    media.SessionId = default;
                    await mediaService.UpdateAsync(media);
                }
                await _sessionService.DeleteAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route("process_session")]
        public async Task<IActionResult> ProcessSession()
        {
            try
            {
                Logger.Log(
                    "Received POST process_session from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _sessionService = SessionService.NewInstance(typeof(MssqlSessionService));
                var mediaService = MediaService.NewInstance(typeof(MssqlMediaService));
                var session = await _sessionService.GetByIdAsync(id);
                var linkedMedia = (await mediaService.GetAsync()).Where(x => x.SessionId == session.Id);
                foreach (var media in linkedMedia.Where(x => !x.IsProcessed()))
                {
                    Media processedMedia;
                    if (media.Type == MediaType.Image)
                    {
                        var processedFilename = ImageProcessor.ProcessImage(media.Filepath);
                        processedMedia = new Media(MediaType.Image, processedFilename, media.Source, media.Description,
                            DateTime.Now, default, default, media.SessionId, media.UserId);
                    }
                    else
                    {
                        var processedFilename = ImageProcessor.ProcessVideo(media.Filepath, 10);
                        processedMedia = new Media(MediaType.Video, processedFilename, media.Source, media.Description,
                            DateTime.Now, default, default, media.SessionId, media.UserId);
                    }

                    processedMedia.Id = await mediaService.SetAsync(processedMedia);
                    media.ProcessedId = processedMedia.Id;
                    await mediaService.UpdateAsync(media);
                }

                session.Processed = true;
                await _sessionService.UpdateAsync(session);
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
