using Amazon.DynamoDBv2.DataModel;

namespace StreamAnalysisWebApp.Models
{
    [DynamoDBTable("UsersTable")]
    public class User
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}