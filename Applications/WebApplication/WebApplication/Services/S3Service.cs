using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;

        private readonly ILogger _logger;

        private readonly string _bucket;

        public S3Service(IOptions<AwsDevCredentials> credentials, IConfiguration configuration, ILogger<S3Service> logger)
        {
            _logger = logger;
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUCentral1);
            _bucket = configuration.GetSection("Buckets").GetSection("StreamAnalysisBucket").Value;
        }

        public async Task<List<QueueMessage>> GetDataFromQueueAsync(string queue)
        {
            string userId = queue.Split("-")[0];
            List<QueueMessage> messages = new List<QueueMessage>();

            try
            {
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = _bucket,
                    Key = userId + "/" + queue
                }))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(QueueMessage));
                    using (StreamReader streamReader = new StreamReader(response.ResponseStream))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            var line = await streamReader.ReadLineAsync();
                            using (TextReader textReader = new StringReader(line))
                            {
                                messages.Add(serializer.Deserialize(textReader) as QueueMessage);
                            }
                        }

                        return messages;
                    }
                }
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}