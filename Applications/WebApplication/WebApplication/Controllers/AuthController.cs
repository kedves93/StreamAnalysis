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
        private readonly IDynamoDBService _dynamoDBService;

        private readonly ILogger<AuthController> _log;

        /// <summary>
        /// The parameter 'dynamoDBService' is injected, see Startup.cs
        /// </summary>
        /// <param name="dynamoDBService"></param>
        public AuthController(IDynamoDBService dynamoDBService, ILogger<AuthController> log)
        {
            _dynamoDBService = dynamoDBService;
            _log = log;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SignIn([FromBody] SignInUserInfo user)
        {
            _log.LogInformation("Hello, world!");
            return await _dynamoDBService.ValidateUser(user);
        }
    }
}