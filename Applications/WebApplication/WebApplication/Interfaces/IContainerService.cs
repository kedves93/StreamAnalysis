using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IContainerService
    {
        Task CreateRepositoryAsync(Repository repository);

        Task<bool> CheckImageAsync(Models.Repository repository);

        Task CreateConfigurationAsync(ImageConfiguration config);

        Task RunImageAsync(string configName);

        Task RunScheduledImageAsync(ScheduledImageFixedRate scheduledImageFixedRate);

        Task RunScheduledImageAsync(ScheduledImageCronExpression scheduledImageCronExpression);

        Task StopScheduledImageAsync(string configName);
    }
}