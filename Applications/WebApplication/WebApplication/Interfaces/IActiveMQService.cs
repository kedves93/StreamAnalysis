using System;
using System.Threading.Tasks;
using static WebApplication.Services.ActiveMQService;

namespace WebApplication.Interfaces
{
    public interface IActiveMQService
    {
        event EventHandler<OnMessageEventArgs> OnTopicMessage;

        Task StartListeningOnTopicAsync(string topicName);

        Task StopListeningOnTopicAsync(string topicName);
    }
}