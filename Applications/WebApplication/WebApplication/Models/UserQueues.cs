using System.Collections.Generic;

namespace WebApplication.Models
{
    public class UserQueues
    {
        public string UserId { get; set; }

        public List<string> Queues { get; set; }
    }
}