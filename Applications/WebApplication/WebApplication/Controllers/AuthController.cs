using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

            _logger.LogInformation("###############GOOOOD HelloWorld!!!!!!!!!!!!!!!!!!!");

            //StoreSession(user.Username);
            return true;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> Register([FromBody] RegisterUserInfo user)
        {
            return await _dynamoDBService.RegisterUserAsync(user);
        }

        private async void StoreSession(string sessionKey)
        {
            await HttpContext.Session.LoadAsync();
            if (HttpContext.Session.GetString(sessionKey) == null)
            {
                _logger.LogInformation("###############HelloWorld! Szilard");
                HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("s"));
                await HttpContext.Session.CommitAsync();
            }
            _logger.LogInformation("###############HelloWorld! Fuuuuck");
            var e = HttpContext.Session.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                _logger.LogInformation("###############SessionKey: " + e.Current);
            }
        }
    }
}