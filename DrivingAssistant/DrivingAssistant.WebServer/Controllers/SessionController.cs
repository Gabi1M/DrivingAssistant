using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Processing;
using DrivingAssistant.WebServer.Services.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class SessionController : ControllerBase
    {
        private static readonly IDrivingSessionService _drivingSessionService = IDrivingSessionService.CreateNew();
        private static readonly IVideoService _videoService = IVideoService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.DrivingSessionEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var sessions = await _drivingSessionService.GetAsync();
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
        [Route(Endpoints.DrivingSessionEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var session = await _drivingSessionService.GetById(id);
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
        [Route(Endpoints.DrivingSessionEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var sessions = await _drivingSessionService.GetByUser(userId);
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
        [Route(Endpoints.DrivingSessionEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var session = JsonConvert.DeserializeObject<DrivingSession>(await streamReader.ReadToEndAsync());
                return Ok(await _drivingSessionService.SetAsync(session));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.DrivingSessionEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var session = await _drivingSessionService.GetById(id);
                foreach (var video in await _videoService.GetBySession(session.Id))
                {
                    await _videoService.DeleteAsync(video);
                }
                await _drivingSessionService.DeleteAsync(session);
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
        [Route(Endpoints.DrivingSessionEndpoints.Submit)]
        public IActionResult ProcessSession()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].Single());
                var type = Enum.Parse<ProcessingAlgorithmType>(Request.Query["Type"].Single());
                ProcessThread.ProcessSession(id, type);
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
