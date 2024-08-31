using System.Collections.Concurrent;

class Program
{
    static void Main(string[] args)
    {
        // Create a ConcurrentBag to store tasks
        ConcurrentBag<string> tasks = new ConcurrentBag<string>();

        // Add tasks to the bag
        for (int i = 1; i <= 10; i++)
        {
            tasks.Add($"Task {i}");
        }

        // Define the number of worker threads
        int workerCount = 4;

        // Create and start worker threads
        Task[] workers = new Task[workerCount];
        for (int i = 0; i < workerCount; i++)
        {
            workers[i] = Task.Run(() => ProcessTasks(tasks));
        }

        // Wait for all worker threads to complete
        Task.WaitAll(workers);

        Console.WriteLine("All tasks have been processed.");
    }

    static void ProcessTasks(ConcurrentBag<string> tasks)
    {
        while (!tasks.IsEmpty)
        {
            if (tasks.TryTake(out string task))
            {
                Console.WriteLine($"{Task.CurrentId} is processing {task}");
                // Simulate task processing
                Thread.Sleep(500);
            }
        }
    }
}
