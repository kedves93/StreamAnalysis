using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly IRedisService _redis;

        private readonly IDynamoDBService _dynamoDBService;

        /// <summary>
        /// The parameter 'dynamoDBService' is injected, see Startup.cs
        /// </summary>
        /// <param name="dynamoDBService"></param>
        public AuthController(IDynamoDBService dynamoDBService, IRedisService redis, ILogger<AuthController> logger)
        {
            _dynamoDBService = dynamoDBService;
            _redis = redis;
            _logger = logger;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SignIn([FromBody] SignInUserInfo user)
        {
            if (!await _dynamoDBService.ValidateUserAsync(user))
                return false;

            if (user.RememberMe)
            {
                _redis.StoreSession(user.Username);
            }

            return true;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<bool>> IsLoggedIn()
        {
            return _redis.CheckSession();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<bool>> SignOut()
        {
            _redis.DeleteSession();
            return true;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> Register([FromBody] RegisterUserInfo user)
        {
            return await _dynamoDBService.RegisterUserAsync(user);
        }
    }
}