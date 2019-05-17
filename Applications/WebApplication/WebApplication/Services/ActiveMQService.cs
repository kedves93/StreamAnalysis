using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebApplication.Interfaces;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ActiveMQService : IActiveMQService
    {
        private readonly IConnection _connection;

        private readonly ILogger _logger;

        public event EventHandler<OnMessageEventArgs> OnTopicMessage;

        public Dictionary<string, CancellationTokenSource> _topicCancellationTokens;

        public ActiveMQService(IConfiguration configuration, ILogger<ActiveMQService> logger)
        {
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

        public async Task StartListeningOnTopicAsync(string topicName)
        {
            _topicCancellationTokens[topicName] = new CancellationTokenSource();
            var token = _topicCancellationTokens[topicName].Token;

            await Task.Run(() =>
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
            }, token);
        }

        public async Task StopListeningOnTopicAsync(string topicName)
        {
            await Task.Run(() =>
            {
                _topicCancellationTokens[topicName].Cancel();
                _topicCancellationTokens[topicName].Dispose();
            });
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