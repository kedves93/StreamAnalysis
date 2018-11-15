﻿using StreamAnalysisLibrary;
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
            using (IStreamAnalysisConnection connection = factory.CreateConnection())
            {
                connection.Start();
                Console.WriteLine(connection.IsStarted);
                using (IStreamAnalysisSession session = connection.CreateStreamingSession())
                {
                    var weather = Observable.Interval(TimeSpan.FromSeconds(5)).Select(x => ApixuService.GetWeatherDataByAutoIP());
                    weather.Subscribe(x =>
                    {
                        Console.WriteLine("Sending data...");
                        session.SendData(x.current.last_updated + "  |  " + x.current.temp_c);
                    });
                }
            }

            Console.ReadKey();
        }
    }
}