using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Hubs;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private IHubContext<MainHub> _hub;

        public DashboardController(IHubContext<MainHub> hub)
        {
            _hub = hub;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> SendMessage()
        {
            await _hub.Clients.All.SendAsync("s3", "hello");

            Thread.Sleep(2000);

            return Ok(new { Message = "Request Completed" });
        }
    }
}