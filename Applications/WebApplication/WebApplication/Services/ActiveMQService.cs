using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly Dictionary<string, ISession> _sessions;

        public event EventHandler<OnMessageEventArgs> OnTopicMessage;

        public ActiveMQService(IConfiguration configuration, ILogger<ActiveMQService> logger)
        {
            _logger = logger;
            _sessions = new Dictionary<string, ISession>();

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
            await Task.Run(() =>
            {
                ISession session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                _sessions[topicName] = session;
                ITopic topic = session.GetTopic(topicName);
                IMessageConsumer consumer = session.CreateConsumer(topic);
                consumer.Listener += new MessageListener(ConsumeMessages);
            });
        }

        public async Task StopListeningOnTopicAsync(string topicName)
        {
            await Task.Run(() => _sessions[topicName]);
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