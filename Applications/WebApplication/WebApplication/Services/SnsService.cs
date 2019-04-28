using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class SnsService : ISnsService
    {
        private readonly IAmazonSimpleNotificationService _snsClient;

        private readonly string _notificationTopicName;

        private readonly string _notificationTopicArn;

        public SnsService(IOptions<AwsDevCredentials> credentials, IConfiguration configuration)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _snsClient = new AmazonSimpleNotificationServiceClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
            _notificationTopicName = configuration.GetSection("SNSTopics").GetSection("ChannelTopic").GetSection("Name").Value;
            _notificationTopicArn = configuration.GetSection("SNSTopics").GetSection("ChannelTopic").GetSection("Arn").Value;
        }

        public async Task SendNotificationToFlushAsync(UserQueues queues)
        {
            try
            {
                await _snsClient.PublishAsync(new PublishRequest
                {
                    Message = JsonConvert.SerializeObject(queues),
                    TopicArn = _notificationTopicArn
                });
            }
            catch (AmazonSimpleNotificationServiceException)
            {
                throw;
            }
        }
    }
}