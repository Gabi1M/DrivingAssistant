using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Processing;
using DrivingAssistant.WebServer.Processing.Algorithms;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class SessionController : ControllerBase
    {
        private static readonly ISessionService _sessionService = ISessionService.CreateNew();
        private static readonly IVideoService _videoService = IVideoService.CreateNew();
        private static readonly IReportService _reportService = IReportService.CreateNew();
        private static readonly IUserService _userService = IUserService.CreateNew();
        private static readonly IThumbnailService _thumbnailService = IThumbnailService.CreateNew();

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
                foreach (var video in await _videoService.GetBySession(session.Id))
                {
                    await _videoService.DeleteAsync(video);
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
                var id = Convert.ToInt64(Request.Query["Id"].Single());
                var type = Enum.Parse<ProcessingAlgorithmType>(Request.Query["Type"].Single());
                new Thread(async () =>
                {
                    var session = await _sessionService.GetById(id);
                    var user = await _userService.GetById(session.UserId);
                    session.Status = SessionStatus.Processing;
                    await _sessionService.SetAsync(session);
                    var linkedVideos = await _videoService.GetBySession(session.Id);
                    switch (type)
                    {
                        case ProcessingAlgorithmType.Lane_Departure_Warning:
                        {
                            var imageProcessor = new LaneDepartureWarningAlgorithm(LaneDepartureWarningAlgorithm.LaneDepartureWarningParameters.Default());
                            foreach (var video in linkedVideos.Where(x => !x.IsProcessed()))
                            {
                                var processedFilename = imageProcessor.ProcessVideo(video.Filepath, 10, out var result);
                                var processedVideo = new VideoRecording
                                {
                                    Filepath = processedFilename,
                                    Source = video.Source,
                                    Description = !string.IsNullOrEmpty(video.Description) ? video.Description + "_processed" : string.Empty,
                                    DateAdded = DateTime.Now,
                                    Id = -1,
                                    ProcessedId = -1,
                                    SessionId = video.SessionId,
                                };

                                var report = LaneDepartureWarningReport.FromVideoReport(result, video.Id);
                                processedVideo.Id = await _videoService.SetAsync(processedVideo);
                                report.Id = await _reportService.SetAsync(report);
                                video.ProcessedId = processedVideo.Id;
                                await _videoService.SetAsync(video);

                                var thumbnail = new Thumbnail
                                {
                                    Id = -1,
                                    VideoId = processedVideo.Id,
                                    Filepath = Common.ExtractThumbnail(processedVideo.Filepath)
                                };
                                await _thumbnailService.SetAsync(thumbnail);
                            }
                            break;
                        }
                    }

                    session.Status = SessionStatus.Processed;
                    await _sessionService.SetAsync(session);

                    await Utils.SendEmail(user.Email, session.Name + " Finished Processing", session.Name + " Finished Processing");

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
