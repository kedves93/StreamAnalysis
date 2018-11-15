using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;

namespace StreamAnalysisLibrary
{
    public class StreamAnalysisConnectionFactory
    {
        private const string PROTOCOL = "ssl";
        private const string HOST_NAME = "b-4959ef54-a5ac-4a87-971d-a17e199f1c54-1.mq.eu-central-1.amazonaws.com";
        private const int PORT = 61617;

        private const string ACTIVE_MQ_USERNAME = "admin";
        private const string ACTIVE_MQ_PASSWORD = "adminPassword";

        public IStreamAnalysisConnection CreateConnection()
        {
            try
            {
                Uri brokerUri = new Uri($"activemq:{PROTOCOL}://{HOST_NAME}:{PORT}?transport.acceptInvalidBrokerCert=true");
                IConnectionFactory factory = new ConnectionFactory(brokerUri);
                IConnection connection = factory.CreateConnection(ACTIVE_MQ_USERNAME, ACTIVE_MQ_PASSWORD);
                return new StreamAnalysisConnection(connection);
            }
            catch (Exception ex)
            {
                throw new StreamAnalysisConnectionException($"Connection to the AWS Broker failed: {ex.Message}.");
            }
        }
    }
}