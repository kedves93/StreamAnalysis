using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IDynamoDBService _dynamoDBService;

        /// <summary>
        /// The parameter 'dynamoDBService' is injected, see Startup.cs
        /// </summary>
        /// <param name="dynamoDBService"></param>
        public AuthController(IDynamoDBService dynamoDBService, ILogger<AuthController> logger)
        {
            _dynamoDBService = dynamoDBService;
            _logger = logger;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SignIn([FromBody] SignInUserInfo user)
        {
            if (!await _dynamoDBService.ValidateUserAsync(user))
                return false;

            return true;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> Register([FromBody] RegisterUserInfo user)
        {
            return await _dynamoDBService.RegisterUserAsync(user);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<string>> GetCurrentUserId([FromBody] string username)
        {
            string id = await _dynamoDBService.GetUserIdFromUsernameAsync(username);
            return JsonConvert.SerializeObject(id);
        }
    }
}