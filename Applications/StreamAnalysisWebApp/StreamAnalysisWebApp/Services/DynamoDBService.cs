using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using StreamAnalysisWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamAnalysisWebApp.Services
{
    public static class DynamoDBService
    {
        private static readonly AmazonDynamoDBClient dynamoDBClient;
        private static readonly BasicAWSCredentials credentials;

        static DynamoDBService()
        {
            credentials = new BasicAWSCredentials("AKIAJHUARGC4GM2YYLJQ", "5tGFt26hQ4Z1FlT/eNiX/JxfJ5/VZKTz/VR/OS7c");
            dynamoDBClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1);
        }

        public static async Task<bool> ValidateUser(SignInUserInfo user)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);

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