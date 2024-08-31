using System.Collections.Concurrent;

public class Logger
{
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
    private AutoResetEvent logEvent = new AutoResetEvent(false);
    private bool isRunning = true;

    public Logger()
    {
        // Start the logging thread
        Task.Run(() => ProcessLogs());
    }

    // Method to add a log message
    public void Log(string message)
    {
        logQueue.Enqueue(message);
        logEvent.Set(); // Signal the logging thread
    }

    // Method to stop the logging thread
    public void Stop()
    {
        isRunning = false;
        logEvent.Set(); // Signal the logging thread to exit
    }

    // Method to process log messages
    private void ProcessLogs()
    {
        while (isRunning || !logQueue.IsEmpty)
        {
            logEvent.WaitOne(); // Wait for a signal

            while (logQueue.TryDequeue(out string logMessage))
            {
                // Simulate writing log message to a file or console
                Console.WriteLine($"Processed Log: {logMessage}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Logger logger = new Logger();

        // Simulate multiple threads generating log messages
        Parallel.For(0, 10, i =>
        {
            logger.Log($"Log message {i}");
        });

        // Give some time for processing logs
        Thread.Sleep(1000);

        // Stop the logger
        logger.Stop();

        Console.WriteLine("All log messages have been processed.");
    }
}
