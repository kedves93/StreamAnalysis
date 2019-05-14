using Amazon;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CloudWatchService : ICloudWatchService
    {
        private const string CLOUDWATCH_EVENTS_ROLE_ARN = "arn:aws:iam::526110916966:role/ecsEventsRole";

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
                    Description = $"Created {DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))}",
                    ScheduleExpression = expression
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task CreateTargetForRuleAsync(string ruleName, string clusterArn, string taskDefinitionArn)
        {
            try
            {
                await _cloudWatchClient.PutTargetsAsync(new PutTargetsRequest()
                {
                    Rule = ruleName,
                    Targets = new List<Target>()
                    {
                        new Target()
                        {
                            Arn = clusterArn,
                            RoleArn = CLOUDWATCH_EVENTS_ROLE_ARN,
                            Id = "targetId_" + ruleName,
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