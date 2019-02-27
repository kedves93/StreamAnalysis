using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Apixu
{
    public class StreamAnalysisConnectionFactory
    {
        /// <summary>
        /// Creates a new connection to Stream Analysis.
        /// </summary>
        /// <returns></returns>
        public IStreamAnalysisConnection CreateConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            while (true)
            {
                try
                {
                    Uri brokerUri = new Uri(configuration.GetConnectionString("ActiveMQ"));
                    IConnectionFactory factory = new ConnectionFactory(brokerUri);
                    IConnection connection = factory.CreateConnection(configuration.GetSection("ActiveMQ").GetSection("Username").Value, configuration.GetSection("ActiveMQ").GetSection("Password").Value);
                    connection.ExceptionListener += exception => Console.WriteLine(exception);
                    connection.Start();
                    if (connection.IsStarted)
                        return new StreamAnalysisConnection(connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection to the AWS Broker failed: {ex.Message}.");
                    //throw new StreamAnalysisConnectionException($"Connection to the AWS Broker failed: {ex.Message}.");
                }
            }
        }
    }
}