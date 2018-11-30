using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;

namespace StreamAnalysisMQListener
{
    internal class Program
    {
        private const string PROTOCOL = "ssl";
        private const string HOST_NAME = "b-4959ef54-a5ac-4a87-971d-a17e199f1c54-1.mq.eu-central-1.amazonaws.com";
        private const int PORT = 61617;

        private const string ACTIVE_MQ_USERNAME = "admin";
        private const string ACTIVE_MQ_PASSWORD = "adminPassword";

        private const string DESTINATION = "queue://StreamAnalysisQueue";

        private static void Main(string[] args)
        {
            Console.WriteLine("Started connecting to broker....");
            Uri brokerUri = new Uri($"activemq:{PROTOCOL}://{HOST_NAME}:{PORT}?transport.acceptInvalidBrokerCert=true");
            IConnectionFactory factory = new ConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection(ACTIVE_MQ_USERNAME, ACTIVE_MQ_PASSWORD);
            connection.Start();
            if (connection.IsStarted)
                Console.WriteLine("Connection established succesfully.");

            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetDestination(DESTINATION);
            IMessageConsumer consumer = session.CreateConsumer(destination);

            Console.WriteLine("Waiting for messages...");
            while (true)
            {
                IMessage message = consumer.Receive();
                ITextMessage txtMsg = message as ITextMessage;
                Console.WriteLine(txtMsg.Text);
            }
        }
    }
}