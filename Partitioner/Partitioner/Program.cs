namespace Partitioner
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            // Generate a large list of integers
            List<int> numbers = Enumerable.Range(1, 10000).ToList();

            // Sequential Sum
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long sequentialSum = numbers.Sum();
            stopwatch.Stop();
            Console.WriteLine($"Sequential Sum: {sequentialSum}, Time: {stopwatch.ElapsedMilliseconds} ms");

            // Parallel Sum with Partitioner
            stopwatch.Restart();
            long parallelSum = 0;

            var rangePartitioner = Partitioner.Create(0, numbers.Count);

            Parallel.ForEach(rangePartitioner, (range, loopState) =>
            {
                long localSum = 0;
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    localSum += numbers[i];
                }

                // Use a lock to safely update the shared variable
                Interlocked.Add(ref parallelSum, localSum);
            });
            Console.WriteLine($"Parallel Sum with Partitioner: {parallelSum}, Time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();


            var parallelSumImproved = rangePartitioner
                .AsParallel()
                .Sum(range =>
                {
                    long localSum = 0;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        localSum += numbers[i];
                    }
                    return localSum;
                });

            stopwatch.Stop();
            Console.WriteLine($"Parallel Sum with Partitioner improved: {parallelSumImproved}, Time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

}
