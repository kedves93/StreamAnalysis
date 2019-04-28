using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reactive.Linq;

namespace StreamAnalysisMQListener
{
    internal static class Program
    {
        private const string DESTINATION = "queue://StreamAnalysisQueue";
        private static IAmazonS3 _s3Client;

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("aws_credentials.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string accessKey = configuration.GetSection("AwsDevCredentials").GetSection("AwsAccessKeyId").Value;
            string secretKey = configuration.GetSection("AwsDevCredentials").GetSection("AwsSecretAccessKey").Value;
            _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUCentral1);

            Console.WriteLine("Started connecting to broker...");
            Uri brokerUri = new Uri(configuration.GetConnectionString("ActiveMQ"));
            IConnectionFactory factory = new ConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection(configuration.GetSection("ActiveMQ").GetSection("Username").Value, configuration.GetSection("ActiveMQ").GetSection("Password").Value);
            connection.Start();
            if (connection.IsStarted)
                Console.WriteLine("Connection established succesfully.");

            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetDestination(DESTINATION);
            IMessageConsumer consumer = session.CreateConsumer(destination);

            var obs = Observable.Interval(TimeSpan.FromSeconds(5));
            obs.Subscribe(onNext: async x =>
            {
                Console.WriteLine("Flushing messages into S3 bucket.");

                //IMessage message = consumer.Receive();
                //ITextMessage txtMsg = message as ITextMessage;
                //Console.WriteLine(txtMsg.Text);

                string originalContent = String.Empty;
                string newMessage = "NEW MESSAGE";

                try
                {
                    using (GetObjectResponse response = await _s3Client.GetObjectAsync(new GetObjectRequest()
                    {
                        BucketName = configuration.GetSection("Bucket").Value,
                        Key = configuration.GetSection("ActiveMQ").GetSection("Username").Value
                    }))
                    {
                        using (StreamReader reader = new StreamReader(response.ResponseStream))
                        {
                            originalContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    await _s3Client.PutObjectAsync(new PutObjectRequest()
                    {
                        ContentBody = originalContent + newMessage,
                        BucketName = configuration.GetSection("Bucket").Value,
                        Key = configuration.GetSection("ActiveMQ").GetSection("Username").Value
                    });
                }
                catch (AmazonS3Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            Console.ReadKey();
        }
    }
}