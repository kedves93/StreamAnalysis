namespace WebApplication.Models
{
    public class ScheduledImageFixedRate
    {
        public string ConfigName { get; set; }

        public string RuleName { get; set; }

        public string TasksGroupName { get; set; }

        public int Rate { get; set; }

        public string Time { get; set; }

        public override string ToString()
        {
            switch (Time)
            {
                case "minutes":
                    if (Rate == 1)
                        Time = "minute";
                    break;

                case "hours":
                    if (Rate == 1)
                        Time = "hour";
                    break;

                case "days":
                    if (Rate == 1)
                        Time = "day";
                    break;
            }

            return $"rate({Rate} {Time})";
        }
    }
}