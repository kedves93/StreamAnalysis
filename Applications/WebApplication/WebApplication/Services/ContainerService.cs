using Amazon;
using Amazon.ECR;
using Amazon.ECR.Model;
using Amazon.ECS;
using Amazon.ECS.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Exceptions;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ContainerService : IContainerService
    {
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
            catch (AmazonECSException)
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
        public async System.Threading.Tasks.Task RunImageAsync(string configName)
        {
            var response = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = configName
            });
            if (!response.TaskDefinitionArns.Any())
            {
                throw new InexistentTaskDefinition();
            }

            try
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
            catch (AmazonECSException)
            {
                throw;
            }
        }
    }
}