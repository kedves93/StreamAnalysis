using StreamAnalysisWebApp.Models;
using System.Threading.Tasks;

namespace StreamAnalysisWebApp.Services
{
    public interface IDynamoDBService
    {
        Task<bool> ValidateUser(SignInUserInfo user);
    }
}