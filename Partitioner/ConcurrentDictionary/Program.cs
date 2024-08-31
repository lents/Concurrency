using System.Collections.Concurrent;

public class SessionCache
{
    private ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

    // Add or update a session value
    public void AddOrUpdateSession(string sessionId, string value)
    {
        cache.AddOrUpdate(sessionId, value, (key, oldValue) => value);
        Console.WriteLine($"Session {sessionId} added/updated with value: {value}");
    }

    // Get a session value
    public string GetSession(string sessionId)
    {
        if (cache.TryGetValue(sessionId, out string value))
        {
            Console.WriteLine($"Session {sessionId} retrieved with value: {value}");
            return value;
        }
        else
        {
            Console.WriteLine($"Session {sessionId} not found.");
            return null;
        }
    }

    // Remove a session value
    public bool RemoveSession(string sessionId)
    {
        if (cache.TryRemove(sessionId, out string value))
        {
            Console.WriteLine($"Session {sessionId} removed.");
            return true;
        }
        else
        {
            Console.WriteLine($"Failed to remove session {sessionId}.");
            return false;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        SessionCache sessionCache = new SessionCache();

        // Simulate concurrent access with multiple threads
        Parallel.Invoke(
            () => sessionCache.AddOrUpdateSession("session1", "data1"),
            () => sessionCache.AddOrUpdateSession("session2", "data2"),
            () => Console.WriteLine(sessionCache.GetSession("session1")),
            () => Console.WriteLine(sessionCache.GetSession("session2")),
            () => sessionCache.RemoveSession("session1"),
            () => Console.WriteLine(sessionCache.GetSession("session1"))
        );

        Console.WriteLine("All operations completed.");
    }
}
