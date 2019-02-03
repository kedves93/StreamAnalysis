using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<ActionResult> CreateTaskRepository([FromBody] Repository repository)
        {
            await _containerService.CreateRepositoryAsync(repository);

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> ConfigureImage([FromBody] ImageConfiguration config)
        {
            await _containerService.ConfigureImageAsync(config);

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> RunImage([FromBody] ImageConfigurationName name)
        {
            await _containerService.RunImageAsync(name);

            return Ok();
        }
    }
}