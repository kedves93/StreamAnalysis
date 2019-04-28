using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebApplication.Exceptions;
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

        private readonly ISnsService _snsService;

        private readonly IDynamoDBService _dynamoDBService;

        /// <summary>
        /// The parameter 'containerService' is injected, see Startup.cs
        /// </summary>
        /// <param name="containerService"></param>
        /// <param name="logger"></param>
        public ContainerController(IContainerService containerService, ISnsService snsService, IDynamoDBService dynamoDBService, ILogger<ContainerController> logger)
        {
            _containerService = containerService;
            _snsService = snsService;
            _dynamoDBService = dynamoDBService;
            _logger = logger;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> CheckImage([FromBody] Models.Repository repository)
        {
            try
            {
                return await _containerService.CheckImageAsync(repository);
            }
            catch (InvalidRepositoryName ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AmazonECRException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateRepository([FromBody] Models.Repository repository)
        {
            try
            {
                await _containerService.CreateRepositoryAsync(repository);
                return Ok(JsonConvert.SerializeObject($"Repository created successfully: {repository.Name}"));
            }
            catch (InvalidRepositoryName ex)
            {
                return BadRequest(ex.Message);
            }
            catch (RepositoryAlreadyExistsException)
            {
                return BadRequest($"Repository already exists: {repository.Name}");
            }
            catch (AmazonECSException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateConfiguration([FromBody] ImageConfiguration config)
        {
            try
            {
                await _containerService.CreateConfigurationAsync(config);
                return Ok(JsonConvert.SerializeObject($"Configuration created successfully: {config.Name}"));
            }
            catch (AmazonECSException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> RunImage([FromBody] string configName)
        {
            try
            {
                await _containerService.RunImageAsync(configName);
                return Ok(JsonConvert.SerializeObject("Started image successfully"));
            }
            catch (InexistentTaskDefinition)
            {
                return BadRequest($"Configuration does not exists: {configName}");
            }
            catch (AmazonECSException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> StartFlushingQueues([FromBody] UserQueues userQueues)
        {
            try
            {
                await Task.Factory.StartNew(() => Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(async x => await _snsService.SendNotificationToFlushAsync(userQueues)));
                return Ok(JsonConvert.SerializeObject($"Started flushing queues successfully: {String.Join(",", userQueues.Queues)}"));
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<string>>> GetUserQueues([FromBody] string userId)
        {
            return await _dynamoDBService.GetQueuesFromUserIdAsync(userId);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<string>>> GetUserTopics([FromBody] string userId)
        {
            return await _dynamoDBService.GetTopicsFromUserIdAsync(userId);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> UpdateUserChannels([FromBody] UserChannels channels)
        {
            await _dynamoDBService.DeleteChannelsFromUserIdAsync(channels.UserId);
            return await _dynamoDBService.SaveUserChannelsAsync(channels);
        }
    }
}