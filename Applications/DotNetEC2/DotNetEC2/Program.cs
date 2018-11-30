using System;
using System.Reactive.Linq;

namespace DotNetEC2
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => Console.WriteLine(x));
            Console.ReadKey();
        }
    }
}