using Microsoft.AspNetCore.Mvc;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class UtilsController : ControllerBase
    {
        //============================================================
        [HttpGet]
        [Route("check_connection")]
        public IActionResult CheckConnection()
        {
            return Ok("Success");
        }
    }
}
