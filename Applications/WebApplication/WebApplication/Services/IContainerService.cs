using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IContainerService
    {
        Task CreateRepositoryAsync(Repository repository);

        Task CreateConfiguration(ImageConfiguration config);

        Task RunImageAsync(string configName);
    }
}