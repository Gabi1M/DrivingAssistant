using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportService _reportService;

        //============================================================
        [HttpGet]
        [Route("reports")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET reports from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _reportService = new MssqlReportService(Constants.ServerConstants.GetMssqlConnectionString());
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
        [HttpPost]
        [Route("reports")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log(
                    "Received POST reports from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _reportService = new MssqlReportService(Constants.ServerConstants.GetMssqlConnectionString());
                var report = JsonConvert.DeserializeObject<Report>(await streamReader.ReadToEndAsync());
                return Ok(await _reportService.SetAsync(report));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("reports")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log(
                    "Received PUT reports from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);

                using var streamReader = new StreamReader(Request.Body);
                _reportService = new MssqlReportService(Constants.ServerConstants.GetMssqlConnectionString());
                var report = JsonConvert.DeserializeObject<Report>(await streamReader.ReadToEndAsync());
                await _reportService.SetAsync(report);
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
        [Route("reports")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE reports from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);

                var id = Convert.ToInt64(Request.Query["Id"].First());
                _reportService = new MssqlReportService(Constants.ServerConstants.GetMssqlConnectionString());
                var report = (await _reportService.GetAsync()).First(x => x.Id == id);
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
