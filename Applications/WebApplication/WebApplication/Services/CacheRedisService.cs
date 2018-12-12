using System.Text;

namespace WebApplication.Services
{
    public class CacheRedisService
    {
        public void StoreSession()
        {
            var bytes = Encoding.UTF8.GetBytes("World");
        }
    }
}