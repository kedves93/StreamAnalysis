using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
    public interface ICloudWatchService
    {
        Task CreateSchedulerRuleAsync(string ruleName, string expression);

        Task<List<SchedulerRule>> ListSchedulerRulesAsync(string ruleNamePrefix);

        Task DeleteSchedulerRuleAsync(string ruleName);

        Task EnableSchedulerRuleAsync(string ruleName);

        Task DisableSchedulerRuleAsync(string ruleName);

        Task CreateTargetForRuleAsync(string ruleName, string targetRoleArn, string clusterArn, string taskDefinitionArn, string tasksGroupName);
    }
}