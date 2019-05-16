using Amazon;
using Amazon.CloudWatchEvents;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.IdentityManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebApplication.Exceptions;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ContainerService : IContainerService
    {
        private readonly IAmazonECS _ecsClient;

        private readonly IAmazonECR _ecrClient;

        private readonly ICloudWatchService _cloudWatchService;

        private readonly IIamService _iamService;

        private readonly ILogger _logger;

        private readonly string _ecsRole;

        /// <summary>
        /// The parameter 'credentials' is injected, see Startup.cs
        /// </summary>
        /// <param name="credentials"></param>
        public ContainerService(ICloudWatchService cloudWatchService, IIamService iamService, IOptions<AwsDevCredentials> credentials, IConfiguration configuration, ILogger<ContainerService> logger)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _ecsClient = new AmazonECSClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
            _ecrClient = new AmazonECRClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
            _ecsRole = configuration.GetSection("IamRoles").GetSection("EcsEventsRole").Value;
            _cloudWatchService = cloudWatchService;
            _iamService = iamService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a repository on ECR.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task CreateRepositoryAsync(Models.Repository repository)
        {
            if (repository.Name.Equals(""))
                throw new InvalidRepositoryName("Repository name cannot be an empty string.");

            try
            {
                await _ecrClient.CreateRepositoryAsync(new CreateRepositoryRequest
                {
                    RepositoryName = repository.Name
                });
            }
            catch (RepositoryAlreadyExistsException)
            {
                throw;
            }
            catch (AmazonECRException)
            {
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> CheckImageAsync(Models.Repository repository)
        {
            if (repository.Name.Equals(""))
                throw new InvalidRepositoryName("Repository name cannot be an empty string.");

            try
            {
                var response = await _ecrClient.ListImagesAsync(new ListImagesRequest
                {
                    RepositoryName = repository.Name
                });

                return response.ImageIds.Any();
            }
            catch (AmazonECRException)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a task definition on ECS.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task CreateConfigurationAsync(ImageConfiguration config)
        {
            try
            {
                await _ecsClient.RegisterTaskDefinitionAsync(new RegisterTaskDefinitionRequest
                {
                    Family = config.Name,
                    ContainerDefinitions = new List<ContainerDefinition>
                    {
                        new ContainerDefinition
                        {
                            Name = config.ContainerName,
                            Memory = 128,
                            Essential = true,
                            Image = config.ImageUri,
                            Interactive = config.Interactive,
                            PseudoTerminal = config.PseudoTerminal,
                            LogConfiguration = new LogConfiguration
                            {
                                LogDriver = LogDriver.Awslogs,
                                Options = new Dictionary<string, string>
                                {
                                    { "awslogs-group", $"/ecs/container-log-group" },
                                    { "awslogs-region", "eu-central-1" },
                                    { "awslogs-stream-prefix", $"ecs/{config.Name}" }
                                }
                            }
                        }
                    }
                });
            }
            catch (AmazonECSException)
            {
                throw;
            }
        }

        /// <summary>
        /// Runs the image on ECS.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task RunImageAsync(RunImageConfiguration runImageConfig)
        {
            var taskDefinitionsResponse = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = runImageConfig.ConfigName
            });
            if (!taskDefinitionsResponse.TaskDefinitionArns.Any())
            {
                throw new InexistentTaskDefinition($"Configuration does not exists: {runImageConfig.ConfigName}");
            }

            var clustersResponse = await _ecsClient.ListClustersAsync(new ListClustersRequest());
            string clusterArn;
            try
            {
                clusterArn = clustersResponse.ClusterArns.First();
            }
            catch (Exception)
            {
                throw new InexistentCluster("No ECS clusters have been found");
            }

            try
            {
                await _ecsClient.RunTaskAsync(new RunTaskRequest
                {
                    Cluster = clusterArn,
                    Group = runImageConfig.TaskGroupName,
                    StartedBy = "StreamAnalysis User",
                    Count = 1,
                    LaunchType = Amazon.ECS.LaunchType.EC2,
                    TaskDefinition = $"{runImageConfig.ConfigName}"
                });
            }
            catch (AmazonECSException)
            {
                throw;
            }
        }

        /// <summary>
        /// Stops a task based on its ARN
        /// </summary>
        /// <param name="taskArn"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task StopTaskAsync(string taskArn)
        {
            // get ECS cluster ARN
            var clustersResponse = await _ecsClient.ListClustersAsync(new ListClustersRequest());
            string clusterArn;
            try
            {
                clusterArn = clustersResponse.ClusterArns.First();
            }
            catch (Exception)
            {
                throw new InexistentCluster("No ECS clusters have been found");
            }

            try
            {
                await _ecsClient.StopTaskAsync(new StopTaskRequest()
                {
                    Cluster = clusterArn,
                    Task = taskArn
                });
            }
            catch (AmazonECSException)
            {
                throw;
            }
        }

        /// <summary>
        /// Schedules the image to run at fixed interval
        /// </summary>
        /// <param name="scheduledImageFixedRate"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task RunScheduledImageAsync(ScheduledImageFixedRate scheduledImageFixedRate)
        {
            // get latest revision task definition ARN
            var taskDefinitionsResponse = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = scheduledImageFixedRate.ConfigName,
                Sort = SortOrder.DESC
            });
            string taskDefinitionArn;
            try
            {
                taskDefinitionArn = taskDefinitionsResponse.TaskDefinitionArns.First();
            }
            catch (Exception)
            {
                throw new InexistentTaskDefinition($"Configuration does not exists: {scheduledImageFixedRate.ConfigName}");
            }

            // get ECS cluster ARN
            var clustersResponse = await _ecsClient.ListClustersAsync(new ListClustersRequest());
            string clusterArn;
            try
            {
                clusterArn = clustersResponse.ClusterArns.First();
            }
            catch (Exception)
            {
                throw new InexistentCluster("No ECS clusters have been found");
            }

            // get target role ARN
            string targetRoleArn;
            try
            {
                targetRoleArn = await _iamService.GetRoleArnFromNameAsync(_ecsRole);
            }
            catch (AmazonIdentityManagementServiceException)
            {
                throw;
            }

            try
            {
                // create event rule
                // rule name basically will be userId + ruleName
                await _cloudWatchService.CreateSchedulerRuleAsync(scheduledImageFixedRate.RuleName, scheduledImageFixedRate.ToString());

                // create target for rule
                await _cloudWatchService.CreateTargetForRuleAsync(scheduledImageFixedRate.RuleName, targetRoleArn, clusterArn, taskDefinitionArn, scheduledImageFixedRate.TasksGroupName);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Schedules the image to run based on a cron expression
        /// </summary>
        /// <param name="scheduledImageCronExpression"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task RunScheduledImageAsync(ScheduledImageCronExpression scheduledImageCronExpression)
        {
            // get latest revision task definition ARN
            var response = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = scheduledImageCronExpression.ConfigName,
                Sort = SortOrder.DESC
            });
            string taskDefinitionArn;
            try
            {
                taskDefinitionArn = response.TaskDefinitionArns.First();
            }
            catch (Exception)
            {
                throw new InexistentTaskDefinition($"Configuration does not exists: {scheduledImageCronExpression.ConfigName}");
            }

            // get ESC cluster ARN
            var clustersResponse = await _ecsClient.ListClustersAsync(new ListClustersRequest());
            string clusterArn;
            try
            {
                clusterArn = clustersResponse.ClusterArns.First();
            }
            catch (Exception)
            {
                throw new InexistentCluster("No ECS clusters have been found");
            }

            // get target role ARN
            string targetRoleArn;
            try
            {
                targetRoleArn = await _iamService.GetRoleArnFromNameAsync(_ecsRole);
            }
            catch (AmazonIdentityManagementServiceException)
            {
                throw;
            }

            try
            {
                // create event rule
                // rule name basically will be userId + ruleName
                await _cloudWatchService.CreateSchedulerRuleAsync(scheduledImageCronExpression.RuleName, scheduledImageCronExpression.ToString());

                // create target for rule
                await _cloudWatchService.CreateTargetForRuleAsync(scheduledImageCronExpression.RuleName, targetRoleArn, clusterArn, taskDefinitionArn, scheduledImageCronExpression.TasksGroupName);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Stoppes the scheduled image
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task StopScheduledImageAsync(string configName)
        {
            try
            {
                // delete event rule
                await _cloudWatchService.DeleteSchedulerRuleAsync(configName);
            }
            catch (AmazonCloudWatchEventsException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Lists tasks based on their group name
        /// </summary>
        /// <param name="tasksGroupName"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<List<Models.Container>> ListTasksFromGroupAsync(string tasksGroupName)
        {
            // get ECS cluster ARN
            var clustersResponse = await _ecsClient.ListClustersAsync(new ListClustersRequest());
            string clusterArn;
            try
            {
                clusterArn = clustersResponse.ClusterArns.First();
            }
            catch (Exception)
            {
                throw new InexistentCluster("No ECS clusters have been found");
            }

            // list tasks based on cluster
            var taskArns = new List<string>();
            try
            {
                var listTasksResponse = await _ecsClient.ListTasksAsync(new ListTasksRequest()
                {
                    Cluster = clusterArn
                });
                taskArns = listTasksResponse.TaskArns;
            }
            catch (AmazonECSException)
            {
                throw;
            }
            if (!taskArns.Any())
                return new List<WebApplication.Models.Container>();

            // describe tasks
            try
            {
                var describeTasksResponse = await _ecsClient.DescribeTasksAsync(new DescribeTasksRequest()
                {
                    Cluster = clusterArn,
                    Tasks = taskArns
                });

                var foundContainers = from task in describeTasksResponse.Tasks
                                      where task.Group.Equals(tasksGroupName)
                                      select new WebApplication.Models.Container()
                                      {
                                          Id = task.Containers.First().Name,
                                          TaskId = task.TaskArn,
                                          Status = task.LastStatus,
                                          StartedAt = task.StartedAt.Equals(DateTime.MinValue) ? "" :
                                            task.StartedAt.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))
                                      };

                return foundContainers.ToList();
            }
            catch (AmazonECSException)
            {
                throw;
            }
        }
    }
}