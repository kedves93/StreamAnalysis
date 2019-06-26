using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Hubs;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IHubContext<TopicsHub> _hub;

        private readonly IDynamoDBService _dynamoDBService;

        public DashboardController(IHubContext<TopicsHub> hub, IDynamoDBService dynamoDBService)
        {
            _hub = hub;
            _dynamoDBService = dynamoDBService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllTopics()
        {
            return await _dynamoDBService.GetTopicsAsync();
        }
    }
}