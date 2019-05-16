namespace WebApplication.Models
{
    public class ScheduledImageCronExpression
    {
        public string ConfigName { get; set; }

        public string RuleName { get; set; }

        public string TasksGroupName { get; set; }

        public string CronExpression { get; set; }

        public override string ToString()
        {
            return $"cron({CronExpression})";
        }
    }
}