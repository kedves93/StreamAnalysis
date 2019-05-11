using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication.Interfaces;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IS3Service _s3Service;

        public HistoryController(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<string>> GetHistoricalData([FromBody] string queue)
        {
            return await _s3Service.GetFromQueueAsync(queue);
        }
    }
}