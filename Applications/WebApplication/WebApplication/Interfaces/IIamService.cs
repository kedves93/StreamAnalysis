using System.Threading.Tasks;

namespace WebApplication.Interfaces
{
    public interface IIamService
    {
        Task<string> GetRoleArnFromNameAsync(string roleName);
    }
}