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
        public async System.Threading.Tasks.Task ConfigureImageAsync(ImageConfiguration config)
        {
            await _ecsClient.RegisterTaskDefinitionAsync(new RegisterTaskDefinitionRequest
            {
                Family = config.Name.Id,
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
                                { "awslogs-group", $"/ecs/{config.Name.Id}" },
                                { "awslogs-region", "eu-central-1" },
                                { "awslogs-stream-prefix", "ecs" }
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
        public async System.Threading.Tasks.Task RunImageAsync(ImageConfigurationName name)
        {
            if (await validateImageConfigurationAsync(name))
            {
                await _ecsClient.RunTaskAsync(new RunTaskRequest
                {
                    Cluster = CLUSTER,
                    Count = 1,
                    LaunchType = LaunchType.EC2,
                    // StartedBy =
                    TaskDefinition = $"{name.Id}"
                });
            }
        }

        private async System.Threading.Tasks.Task<bool> validateImageConfigurationAsync(ImageConfigurationName name)
        {
            var response = await _ecsClient.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = name.Id
            });
            return response.TaskDefinitionArns.Any();
        }

        //public async System.Threading.Tasks.Task PushImageAsync(string repositoryName)
        //{
        //    var response = await _ecrClient.GetAuthorizationTokenAsync(new GetAuthorizationTokenRequest
        //    {
        //        RegistryIds = new List<string> { REGISTRY_ID }
        //    });
        //    var authorization = response.AuthorizationData.First();
        //    await _ecrClient.PutImageAsync(new PutImageRequest
        //    {
        //        RepositoryName = repositoryName,
        //        // ImageManifest = "",
        //        RegistryId = REGISTRY_ID,
        //        ImageTag = "tag"
        //    });
        //}
    }
}