using System;
using System.Reactive.Linq;

namespace Apixu
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var factory = new StreamAnalysisConnectionFactory();

            Console.WriteLine("Started connecting to broker...");
            try
            {
                using (IStreamAnalysisConnection connection = factory.CreateConnection())
                {
                    connection.Start();
                    if (connection.IsStarted)
                        Console.WriteLine("Connection established succesfully.");
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession())
                    {
                        Console.WriteLine("Sending data...");
                        //var topicMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        //{
                        //    var weather = ApixuService.GetWeatherDataByAutoIP();
                        //    return new TopicMessage()
                        //    {
                        //        Topic = "b3e4cf74252c495f93f48f02d98aa16b-topic1",
                        //        Value = weather.current.temp_c.ToString(),
                        //        Measurement = "°C",
                        //        Icon = weather.current.condition.icon
                        //    };
                        //});
                        //session.StreamData(topicMessage);

                        var queueMessage = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new QueueMessage()
                            {
                                Queue = "b3e4cf74252c495f93f48f02d98aa16b-queue33",
                                Value = weather.current.temp_c.ToString(),
                                Measurement = "°C",
                                Icon = weather.current.condition.icon,
                                TimestampEpoch = weather.current.last_updated_epoch
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