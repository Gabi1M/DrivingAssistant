using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.ImageProcessing;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Mssql;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        //============================================================
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _userService = IUserService.NewInstance(typeof(MssqlUserService));
                var users = await _userService.GetAsync();
                return Ok(JsonConvert.SerializeObject(users, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received POST users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _userService = IUserService.NewInstance(typeof(MssqlUserService));
                var userSettingsService = IUserSettingsService.NewInstance(typeof(MssqlUserSettingsService));
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                user.Id = await _userService.SetAsync(user);
                var userSettings = new UserSettings
                {
                    UserId = user.Id,
                    Parameters = Parameters.Default()
                };
                await userSettingsService.SetAsync(userSettings);
                return Ok(user.Id);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
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
                Logger.Log(
                    "Received PUT users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _userService = IUserService.NewInstance(typeof(MssqlUserService));
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                await _userService.SetAsync(user);
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
        [Route("users")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _userService = IUserService.NewInstance(typeof(MssqlUserService));
                var user = (await _userService.GetAsync()).First(x => x.Id == id);
                await _userService.DeleteAsync(user);
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
