using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IDynamoDBService
    {
        Task<bool> ValidateUserAsync(SignInUserInfo user);

        Task<bool> RegisterUserAsync(RegisterUserInfo user);
    }
}