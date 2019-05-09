using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<string> GetFromQueueAsync(string queueName)
        {
            string userId = queueName.Split("-")[1];

            try
            {
                using (GetObjectResponse response = await _s3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = _bucket,
                    Key = userId + "/" + queueName
                }))
                {
                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        return reader.ReadToEnd();
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