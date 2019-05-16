using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class IamService : IIamService
    {
        private readonly IAmazonIdentityManagementService _iamClient;

        public IamService(IOptions<AwsDevCredentials> credentials)
        {
            string accessKey = credentials.Value.AwsAccessKeyId;
            string secretKey = credentials.Value.AwsSecretAccessKey;
            _iamClient = new AmazonIdentityManagementServiceClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
        }

        public async Task<string> GetRoleArnFromNameAsync(string roleName)
        {
            try
            {
                var response = await _iamClient.GetRoleAsync(new GetRoleRequest()
                {
                    RoleName = roleName
                });
                return response.Role.Arn;
            }
            catch (AmazonIdentityManagementServiceException)
            {
                throw;
            }
        }
    }
}