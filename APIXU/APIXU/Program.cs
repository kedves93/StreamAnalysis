using StreamAnalysisLibrary;
using System;
using System.Reactive.Linq;

namespace APIXU
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var factory = new StreamAnalysisConnectionFactory();

            Console.WriteLine("Establishing connection...");
            try
            {
                using (IStreamAnalysisConnection connection = factory.CreateConnection())
                {
                    connection.Start();
                    Console.WriteLine(connection.IsStarted);
                    using (IStreamAnalysisSession session = connection.CreateStreamingSession())
                    {
                        Console.WriteLine("Sending data...");
                        var weather = Observable.Interval(TimeSpan.FromSeconds(5)).Select(x => ApixuService.GetWeatherDataByAutoIP());
                        session.StreamData(weather);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}