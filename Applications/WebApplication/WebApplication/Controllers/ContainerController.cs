using Amazon.CloudWatchEvents;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ISnsService _snsService;

        private readonly IDynamoDBService _dynamoDBService;

        private readonly IContainerService _containerService;

        /// <summary>
        /// The parameter 'containerService' is injected, see Startup.cs
        /// </summary>
        /// <param name="containerService"></param>
        public ContainerController(IContainerService containerService, ISnsService snsService, IDynamoDBService dynamoDBService)
        {
            _containerService = containerService;
            _snsService = snsService;
            _dynamoDBService = dynamoDBService;
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
            catch (AmazonECRException ex)
            {
                return BadRequest(ex.Message);
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
            catch (InexistentTaskDefinition ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AmazonECSException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> ScheduleImageFixedRate([FromBody] ScheduledImageFixedRate scheduledImageFixedRate)
        {
            try
            {
                await _containerService.RunScheduledImageAsync(scheduledImageFixedRate);
                return Ok(JsonConvert.SerializeObject("Started image schedule successfully"));
            }
            catch (InexistentTaskDefinition ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> ScheduleImageCronExp([FromBody] ScheduledImageCronExpression scheduledImageCronExpression)
        {
            try
            {
                await _containerService.RunScheduledImageAsync(scheduledImageCronExpression);
                return Ok(JsonConvert.SerializeObject("Started image schedule successfully"));
            }
            catch (InexistentTaskDefinition ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> StopScheduledImage([FromBody] string configName)
        {
            try
            {
                await _containerService.StopScheduledImageAsync(configName);
                return Ok(JsonConvert.SerializeObject("Cancelled image schedule successfully"));
            }
            catch (AmazonCloudWatchEventsException ex)
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