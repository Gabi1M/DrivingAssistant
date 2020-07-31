using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.ImageProcessing;
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
        private static readonly ISessionService _sessionService = new MssqlSessionService();
        private static readonly IVideoService VideoService = new MssqlVideoService();

        //============================================================
        [HttpGet]
        [Route(Endpoints.SessionEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
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
        [HttpGet]
        [Route(Endpoints.SessionEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var session = await _sessionService.GetById(id);
                return Ok(JsonConvert.SerializeObject(session, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.SessionEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var sessions = await _sessionService.GetByUser(userId);
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
        [Route(Endpoints.SessionEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
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
        [HttpDelete]
        [Route(Endpoints.SessionEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var session = await _sessionService.GetById(id);
                foreach (var video in await VideoService.GetBySession(session.Id))
                {
                    await VideoService.DeleteAsync(video);
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
        [Route(Endpoints.SessionEndpoints.Submit)]
        public IActionResult ProcessSession()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                new Thread(async () =>
                {
                    using var videoService = new MssqlVideoService();
                    using var reportService = new MssqlReportService();
                    var session = await _sessionService.GetById(id);
                    session.Status = SessionStatus.Processing;
                    await _sessionService.SetAsync(session);
                    var linkedVideos = await videoService.GetBySession(session.Id);
                    var imageProcessor = new ImageProcessor(Parameters.Default());
                    foreach (var video in linkedVideos.Where(x => !x.IsProcessed()))
                    {
                        var processedFilename = imageProcessor.ProcessVideo(video.Filepath, 10, out var result);
                        var processedVideo = new Video
                        {
                            Filepath = processedFilename,
                            Source = video.Source,
                            Description = video.Description,
                            DateAdded = DateTime.Now,
                            Id = -1,
                            ProcessedId = -1,
                            SessionId = video.SessionId,
                        };

                        var report = Report.FromVideoReport(result, video.Id);
                        processedVideo.Id = await videoService.SetAsync(processedVideo);
                        report.Id = await reportService.SetAsync(report);
                        video.ProcessedId = processedVideo.Id;
                        await videoService.SetAsync(video);
                    }

                    session.Status = SessionStatus.Processed;
                    await _sessionService.SetAsync(session);
                }).Start();
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
