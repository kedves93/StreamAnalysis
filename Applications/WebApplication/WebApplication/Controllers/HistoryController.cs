using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Interfaces;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IS3Service _s3Service;

        private readonly IDynamoDBService _dynamoDBService;

        public HistoryController(IS3Service s3Service, IDynamoDBService dynamoDBService)
        {
            _s3Service = s3Service;
            _dynamoDBService = dynamoDBService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllQueues()
        {
            return await _dynamoDBService.GetQueuesAsync();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<QueueMessage>>> GetHistoricalData([FromBody] string queue)
        {
            return await _s3Service.GetDataFromQueueAsync(queue);
        }
    }
}