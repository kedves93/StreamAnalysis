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
        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SignIn([FromBody] SignInUserInfo user)
        {
            return await DynamoDBService.ValidateUser(user);
        }
    }
}