using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class SessionController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("sessions")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log("Received GET sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var sessionService = new SessionService(Constants.ServerConstants.GetConnectionString());
                return Ok(await sessionService.GetAsync());
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
                using var sessionService = new SessionService(Constants.ServerConstants.GetConnectionString());
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                return Ok(await sessionService.SetAsync(session));
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
                using var sessionService = new SessionService(Constants.ServerConstants.GetConnectionString());
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                await sessionService.UpdateAsync(session);
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
                using var sessionService = new SessionService(Constants.ServerConstants.GetConnectionString());
                var session = await sessionService.GetByIdAsync(id);
                await sessionService.DeleteAsync(session);
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
