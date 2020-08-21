using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly IUserService _userService = IUserService.CreateNew();
        private static readonly IUserSettingsService _userSettingsService = IUserSettingsService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.UserEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
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
        [HttpGet]
        [Route(Endpoints.UserEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var user = await _userService.GetById(id);
                return Ok(JsonConvert.SerializeObject(user, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route(Endpoints.UserEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                var userId = await _userService.SetAsync(user);
                if (user.Id == -1)
                {
                    await _userSettingsService.SetAsync(UserSettings.Default(userId));
                }
                return Ok(userId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.UserEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var user = await _userService.GetById(id);
                await _userService.DeleteAsync(user);
                await _userSettingsService.DeleteAsync(await _userSettingsService.GetByUser(id));
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
