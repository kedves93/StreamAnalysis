using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IDynamoDBService
    {
        Task<bool> ValidateUserAsync(SignInUserInfo user);

        Task<bool> RegisterUserAsync(RegisterUserInfo user);

        Task<string> GetUserIdFromUsernameAsync(string username);

        Task<List<string>> GetQueuesFromUserIdAsync(string userId);

        Task<List<string>> GetTopicsFromUserIdAsync(string userId);

        Task<bool> SaveUserChannelsAsync(UserChannels channels);

        Task<bool> DeleteChannelsFromUserIdAsync(string userId);
    }
}