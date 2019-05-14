using Amazon;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CloudWatchService : ICloudWatchService
    {
        private readonly IAmazonCloudWatchEvents _cloudWatchClient;

        public CloudWatchService(IOptions<AwsDevCredentials> credentials)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _cloudWatchClient = new AmazonCloudWatchEventsClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
        }

        public async Task CreateSchedulerRuleAsync(string ruleName, string expression)
        {
            try
            {
                await _cloudWatchClient.PutRuleAsync(new PutRuleRequest()
                {
                    Name = ruleName,
                    ScheduleExpression = expression
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task CreateTargetForRuleAsync(string taskDefinitionArn)
        {
            try
            {
                await _cloudWatchClient.PutTargetsAsync(new PutTargetsRequest()
                {
                    Rule = "RuleName",
                    Targets = new List<Target>()
                    {
                        new Target()
                        {
                            EcsParameters = new EcsParameters()
                            {
                                LaunchType = LaunchType.EC2,
                                TaskCount = 1,
                                TaskDefinitionArn = taskDefinitionArn
                            }
                        }
                    }
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task DeleteSchedulerRuleAsync(string ruleName)
        {
            try
            {
                await _cloudWatchClient.DeleteRuleAsync(new DeleteRuleRequest()
                {
                    Name = ruleName
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }
    }
}