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
    public class ContainerController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IContainerService _containerService;

        /// <summary>
        /// The parameter 'containerService' is injected, see Startup.cs
        /// </summary>
        /// <param name="containerService"></param>
        /// <param name="logger"></param>
        public ContainerController(IContainerService containerService, ILogger<ContainerController> logger)
        {
            _containerService = containerService;
            _logger = logger;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateRepository([FromBody] Repository repository)
        {
            await _containerService.CreateRepositoryAsync(repository);

            return Ok(JsonConvert.SerializeObject($"Repository created successfully: {repository.Name}"));
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateConfiguration([FromBody] ImageConfiguration config)
        {
            await _containerService.CreateConfiguration(config);

            return Ok(JsonConvert.SerializeObject($"Configuration created successfully: {config.Name}"));
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> RunImage([FromBody] string configName)
        {
            await _containerService.RunImageAsync(configName);

            return Ok(JsonConvert.SerializeObject("Running image successfully"));
        }
    }
}