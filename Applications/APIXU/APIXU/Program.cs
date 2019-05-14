using System;
using System.Reactive.Linq;

namespace Apixu
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string queue1 = "queue://b3e4cf74252c495f93f48f02d98aa16b-temperatureQueue";
            const string queue2 = "queue://b3e4cf74252c495f93f48f02d98aa16b-windQueue";
            const string topic1 = "topic://b3e4cf74252c495f93f48f02d98aa16b-temperatureTopic";
            const string topic2 = "topic://b3e4cf74252c495f93f48f02d98aa16b-windTopic";

            var factory = new StreamAnalysisConnectionFactory();

            Console.WriteLine("Started connecting to broker...");
            try
            {
                using (IStreamAnalysisConnection connection = factory.CreateConnection())
                {
                    connection.Start();
                    if (connection.IsStarted)
                        Console.WriteLine("Connection established succesfully.");

                    // queue1
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession(queue1))
                    {
                        Console.WriteLine("Sending data to " + queue1);
                        var queueMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new QueueMessage()
                            {
                                Queue = queue1.Split("://")[1],
                                Value = weather.current.temp_c.ToString(),
                                Measurement = "°C",
                                TimestampEpoch = weather.current.last_updated_epoch
                            };
                        });
                        session.StreamData(queueMessage);
                    }

                    // queue2
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession(queue2))
                    {
                        Console.WriteLine("Sending data to " + queue2);
                        var queueMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new QueueMessage()
                            {
                                Queue = queue2.Split("://")[1],
                                Value = weather.current.wind_kph.ToString(),
                                Measurement = "km/h",
                                TimestampEpoch = weather.current.last_updated_epoch
                            };
                        });
                        session.StreamData(queueMessage);
                    }

                    // topic1
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession(topic1))
                    {
                        Console.WriteLine("Sending data to " + topic1);
                        var queueMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new TopicMessage()
                            {
                                Topic = topic1.Split("://")[1],
                                Value = weather.current.temp_c.ToString(),
                                Measurement = "°C",
                                Icon = weather.current.condition.icon
                            };
                        });
                        session.StreamData(queueMessage);
                    }

                    // topic2
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession(topic2))
                    {
                        Console.WriteLine("Sending data to " + topic2);
                        var queueMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new TopicMessage()
                            {
                                Topic = topic2.Split("://")[1],
                                Value = weather.current.wind_kph.ToString(),
                                Measurement = "km/h",
                                Icon = weather.current.condition.icon
                            };
                        });
                        session.StreamData(queueMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}