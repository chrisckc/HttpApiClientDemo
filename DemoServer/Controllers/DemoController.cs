using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        
        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<dynamic> Get200()
        {
            var obj = new { Demo = new { Title = "Get200", Message = "Ok", TimeStamp = DateTime.Now } };
            return obj;
        }

        public ActionResult<dynamic> Get201()
        {
            var obj = new { Demo = new { Title = "Get201", Message = "Created", TimeStamp = DateTime.Now } };
            return CreatedAtAction("Get201", obj);
        }

        public ActionResult<dynamic> Get400()
        {
            var obj = new { Error = new { Title = "Get400", Message = "Bad Request", TimeStamp = DateTime.Now } };
            return BadRequest(obj);
        }

        public ActionResult<dynamic> Get401()
        {
            return Unauthorized();
        }

        public ActionResult<dynamic> Get403()
        {
            var obj = new { Error = new { Title = "Get403", Message = "Forbidden", TimeStamp = DateTime.Now } };
            return StatusCode(403, obj);
        }

        public ActionResult<dynamic> Get404()
        {
            var obj = new { Error = new { Title = "Get404", Message = "Not Found", TimeStamp = DateTime.Now } };
            return NotFound(obj);
        }

        public ActionResult<dynamic> Get408()
        {
            var obj = new { Error = new { Title = "Get408", Message = "Request Timeout", TimeStamp = DateTime.Now } };
            return StatusCode(408, obj);
        }

        public ActionResult<dynamic> Get409()
        {
            var obj = new { Error = new { Title = "Get409", Message = "Conflict", TimeStamp = DateTime.Now } };
            return Conflict(obj);
        }

        [HttpGet("{retryAfter}")]
        public ActionResult<dynamic> Get429(int retryAfter)
        {
            var obj = new { Error = new { Title = "Get429", Message = $"Too Many Requests, Retry After {retryAfter} Seconds", TimeStamp = DateTime.Now } };
            Request.HttpContext.Response.Headers.Add("Retry-After", retryAfter.ToString());
            return StatusCode(429, obj);
        }

        public ActionResult<dynamic> Get500()
        {
            var obj = new { Error = new { Title = "Get500", Message = "Server Error", TimeStamp = DateTime.Now } };
            return StatusCode(500, obj);
        }

        [HttpGet("{delay}")]
        public async Task<ActionResult<dynamic>> GetDelay(int delay)
        {
            var obj = new { Demo = new { Title = "GetTimedOut", Message = $"Delayed for {delay} Seconds", TimeStamp = DateTime.Now } };
            await Task.Delay(TimeSpan.FromSeconds(delay));
            return obj;
        }

        public async Task<ActionResult<dynamic>> GetTimedOut()
        {
            var obj = new { Demo = new { Title = "GetTimedOut", Message = "Deliberately Timed Out", TimeStamp = DateTime.Now } };
            await Task.Delay(TimeSpan.FromSeconds(120));
            return obj;
        }
    }
}
