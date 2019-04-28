using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
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

        public async Task<bool> ValidateUserAsync(SignInUserInfo user)
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

        public async Task<bool> RegisterUserAsync(RegisterUserInfo user)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // save
            await context.SaveAsync(new User
            {
                UserId = Guid.NewGuid().ToString("N"),
                Email = user.Email,
                Username = user.Username,
                Password = user.Password
            });

            // dispose context
            context.Dispose();

            return true;
        }

        public async Task<string> GetUserIdFromUsernameAsync(string username)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("Username", ScanOperator.Equal, username)
            };

            // scan
            List<User> foundUsers = await context.ScanAsync<User>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            return foundUsers.First().UserId;
        }

        public async Task<List<string>> GetQueuesFromUserIdAsync(string userId)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("UserId", ScanOperator.Equal, userId)
            };

            // scan
            List<UserChannels> foundUserQueues = await context.ScanAsync<UserChannels>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            return foundUserQueues.First().Queues;
        }

        public async Task<List<string>> GetTopicsFromUserIdAsync(string userId)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("UserId", ScanOperator.Equal, userId)
            };

            // scan
            List<UserChannels> foundUserQueues = await context.ScanAsync<UserChannels>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            return foundUserQueues.First().Topics;
        }

        public async Task<bool> SaveUserChannelsAsync(UserChannels channels)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // save
            await context.SaveAsync(new UserChannels
            {
                UserId = channels.UserId,
                Queues = channels.Queues,
                Topics = channels.Topics
            });

            // dispose context
            context.Dispose();

            return true;
        }

        public async Task<bool> DeleteChannelsFromUserIdAsync(string userId)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // creating an UserChannels object just for its UserId
            // DynamoDb will delete the record matching has this UserId
            UserChannels userChannels = new UserChannels
            {
                UserId = userId
            };

            // delete
            await context.DeleteAsync(userChannels);

            // dispose context
            context.Dispose();

            return true;
        }
    }
}