using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Interfaces
{
    /// <summary>
    /// SignalR uses RPC (Remote Procedure Call) which means that methods called on the server are executed on the client side.
    /// This interface strongly types those client side methods.
    /// </summary>
    public interface ITopicClient
    {
        /// <summary>
        /// The implementation of this method is on client side (RPC).
        /// </summary>
        /// <param name="topicMessage"></param>
        /// <returns></returns>
        Task OnNewMessageArrived(TopicMessage topicMessage);
    }
}