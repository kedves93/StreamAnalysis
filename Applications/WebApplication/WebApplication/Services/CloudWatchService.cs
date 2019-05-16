using Amazon;
using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                    Description = $"Created {DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))}",
                    ScheduleExpression = expression
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task<List<SchedulerRule>> ListSchedulerRulesAsync(string ruleNamePrefix)
        {
            try
            {
                var response = await _cloudWatchClient.ListRulesAsync(new ListRulesRequest()
                {
                    NamePrefix = ruleNamePrefix
                });

                var foundRules = from rule in response.Rules
                                 select new SchedulerRule() { Id = rule.Name, State = rule.State };

                return foundRules.ToList();
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task DeleteSchedulerRuleAsync(string ruleName)
        {
            // list targets by rule name
            List<string> targetIds = new List<string>();
            try
            {
                var response = await _cloudWatchClient.ListTargetsByRuleAsync(new ListTargetsByRuleRequest()
                {
                    Rule = ruleName
                });
                var foundTargetIds = from target in response.Targets
                                     select target.Id;

                targetIds = foundTargetIds.ToList();
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }

            // remove targets from rule
            try
            {
                await _cloudWatchClient.RemoveTargetsAsync(new RemoveTargetsRequest()
                {
                    Rule = ruleName,
                    Ids = targetIds,
                    Force = true
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }

            // finally delete rule
            try
            {
                await _cloudWatchClient.DeleteRuleAsync(new DeleteRuleRequest()
                {
                    Name = ruleName,
                    Force = true
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task EnableSchedulerRuleAsync(string ruleName)
        {
            try
            {
                await _cloudWatchClient.EnableRuleAsync(new EnableRuleRequest()
                {
                    Name = ruleName
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task DisableSchedulerRuleAsync(string ruleName)
        {
            try
            {
                await _cloudWatchClient.DisableRuleAsync(new DisableRuleRequest()
                {
                    Name = ruleName
                });
            }
            catch (AmazonCloudWatchEventsException)
            {
                throw;
            }
        }

        public async Task CreateTargetForRuleAsync(string ruleName, string targetRoleArn, string clusterArn, string taskDefinitionArn, string tasksGroupName)
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
                            RoleArn = targetRoleArn,
                            Id = "targetId_" + ruleName,
                            EcsParameters = new EcsParameters()
                            {
                                Group = tasksGroupName,
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
    }
}