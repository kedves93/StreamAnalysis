using System.Text;

namespace StreamAnalysisWebApp.Services
{
    public class CacheRedisService
    {
        public void StoreSession()
        {
            var bytes = Encoding.UTF8.GetBytes("World");
        }
    }
}