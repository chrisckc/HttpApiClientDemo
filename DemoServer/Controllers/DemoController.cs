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

        [HttpGet("{id}")]
        public ActionResult<dynamic> Get200(int id)
        {
            var obj = new { Demo = new { Title = $"Get200 {id}", Message = $"Ok {id}", Timestamp = DateTime.Now } };
            return obj;
        }

        public ActionResult<dynamic> Get200()
        {
            var list = new List<dynamic>();
            for (int i = 0; i < 10; i++) {
                var demo =  new { Title = $"Get200 {i}", Message = $"Ok {i}", Timestamp = DateTime.Now.AddDays(i) };
                list.Add(demo);
            }
            var obj = new { Demos = list };
            return obj;
        }

        [HttpPost]
        public ActionResult<dynamic> Post201(dynamic obj)
        {
            return CreatedAtAction("Post201", obj); // resource created
        }

        [HttpPut]
        public ActionResult<dynamic> Put200(dynamic obj)
        {
            return obj; // resource updated
        }

        [HttpDelete("{id}")]
        public ActionResult<dynamic> Delete202(int id)
        {
            return StatusCode(202); // resource marked for deletion
        }

        [HttpDelete("{id}")]
        public ActionResult<dynamic> Delete204(int id)
        {
            return StatusCode(204); // resource deleted successfully
        }

        public ActionResult<dynamic> Get400()
        {
            var obj = new { Error = new { Title = "Get400", Message = "Bad Request", Timestamp = DateTime.Now } };
            return BadRequest(obj);
        }

        public ActionResult<dynamic> Get401()
        {
            return Unauthorized();
        }

        public ActionResult<dynamic> Get403()
        {
            var obj = new { Error = new { Title = "Get403", Message = "Forbidden", Timestamp = DateTime.Now } };
            return StatusCode(403, obj);
        }

        public ActionResult<dynamic> Get404()
        {
            var obj = new { Error = new { Title = "Get404", Message = "Not Found", Timestamp = DateTime.Now } };
            return NotFound(obj);
        }

        public ActionResult<dynamic> Get408()
        {
            var obj = new { Error = new { Title = "Get408", Message = "Request Timeout", Timestamp = DateTime.Now } };
            return StatusCode(408, obj);
        }

        public ActionResult<dynamic> Get409()
        {
            var obj = new { Error = new { Title = "Get409", Message = "Conflict", Timestamp = DateTime.Now } };
            return Conflict(obj);
        }

        [HttpGet("{retryAfter}")]
        public ActionResult<dynamic> Get429(int retryAfter)
        {
            var obj = new { Error = new { Title = "Get429", Message = $"Too Many Requests, Retry After {retryAfter} Seconds", Timestamp = DateTime.Now } };
            Request.HttpContext.Response.Headers.Add("Retry-After", retryAfter.ToString());
            return StatusCode(429, obj);
        }

        public ActionResult<dynamic> Get500()
        {
            var obj = new { Error = new { Title = "Get500", Message = "Server Error", Timestamp = DateTime.Now } };
            return StatusCode(500, obj);
        }

        [HttpGet("{delay}")]
        public async Task<ActionResult<dynamic>> GetDelay(int delay)
        {
            var obj = new { Demo = new { Title = "GetTimedOut", Message = $"Delayed for {delay} Seconds", Timestamp = DateTime.Now } };
            await Task.Delay(TimeSpan.FromSeconds(delay));
            return obj;
        }

        public async Task<ActionResult<dynamic>> GetTimedOut()
        {
            var obj = new { Demo = new { Title = "GetTimedOut", Message = "Deliberately Timed Out", Timestamp = DateTime.Now } };
            await Task.Delay(TimeSpan.FromSeconds(120));
            return obj;
        }
    }
}
