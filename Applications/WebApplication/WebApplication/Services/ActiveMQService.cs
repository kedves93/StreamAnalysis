using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using WebApplication.Hubs;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ActiveMQService : IActiveMQService
    {
        private readonly IConnection _connection;

        private readonly ILogger _logger;

        private readonly IHubContext<TopicsHub> _hubContext;

        public event EventHandler<OnMessageEventArgs> OnTopicMessage;

        public Dictionary<string, CancellationTokenSource> _topicCancellationTokens;

        public Dictionary<string, Thread> _threads;

        public ActiveMQService(IConfiguration configuration, IHubContext<TopicsHub> hubContext, ILogger<ActiveMQService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            _topicCancellationTokens = new Dictionary<string, CancellationTokenSource>();

            Uri brokerUri = new Uri(configuration.GetConnectionString("ActiveMQ"));
            IConnectionFactory factory = new ConnectionFactory(brokerUri);
            try
            {
                _connection = factory.CreateConnection(configuration.GetSection("ActiveMQ").GetSection("Username").Value, configuration.GetSection("ActiveMQ").GetSection("Password").Value);
            }
            catch (NMSConnectionException ex)
            {
                _logger.LogError(ex.Message);
                return;
            }

            _connection.ExceptionListener += exception => _logger.LogError(exception.Message);
            _connection.Start();
            if (_connection.IsStarted)
                _logger.LogInformation("Connection with broker successfully started. Id: " + _connection.ClientId);
            else
                _logger.LogError("Connection with broker could not be started.");
        }

        public void StartListeningOnTopic(ITopicClient caller, string topicName)
        {
            _topicCancellationTokens[topicName] = new CancellationTokenSource();
            var token = _topicCancellationTokens[topicName].Token;

            OnTopicMessage += async (sender, e) => await caller.OnNewMessageArrived(e.Message);

            Thread thread = new Thread(() =>
            {
                using (ISession session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                {
                    using (ITopic topic = session.GetTopic(topicName))
                    {
                        using (IMessageConsumer consumer = session.CreateConsumer(topic))
                        {
                            while (!token.IsCancellationRequested)
                            {
                                IMessage message = consumer.Receive();
                                ConsumeMessages(message);
                            }
                        }
                    }
                }
            });
            thread.Start();
        }

        public void StopListeningOnTopic(string topicName)
        {
            _topicCancellationTokens[topicName].Cancel();
            _topicCancellationTokens[topicName].Dispose();
        }

        public class OnMessageEventArgs : EventArgs
        {
            public TopicMessage Message { get; set; }
        }

        private void ConsumeMessages(IMessage message)
        {
            var msg = message as ITextMessage;

            XmlSerializer serializer = new XmlSerializer(typeof(TopicMessage));
            using (TextReader reader = new StringReader(msg.Text))
            {
                var topicMessage = serializer.Deserialize(reader) as TopicMessage;
                OnMessageEventArgs args = new OnMessageEventArgs
                {
                    Message = topicMessage
                };
                OnTopicMessage?.Invoke(this, args);
            }
        }
    }
}