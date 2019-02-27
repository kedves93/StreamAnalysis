using StackExchange.Redis;
using System;
using System.Text;

namespace WebApplication.Services
{
    public class RedisService : IRedisService
    {
        public string RedisUrl { get; set; }

        public RedisService(string redisUrl)
        {
            RedisUrl = redisUrl;
        }

        public void StoreSession(string userName)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisUrl);
            IDatabase db = redis.GetDatabase();
            var encodedUserName = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
            db.HashSet("loggedInUsers", "users", encodedUserName);
            db.StringSet("currentUser", encodedUserName, TimeSpan.FromDays(2));
        }

        public bool CheckSession()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisUrl);
            IDatabase db = redis.GetDatabase();
            string value = db.HashGet("loggedInUsers", "users");
            string currentUser = db.StringGet("currentUser");
            return currentUser == value;
        }

        public void DeleteSession()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisUrl);
            IDatabase db = redis.GetDatabase();
            db.KeyDelete("currentUser");
        }
    }
}