using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    [DynamoDBTable("UserChannelsTable")]
    public class UserChannels
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        public List<string> Queues { get; set; }

        public List<string> Topics { get; set; }
    }
}
