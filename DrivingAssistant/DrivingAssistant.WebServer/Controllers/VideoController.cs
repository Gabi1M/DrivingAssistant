﻿using System;
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
    public class VideoController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("videos")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log("Received GET video from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var videoService = new VideoService(Constants.ServerConstants.GetConnectionString());
                return Ok(await videoService.GetAsync());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("videos")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log("Received POST videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var videoService = new VideoService(Constants.ServerConstants.GetConnectionString());
                var stream = Request.Body;
                var filepath = Utils.GetRandomFilename(".avi", "video");
                var file = System.IO.File.Create(filepath);
                await stream.CopyToAsync(file);
                file.Close();
                Logger.Log("Finished saving video to " + filepath, LogType.Info);
                var video = new Video(filepath, 0, 0, 0, string.Empty, Request.HttpContext.Connection.RemoteIpAddress.ToString(), DateTime.Now);
                await videoService.SetAsync(video);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("videos")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log("Received PUT videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                using var videoService = new VideoService(Constants.ServerConstants.GetConnectionString());
                var video = JsonConvert.DeserializeObject<Video>(await streamReader.ReadToEndAsync());
                await videoService.UpdateAsync(video);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem();
            }
        }

        //============================================================
        [HttpDelete]
        [Route("videos")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log("Received DELETE videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var videoService = new VideoService(Constants.ServerConstants.GetConnectionString());
                await videoService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route("videos_download")]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                Logger.Log("Received GET videos_download from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var videoService = new VideoService(Constants.ServerConstants.GetConnectionString());
                var video = (await videoService.GetAsync()).First(x => x.Id == id);
                return File(System.IO.File.Open(video.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), "video/mp4");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }
    }
}
