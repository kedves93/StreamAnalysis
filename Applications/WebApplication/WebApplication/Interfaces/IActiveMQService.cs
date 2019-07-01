using System;
using static WebApplication.Services.ActiveMQService;

namespace WebApplication.Interfaces
{
    public interface IActiveMQService
    {
        event EventHandler<OnMessageEventArgs> OnTopicMessage;

        void StartListeningOnTopic(ITopicClient caller, string topicName);

        void StopListeningOnTopic(string topicName);
    }
}