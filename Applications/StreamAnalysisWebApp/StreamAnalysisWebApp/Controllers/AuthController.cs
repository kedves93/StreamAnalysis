using Microsoft.AspNetCore.Mvc;
using StreamAnalysisWebApp.Models;
using StreamAnalysisWebApp.Services;
using System.Threading.Tasks;

namespace StreamAnalysisWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDynamoDBService _dynamoDBService;

        /// <summary>
        /// The parameter 'dynamoDBService' is injected, see Startup.cs
        /// </summary>
        /// <param name="dynamoDBService"></param>
        public AuthController(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SignIn([FromBody] SignInUserInfo user)
        {
            return await _dynamoDBService.ValidateUser(user);
        }
    }
}