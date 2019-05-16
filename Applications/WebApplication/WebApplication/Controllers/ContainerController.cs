using Amazon.CloudWatchEvents;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.IdentityManagement;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebApplication.Exceptions;
using WebApplication.Interfaces;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly int _queueFlushingIntervalInMinutes;

        private readonly ISnsService _snsService;

        private readonly IDynamoDBService _dynamoDBService;

        private readonly IContainerService _containerService;

        private readonly ICloudWatchService _cloudWatchService;

        /// <summary>
        /// The parameter 'containerService' is injected, see Startup.cs
        /// </summary>
        /// <param name="containerService"></param>
        public ContainerController(IConfiguration configuration, IContainerService containerService, ISnsService snsService, IDynamoDBService dynamoDBService, ICloudWatchService cloudWatchService)
        {
            _queueFlushingIntervalInMinutes = int.Parse(configuration.GetSection("QueueFlushing").GetSection("IntervalInMinutes").Value);
            _containerService = containerService;
            _snsService = snsService;
            _dynamoDBService = dynamoDBService;
            _cloudWatchService = cloudWatchService;
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
        public async Task<ActionResult> RunImage([FromBody] RunImageConfiguration runImageConfig)
        {
            try
            {
                await _containerService.RunImageAsync(runImageConfig);
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
            catch (InexistentCluster ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (AmazonIdentityManagementServiceException ex)
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
            catch (AmazonIdentityManagementServiceException ex)
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
        public async Task<ActionResult<List<Container>>> ListContainers([FromBody] string tasksGroupName)
        {
            try
            {
                return await _containerService.ListTasksFromGroupAsync(tasksGroupName);
            }
            catch (InexistentCluster ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (AmazonECSException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> StopTask([FromBody] string taskId)
        {
            try
            {
                await _containerService.StopTaskAsync(taskId);
                return Ok();
            }
            catch (InexistentCluster ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
                await Task.Factory.StartNew(() => Observable.Interval(TimeSpan.FromMinutes(_queueFlushingIntervalInMinutes))
                    .Subscribe(async x => await _snsService.SendNotificationToFlushAsync(userQueues)));
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

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<SchedulerRule>>> ListSchedulerRules([FromBody] string ruleNamePrefix)
        {
            try
            {
                return await _cloudWatchService.ListSchedulerRulesAsync(ruleNamePrefix);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> DeleteSchedulerRule([FromBody] string ruleName)
        {
            try
            {
                await _cloudWatchService.DeleteSchedulerRuleAsync(ruleName);
                return Ok();
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> EnableSchedulerRule([FromBody] string ruleName)
        {
            try
            {
                await _cloudWatchService.EnableSchedulerRuleAsync(ruleName);
                return Ok();
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> DisableSchedulerRule([FromBody] string ruleName)
        {
            try
            {
                await _cloudWatchService.DisableSchedulerRuleAsync(ruleName);
                return Ok();
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}