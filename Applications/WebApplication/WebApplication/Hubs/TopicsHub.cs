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

        public async Task StartRealTimeMessagesFromTopic(string topic)
        {
            _activeMQService.OnTopicMessage += async (sender, e) => await Clients.Caller.OnNewMessageArrived(e.Message);
            await _activeMQService.StartListeningOnTopicAsync(topic);
        }

        public async Task StopRealTimeMessagesFromTopic(string topic)
        {
            await _activeMQService.StopListeningOnTopicAsync(topic);
        }

        public override async Task OnConnectedAsync()
        {
            await Task.Run(() => _logger.LogInformation("Connection with hub started. Id: " + Context.ConnectionId));
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Task.Run(() => _logger.LogError("Hub connection disconnected. Error: " + ex.Message));
        }
    }
}