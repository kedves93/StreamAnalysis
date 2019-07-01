using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApplication.Interfaces;

namespace WebApplication.Hubs
{
    public class TopicsHub : Hub<ITopicClient>
    {
        private readonly IActiveMQService _activeMQService;

        private readonly ILogger _logger;

        public TopicsHub(IActiveMQService activeMQService, ILogger<TopicsHub> logger)
        {
            _activeMQService = activeMQService;
            _logger = logger;
        }

        public void StartRealTimeMessagesFromTopic(string topic)
        {
            _activeMQService.StartListeningOnTopic(Clients.Caller, topic);
        }

        public void StopRealTimeMessagesFromTopic(string topic)
        {
            _activeMQService.StopListeningOnTopic(topic);
        }

        public override async Task OnConnectedAsync()
        {
            await Task.Run(() => _logger.LogInformation("Connection with hub started. Id: " + Context.ConnectionId));
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Task.Run(() => _logger.LogError("Hub connection disconnected."));
        }
    }
}