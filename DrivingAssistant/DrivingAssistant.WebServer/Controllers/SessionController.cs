using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Psql;
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
                Logger.Log("Received GET sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                _sessionService = new PsqlSessionService(Constants.ServerConstants.GetConnectionString());
                return Ok(await _sessionService.GetAsync());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
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
                Logger.Log("Received POST sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = new PsqlSessionService(Constants.ServerConstants.GetConnectionString());
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                return Ok(await _sessionService.SetAsync(session));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
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
                Logger.Log("Received PUT sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = new PsqlSessionService(Constants.ServerConstants.GetConnectionString());
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                await _sessionService.UpdateAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
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
                Logger.Log("Received DELETE sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                _sessionService = new PsqlSessionService(Constants.ServerConstants.GetConnectionString());
                var session = await _sessionService.GetByIdAsync(id);
                await _sessionService.DeleteAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }
    }
}
