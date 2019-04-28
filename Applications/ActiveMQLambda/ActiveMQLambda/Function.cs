using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ActiveMQLambda
{
    public class Function
    {
        private readonly IAmazonS3 _s3Client = new AmazonS3Client("AKIAIHFEMFJJYZBQYYAA", "f/h+Ir4D+sWNPTACPz+9HDV3yb24jxwP6wA8Zi5M", RegionEndpoint.EUCentral1);

        private readonly string _brokerUri = "activemq:ssl://b-d517d345-559e-4c9c-b84a-8413f5aedbdd-1.mq.eu-central-1.amazonaws.com:61617?transport.acceptInvalidBrokerCert=true&wireFormat.maxInactivityDuration=0";

        private readonly string _username = "admin";

        private readonly string _password = "adminPassword";

        private readonly string _bucket = "stream.analysis.bucket";

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
        }

        public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
        {
            Uri brokerUri = new Uri(_brokerUri);
            IConnectionFactory factory = new ConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection(_username, _password);
            connection.Start();
            if (connection.IsStarted)
                context.Logger.LogLine("Connection established succesfully.");

            using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
            {
                foreach (var record in snsEvent.Records)
                {
                    var snsRecord = record.Sns;
                    context.Logger.LogLine($"[{record.EventSource} {snsRecord.Timestamp}] Message = {snsRecord.Message}");

                    SnsMessage deserializedMessage = JsonConvert.DeserializeObject<SnsMessage>(snsRecord.Message);

                    foreach (var queueName in deserializedMessage.Queues)
                    {
                        string originalContent = String.Empty;
                        string buffer = String.Empty;

                        using (IQueue queue = session.GetQueue(queueName))
                        {
                            using (IQueueBrowser browser = session.CreateBrowser(queue))
                            {
                                using (IMessageConsumer consumer = session.CreateConsumer(queue))
                                {
                                    foreach (object msg in browser)
                                    {
                                        IMessage message = consumer.Receive();
                                        ITextMessage txtMsg = message as ITextMessage;
                                        buffer += txtMsg.Text + "\n";
                                    }

                                    if (buffer.Equals(String.Empty))
                                    {
                                        context.Logger.LogLine("Empty buffer");
                                        continue;
                                    }
                                }
                            }
                        }

                        try
                        {
                            using (GetObjectResponse response = await _s3Client.GetObjectAsync(new GetObjectRequest()
                            {
                                BucketName = _bucket,
                                Key = deserializedMessage.UserId + "/" + queueName
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
                            context.Logger.LogLine(ex.Message);
                        }

                        try
                        {
                            await _s3Client.PutObjectAsync(new PutObjectRequest()
                            {
                                ContentBody = originalContent + buffer,
                                BucketName = _bucket,
                                Key = deserializedMessage.UserId + "/" + queueName
                            });
                        }
                        catch (AmazonS3Exception ex)
                        {
                            context.Logger.LogLine(ex.Message);
                        }
                    }
                }
            }
        }

        public class SnsMessage
        {
            public string UserId { get; set; }

            public List<string> Queues { get; set; }
        }
    }
}
