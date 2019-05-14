﻿using System.Threading.Tasks;

namespace WebApplication.Interfaces
{
    public interface ICloudWatchService
    {
        Task CreateSchedulerRuleAsync(string ruleName, string expression);

        Task CreateTargetForRuleAsync(string ruleName, string clusterArn, string taskDefinitionArn);

        Task DeleteSchedulerRuleAsync(string ruleName);
    }
}