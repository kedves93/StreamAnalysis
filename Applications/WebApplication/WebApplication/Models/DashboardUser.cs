using Amazon.DynamoDBv2.DataModel;

namespace WebApplication.Models
{
    [DynamoDBTable("DashboardUsersTable")]
    public class DashboardUser
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}