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
    public class UserController : ControllerBase
    {
        private UserService _userService;

        //============================================================
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log("Received GET users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                _userService = new PsqlUserService(Constants.ServerConstants.GetConnectionString());
                return Ok(await _userService.GetAsync());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log("Received POST users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                _userService = new PsqlUserService(Constants.ServerConstants.GetConnectionString());
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                return Ok(await _userService.SetAsync(user));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("users")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log("Received PUT users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                _userService = new PsqlUserService(Constants.ServerConstants.GetConnectionString());
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                await _userService.UpdateAsync(user);
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
        [Route("users")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log("Received DELETE users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                _userService = new PsqlUserService(Constants.ServerConstants.GetConnectionString());
                var user = await _userService.GetByIdAsync(id);
                await _userService.DeleteAsync(user);
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
