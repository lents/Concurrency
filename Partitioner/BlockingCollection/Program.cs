using System.Collections.Concurrent;

class Program
{
    static async Task Main(string[] args)
    {
        // Define the BlockingCollection with a bounded capacity of 100
        BlockingCollection<string> urlQueue = new BlockingCollection<string>(100);

        // Define a list of URLs to be scraped
        List<string> urls = new List<string>
        {
            "https://example.com",
            "https://example.org",
            "https://example.net",
            // Add more URLs as needed
        };

        // Start the producer task
        Task producerTask = Task.Run(() => Producer(urlQueue, urls));

        // Start the consumer tasks
        Task[] consumerTasks = new Task[4]; // Four consumer threads
        for (int i = 0; i < consumerTasks.Length; i++)
        {
            consumerTasks[i] = Task.Run(() => Consumer(urlQueue));
        }

        // Wait for all tasks to complete
        await producerTask;
        urlQueue.CompleteAdding(); // Signal that no more items will be added
        await Task.WhenAll(consumerTasks);

        Console.WriteLine("All tasks completed.");
    }

    static void Producer(BlockingCollection<string> urlQueue, List<string> urls)
    {
        foreach (var url in urls)
        {
            urlQueue.Add(url);
            Console.WriteLine($"Produced: {url}");
        }
    }

    static void Consumer(BlockingCollection<string> urlQueue)
    {
        HttpClient client = new HttpClient();

        foreach (var url in urlQueue.GetConsumingEnumerable())
        {
            try
            {
                string content = client.GetStringAsync(url).Result;
                Console.WriteLine($"Consumed: {url} with content length: {content.Length}");
                // Simulate processing the content
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {url}: {ex.Message}");
            }
        }
    }
}
