using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Mssql;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class UserSettingsController : ControllerBase
    {
        private UserSettingsService _userSettingsService;

        //============================================================
        [HttpGet]
        [Route("user_settings")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET user_settings from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _userSettingsService = UserSettingsService.NewInstance(typeof(MssqlUserSettingsService));
                var userSettings = await _userSettingsService.GetAsync();
                return Ok(JsonConvert.SerializeObject(userSettings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("user_settings")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log(
                    "Received POST user_settings from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _userSettingsService = UserSettingsService.NewInstance(typeof(MssqlUserSettingsService));
                using var streamReader = new StreamReader(Request.Body);
                var userSettings = JsonConvert.DeserializeObject<UserSettings>(await streamReader.ReadToEndAsync());
                return Ok(await _userSettingsService.SetAsync(userSettings));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("user_settings")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log(
                    "Received PUT user_settings from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _userSettingsService = UserSettingsService.NewInstance(typeof(MssqlUserSettingsService));
                using var streamReader = new StreamReader(Request.Body);
                var userSettings = JsonConvert.DeserializeObject<UserSettings>(await streamReader.ReadToEndAsync());
                await _userSettingsService.UpdateAsync(userSettings);
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
        [Route("user_settings")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE user_settings from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _userSettingsService = UserSettingsService.NewInstance(typeof(MssqlUserSettingsService));
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var userSettings = await _userSettingsService.GetByIdAsync(id);
                await _userSettingsService.DeleteAsync(userSettings);
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
