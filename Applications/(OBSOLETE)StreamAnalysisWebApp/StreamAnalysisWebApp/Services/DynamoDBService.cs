using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using StreamAnalysisWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamAnalysisWebApp.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;

        public DynamoDBService()
        {
        }

        /// <summary>
        /// The parameter 'credentials' is injected, see Startup.cs
        /// </summary>
        /// <param name="credentials"></param>
        public DynamoDBService(IOptions<AwsDevCredentials> credentials)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _dynamoDBClient = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
        }

        public async Task<bool> ValidateUser(SignInUserInfo user)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("Username", ScanOperator.Equal, user.Username),
                new ScanCondition("Password", ScanOperator.Equal, user.Password)
            };

            // scan
            List<User> foundUsers = await context.ScanAsync<User>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            return foundUsers.Any();
        }
    }
}