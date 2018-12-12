using WebApplication.Models;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IDynamoDBService
    {
        Task<bool> ValidateUser(SignInUserInfo user);
    }
}