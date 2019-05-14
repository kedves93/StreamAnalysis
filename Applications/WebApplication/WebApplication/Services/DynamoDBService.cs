using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            var hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(user.Password), Encoding.UTF8.GetBytes(user.Username));

            List<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("Username", ScanOperator.Equal, user.Username),
                new ScanCondition("Password", ScanOperator.Equal, Convert.ToBase64String(hash))
            };

            // scan
            if (user.ContainerUser)
            {
                List<ContainerUser> foundContainerUsers = await context.ScanAsync<ContainerUser>(conditions).GetRemainingAsync();

                // dispose context
                context.Dispose();

                return foundContainerUsers.Any();
            }
            else
            {
                List<DashboardUser> foundDashboardUsers = await context.ScanAsync<DashboardUser>(conditions).GetRemainingAsync();

                // dispose context
                context.Dispose();

                return foundDashboardUsers.Any();
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterUserInfo user)
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // save
            var hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(user.Password), Encoding.UTF8.GetBytes(user.Username));

            if (user.ContainerUser)
            {
                await context.SaveAsync(new ContainerUser
                {
                    UserId = Guid.NewGuid().ToString("N"),
                    Email = user.Email,
                    Username = user.Username,
                    Password = Convert.ToBase64String(hash)
                });
            }
            else
            {
                await context.SaveAsync(new DashboardUser
                {
                    UserId = Guid.NewGuid().ToString("N"),
                    Email = user.Email,
                    Username = user.Username,
                    Password = Convert.ToBase64String(hash)
                });
            }

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
            List<ContainerUser> foundUsers = await context.ScanAsync<ContainerUser>(conditions).GetRemainingAsync();

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
            List<UserChannels> foundUserTopics = await context.ScanAsync<UserChannels>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            return foundUserTopics.First().Topics;
        }

        public async Task<List<string>> GetTopicsAsync()
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>();

            // scan
            List<UserChannels> foundUserTopics = await context.ScanAsync<UserChannels>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            // all topics
            List<string> topics = new List<string>();
            foreach (UserChannels userChannels in foundUserTopics)
            {
                foreach (string topic in userChannels.Topics)
                {
                    topics.Add(topic);
                }
            }

            return topics;
        }

        public async Task<List<string>> GetQueuesAsync()
        {
            // context
            DynamoDBContext context = new DynamoDBContext(_dynamoDBClient);

            // conditions
            List<ScanCondition> conditions = new List<ScanCondition>();

            // scan
            List<UserChannels> foundUserQueues = await context.ScanAsync<UserChannels>(conditions).GetRemainingAsync();

            // dispose context
            context.Dispose();

            // all queues
            List<string> queues = new List<string>();
            foreach (UserChannels userChannels in foundUserQueues)
            {
                foreach (string queue in userChannels.Queues)
                {
                    queues.Add(queue);
                }
            }

            return queues;
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

        private byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}