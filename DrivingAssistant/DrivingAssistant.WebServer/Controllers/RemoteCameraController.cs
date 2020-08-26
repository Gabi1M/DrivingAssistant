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
    public class RemoteCameraController : ControllerBase
    {
        private static readonly IRemoteCameraService _remoteCameraService = IRemoteCameraService.CreateNew();

        //============================================================
        [HttpGet]
        [Route(Endpoints.RemoteCameraEndpoints.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var remoteCameras = await _remoteCameraService.GetAsync();
                return Ok(JsonConvert.SerializeObject(remoteCameras, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.RemoteCameraEndpoints.GetById)]
        public async Task<IActionResult> GetByIdAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var remoteCamera = await _remoteCameraService.GetById(id);
                return Ok(JsonConvert.SerializeObject(remoteCamera, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route(Endpoints.RemoteCameraEndpoints.GetByUserId)]
        public async Task<IActionResult> GetByUserAsync()
        {
            try
            {
                var userId = Convert.ToInt64(Request.Query["UserId"].First());
                var remoteCamera = await _remoteCameraService.GetByUser(userId);
                return Ok(JsonConvert.SerializeObject(remoteCamera, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route(Endpoints.RemoteCameraEndpoints.AddOrUpdate)]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var remoteCamera = JsonConvert.DeserializeObject<RemoteCamera>(await streamReader.ReadToEndAsync());
                return Ok(await _remoteCameraService.SetAsync(remoteCamera));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route(Endpoints.RemoteCameraEndpoints.Delete)]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                var id = Convert.ToInt64(Request.Query["Id"].First());
                var remoteCamera = await _remoteCameraService.GetById(id);
                await _remoteCameraService.DeleteAsync(remoteCamera);
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
