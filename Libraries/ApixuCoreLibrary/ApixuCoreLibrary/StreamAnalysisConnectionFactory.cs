using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;

namespace ApixuCoreLibrary
{
    public class StreamAnalysisConnectionFactory
    {
        private const string PROTOCOL = "ssl";
        private const string HOST_NAME = "b-d517d345-559e-4c9c-b84a-8413f5aedbdd-1.mq.eu-central-1.amazonaws.com";
        private const int PORT = 61617;

        private const string ACTIVE_MQ_USERNAME = "admin";
        private const string ACTIVE_MQ_PASSWORD = "adminPassword";

        /// <summary>
        /// Creates a new connection to Stream Analysis.
        /// </summary>
        /// <returns></returns>
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