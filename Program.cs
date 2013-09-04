using System;
using System.Diagnostics;
using System.Linq;

namespace XPathSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var configProvider = new ConfigProvider();
            var xPathNodeCounter = new XPathNodeCounter(configProvider);
            Stopwatch sw = Stopwatch.StartNew();
            const int count = 100;
            for (int i = 0; i < count; i++)
            {
                IOrderedEnumerable<WordCount> valueCountsOrdered = xPathNodeCounter.GetNodeValueCountsOrdered();
                foreach (WordCount wordCount in valueCountsOrdered)
                {
                    Console.WriteLine("Word: {0}, Count: {1}", wordCount.Key, wordCount.Count);
                }
            }
            Console.WriteLine("Elapsed: {0}", sw.ElapsedMilliseconds / (double)count);
        }
    }
}
