using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
    public interface IS3Service
    {
        Task<List<QueueMessage>> GetDataFromQueueAsync(string queue);
    }
}