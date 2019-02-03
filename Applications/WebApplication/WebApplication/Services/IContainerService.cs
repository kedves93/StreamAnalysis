using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IContainerService
    {
        Task CreateRepositoryAsync(Repository repository);

        Task ConfigureImageAsync(ImageConfiguration config);

        Task RunImageAsync(ImageConfigurationName name);
    }
}