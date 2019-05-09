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
                        var message = Observable.Interval(TimeSpan.FromSeconds(8)).Select(x =>
                        {
                            var weather = ApixuService.GetWeatherDataByAutoIP();
                            return new TopicMessage()
                            {
                                Topic = "b3e4cf74252c495f93f48f02d98aa16b-topic1",
                                Value = weather.current.temp_c.ToString(),
                                Measurement = "°C",
                                Icon = weather.current.condition.icon
                            };
                        });
                        session.StreamData(message);
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