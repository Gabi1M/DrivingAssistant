﻿using System;
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
        private static readonly IUserSettingsService _userSettingsService = new MssqlUserSettingsService();

        //============================================================
        [HttpGet]
        [Route(Endpoints.UserSettingsEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
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
        [HttpGet]
        [Route(Endpoints.UserSettingsEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var userSettings = await _userSettingsService.GetById(id);
                return Ok(JsonConvert.SerializeObject(userSettings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.UserSettingsEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var userSettings = await _userSettingsService.GetByUser(userId);
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
        [Route(Endpoints.UserSettingsEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
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
        [HttpDelete]
        [Route(Endpoints.UserSettingsEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var userSettings = await _userSettingsService.GetById(id);
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
