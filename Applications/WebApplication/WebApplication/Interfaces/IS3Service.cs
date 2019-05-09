using System.Threading.Tasks;

namespace WebApplication.Interfaces
{
    public interface IS3Service
    {
        Task<string> GetFromQueueAsync(string queueName);
    }
}