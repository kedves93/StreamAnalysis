using Amazon;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.ECS.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ContainerService : IContainerService
    {
        private const string REGISTRY_ID = "526110916966"; // AWS Account number
        private const string CLUSTER = "stream-analysis-ecs-cluster";

        private readonly IAmazonECS _ecsClient;

        private readonly IAmazonECR _ecrClient;

        public ContainerService()
        {
        }

        /// <summary>
        /// The parameter 'credentials' is injected, see Startup.cs
        /// </summary>
        /// <param name="credentials"></param>
        public ContainerService(IOptions<AwsDevCredentials> credentials)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _ecsClient = new AmazonECSClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
            _ecrClient = new AmazonECRClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
        }

        /// <summary>
        /// Creates a repository on ECR.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task CreateRepositoryAsync(WebApplication.Models.Repository repository)
        {
            await _ecrClient.CreateRepositoryAsync(new CreateRepositoryRequest
            {
                RepositoryName = repository.Name
            });
        }

        /// <summary>
        /// Creates a task definition on ECS.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task CreateConfiguration(ImageConfiguration config)
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

        /// <summary>
        /// Runs the image on ECS.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task RunImageAsync(string configName)
        {
            if (await validateImageConfigurationAsync(configName))
            {
                await _ecsClient.RunTaskAsync(new RunTaskRequest
                {
                    Cluster = CLUSTER,
                    Count = 1,
                    LaunchType = LaunchType.EC2,
                    // StartedBy =
                    TaskDefinition = $"{configName}"
                });
            }
        }

        private async System.Threading.Tasks.Task<bool> validateImageConfigurationAsync(string configName)
        {
            var response = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = configName
            });
            return response.TaskDefinitionArns.Any();
        }
    }
}