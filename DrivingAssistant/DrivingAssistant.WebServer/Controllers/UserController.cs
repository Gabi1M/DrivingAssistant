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
    public class UserController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log("Received GET users from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var userService = new UserService(Constants.ServerConstants.GetConnectionString());
                return Ok(await userService.GetAsync());
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
                using var userService = new UserService(Constants.ServerConstants.GetConnectionString());
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                return Ok(await userService.SetAsync(user));
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
                using var userService = new UserService(Constants.ServerConstants.GetConnectionString());
                var user = JsonConvert.DeserializeObject<User>(await streamReader.ReadToEndAsync());
                await userService.UpdateAsync(user);
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
                using var userService = new UserService(Constants.ServerConstants.GetConnectionString());
                var user = await userService.GetByIdAsync(id);
                await userService.DeleteAsync(user);
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
