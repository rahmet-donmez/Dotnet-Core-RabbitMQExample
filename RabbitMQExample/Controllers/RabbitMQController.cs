using Microsoft.AspNetCore.Mvc;
using RabbitMQExample.Models;
using RabbitMQExample.Services;

namespace RabbitMQExample.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RabbitMqController : Controller
    {
        private readonly RabbitMQService _service;

        public RabbitMqController(RabbitMQService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] Notification notification)
        {
            _service.Publish(notification);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Consumer()
        {
            _service.Consumer();
            return Ok();
        }
       
    }
}
