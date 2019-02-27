namespace WebApplication.Services
{
    public interface IRedisService
    {
        void StoreSession(string userName);

        bool CheckSession();

        void DeleteSession();
    }
}