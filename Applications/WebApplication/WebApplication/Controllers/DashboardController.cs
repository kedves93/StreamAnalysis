using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Hubs;
using WebApplication.Interfaces;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IHubContext<TopicsHub> _hub;

        private readonly IDynamoDBService _dynamoDBService;

        private readonly IActiveMQService _activeMQService;

        public DashboardController(IHubContext<TopicsHub> hub, IDynamoDBService dynamoDBService, IActiveMQService activeMQService)
        {
            _hub = hub;
            _dynamoDBService = dynamoDBService;
            _activeMQService = activeMQService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllTopics()
        {
            return await _dynamoDBService.GetTopicsAsync();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> StartRealTimeMessagesFromTopic([FromBody] string topic)
        {
            _activeMQService.OnTopicMessage += async (sender, e) => await _hub.Clients.All.SendAsync(topic, e.Message);
            await _activeMQService.StartListeningOnTopicAsync(topic);

            return Ok(new { Message = "Backend started to send the real time messages on topic:" + topic });
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> StopRealTimeMessagesFromTopic([FromBody] string topic)
        {
            await _activeMQService.StopListeningOnTopicAsync(topic);

            return Ok(new { Message = "Backend stopped to send the real time messages on topic: " + topic });
        }
    }
}