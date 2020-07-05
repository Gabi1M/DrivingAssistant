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
    public class ImageController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("images")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log("Received GET image from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var imageService = new ImageService(Constants.ServerConstants.ConnectionString);
                return Ok(await imageService.GetAsync());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("images")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log("Received POST image from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                using var streamReader = new StreamReader(Request.Body);
                using var imageService = new ImageService(Constants.ServerConstants.ConnectionString);
                var base64Bytes = Convert.FromBase64String(await streamReader.ReadToEndAsync());
                var bitmap = Utils.Base64ToBitmap(base64Bytes);
                var filepath = Utils.GetRandomFilename("." + bitmap.RawFormat);
                bitmap.Save(filepath, bitmap.RawFormat);
                var image = new Image(
                    filepath, 
                    bitmap.Width, 
                    bitmap.Height, 
                    bitmap.RawFormat.ToString(),
                    Request.HttpContext.Connection.RemoteIpAddress.ToString(), 
                    DateTime.Now);
                return Ok(await imageService.SetAsync(image));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route("images")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log("Received DELETE image from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var imageService = new ImageService(Constants.ServerConstants.ConnectionString);
                await imageService.DeleteAsync(id);
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
        [Route("images_download")]
        public async Task<IActionResult> DownloadAsync()
        {
            try
            {
                Logger.Log("Received GET images_download from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" + Request.HttpContext.Connection.RemotePort, LogType.Info);
                var id = Convert.ToInt64(Request.Query["id"].First());
                using var imageService = new ImageService(Constants.ServerConstants.ConnectionString);
                var image = (await imageService.GetAsync()).First(x => x.Id == id);
                return File(System.IO.File.Open(image.Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return Problem(ex.Message);
            }
        }
    }
}
