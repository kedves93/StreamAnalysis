using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface ISnsService
    {
        Task SendNotificationToFlushAsync(UserQueues queues);
    }
}