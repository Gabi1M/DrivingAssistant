using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private static readonly IReportService _reportService = IReportService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.ReportEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var reports = await _reportService.GetAsync();
                return Ok(JsonConvert.SerializeObject(reports, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ReportEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var report = await _reportService.GetById(id);
                return Ok(JsonConvert.SerializeObject(report, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ReportEndpoints.GetByVideoId)]
        public async Task<IActionResult> GetByVideoAsync()
        {
            try
            {
                var videoId = Convert.ToInt64(Request.Query["VideoId"].First());
                var report = await _reportService.GetByVideo(videoId);
                return Ok(JsonConvert.SerializeObject(report, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ReportEndpoints.GetBySessionId)]
        public async Task<IActionResult> GetBySessionAsync()
        {
            try
            {
                var sessionId = Convert.ToInt64(Request.Query["SessionId"].First());
                var reports = await _reportService.GetBySession(sessionId);
                return Ok(JsonConvert.SerializeObject(reports, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.ReportEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var reports = await _reportService.GetByUser(userId);
                return Ok(JsonConvert.SerializeObject(reports, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route(Endpoints.ReportEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var report = JsonConvert.DeserializeObject<LaneDepartureWarningReport>(await streamReader.ReadToEndAsync());
                return Ok(await _reportService.SetAsync(report));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.ReportEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var report = await _reportService.GetById(id);
                await _reportService.DeleteAsync(report);
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
