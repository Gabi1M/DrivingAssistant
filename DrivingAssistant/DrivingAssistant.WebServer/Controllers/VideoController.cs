using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;

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
                using var videoService = new VideoService(Constants.ServerConstants.ConnectionString);
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
                using var streamReader = new StreamReader(Request.Body);
                using var imageService = new ImageService(Constants.ServerConstants.ConnectionString);
                var base64Frames = (await streamReader.ReadToEndAsync()).Split(' ');
                foreach (var base64Frame in base64Frames)
                {
                    var bitmap = Utils.Base64ToBitmap(Convert.FromBase64String(base64Frame));
                    var filepath = Utils.GetRandomFilename("." + bitmap.RawFormat, "image");
                    bitmap.Save(filepath, bitmap.RawFormat);
                    var image = new Image(
                        filepath,
                        bitmap.Width,
                        bitmap.Height,
                        bitmap.RawFormat.ToString(),
                        Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        DateTime.Now);
                    await imageService.SetAsync(image);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("videos_2")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostAsync2()
        {
            try
            {
                Logger.Log("Received POST videos_2 from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var videoService = new VideoService(Constants.ServerConstants.ConnectionString);
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
        [HttpDelete]
        [Route("videos")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log("Received DELETE videos from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var videoService = new VideoService(Constants.ServerConstants.ConnectionString);
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
                using var videoService = new VideoService(Constants.ServerConstants.ConnectionString);
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
