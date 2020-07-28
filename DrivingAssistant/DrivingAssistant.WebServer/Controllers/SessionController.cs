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
        private static readonly IMediaService _mediaService = new MssqlMediaService();

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
                foreach (var media in await _mediaService.GetBySession(session.Id))
                {
                    await _mediaService.DeleteAsync(media);
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
                    using var mediaService = new MssqlMediaService();
                    using var reportService = new MssqlReportService();
                    var session = await _sessionService.GetById(id);
                    var linkedMedia = await mediaService.GetBySession(session.Id);
                    var imageProcessor = new ImageProcessor(Parameters.Default());
                    foreach (var media in linkedMedia.Where(x => !x.IsProcessed()))
                    {
                        Media processedMedia;
                        Report report;
                        if (media.Type == MediaType.Image)
                        {
                            var processedFilename = imageProcessor.ProcessImage(media.Filepath, out var result);
                            if (processedFilename == null)
                            {
                                continue;
                            }
                            processedMedia = new Media
                            {
                                Type = MediaType.Image,
                                Filepath = processedFilename,
                                Source = media.Source,
                                Description = media.Description,
                                DateAdded = DateTime.Now,
                                Id = -1,
                                ProcessedId = -1,
                                SessionId = media.SessionId,
                            };
                            report = Report.FromImageReport(result, media.Id);
                        }
                        else
                        {
                            var processedFilename = imageProcessor.ProcessVideo(media.Filepath, 10, out var result);
                            processedMedia = new Media
                            {
                                Type = MediaType.Video,
                                Filepath = processedFilename,
                                Source = media.Source,
                                Description = media.Description,
                                DateAdded = DateTime.Now,
                                Id = -1,
                                ProcessedId = -1,
                                SessionId = media.SessionId,
                            };
                            report = Report.FromVideoReport(result, media.Id);
                        }

                        processedMedia.Id = await mediaService.SetAsync(processedMedia);
                        report.Id = await reportService.SetAsync(report);
                        media.ProcessedId = processedMedia.Id;
                        await mediaService.SetAsync(media);
                    }

                    session.Processed = true;
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
