using System.Diagnostics;

class Program
{
    // Define the range and workload size for demonstration
    private const int RangeStart = 1;
    private const int RangeEnd = 1_000_000; 
    private const int NumProcesses = 4;     // Number of processes to use

    static void Main(string[] args)
    {
        Console.WriteLine("Synchronous Execution:");
        ExecuteSynchronously();

        Console.WriteLine("\nParallel Execution using Threads:");
        ExecuteUsingThreads();

        Console.WriteLine("\nParallel Execution using Tasks:");
        ExecuteUsingTasks();

        Console.WriteLine("\nParallel Execution using Processes:");
        ExecuteUsingProcesses();

        Console.ReadLine();
    }

    // 1. Synchronous Execution
    static void ExecuteSynchronously()
    {
        var stopwatch = Stopwatch.StartNew();

        long result = CalculateSumOfSquares(RangeStart, RangeEnd);

        stopwatch.Stop();
        Console.WriteLine($"Synchronous result: {result}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    // Helper function to calculate the sum of squares for a given range
    static long CalculateSumOfSquares(int start, int end)
    {
        long sum = 0;
        for (int i = start; i <= end; i++)
        {
            sum += (long)i * i;
        }
        return sum;
    }

    // 2. Parallel Execution using Threads
    static void ExecuteUsingThreads()
    {
        var stopwatch = Stopwatch.StartNew();

        long totalSum = 0;
        int rangePerThread = (RangeEnd - RangeStart + 1) / NumProcesses;
        Thread[] threads = new Thread[NumProcesses];
        long[] results = new long[NumProcesses];

        // Create and start threads
        for (int i = 0; i < NumProcesses; i++)
        {
            int threadIndex = i;
            int start = RangeStart + threadIndex * rangePerThread;
            int end = (threadIndex == NumProcesses - 1) ? RangeEnd : start + rangePerThread - 1;

            threads[i] = new Thread(() =>
            {
                results[threadIndex] = CalculateSumOfSquares(start, end);
            });
            threads[i].Start();
        }

        // Wait for all threads to complete
        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Sum results from all threads
        foreach (var result in results)
        {
            totalSum += result;
        }

        stopwatch.Stop();
        Console.WriteLine($"Threads result: {totalSum}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    // 3. Parallel Execution using Tasks
    static void ExecuteUsingTasks()
    {
        var stopwatch = Stopwatch.StartNew();

        int rangePerTask = (RangeEnd - RangeStart + 1) / NumProcesses;
        Task<long>[] tasks = new Task<long>[NumProcesses];

        // Create and start tasks
        for (int i = 0; i < NumProcesses; i++)
        {
            int taskIndex = i;
            int start = RangeStart + taskIndex * rangePerTask;
            int end = (taskIndex == NumProcesses - 1) ? RangeEnd : start + rangePerTask - 1;

            tasks[i] = Task.Run(() => CalculateSumOfSquares(start, end));
        }

        // Wait for all tasks to complete and sum results
        Task.WaitAll(tasks);
        long totalSum = 0;
        foreach (var task in tasks)
        {
            totalSum += task.Result;
        }

        stopwatch.Stop();
        Console.WriteLine($"Tasks result: {totalSum}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

    // 4. Parallel Execution using Processes
    static void ExecuteUsingProcesses()
    {
        var stopwatch = Stopwatch.StartNew();
        int rangePerProcess = (RangeEnd - RangeStart + 1) / NumProcesses;
        string[] tempFiles = new string[NumProcesses];
        long totalSum = 0;
        string processDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Calc.dll");

        if (!File.Exists(processDllPath))
        {
            Console.WriteLine($"Error: The DLL '{processDllPath}' was not found.");
            return;
        }

        for (int i = 0; i < NumProcesses; i++)
        {
            int start = RangeStart + i * rangePerProcess;
            int end = (i == NumProcesses - 1) ? RangeEnd : start + rangePerProcess - 1;

            // Create a temporary file for each process to write results
            tempFiles[i] = Path.GetTempFileName();

            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"\"{processDllPath}\" {start} {end} {tempFiles[i]}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            try
            {
                process.Start();

                // Read standard output and error output
                string output = process.StandardOutput.ReadToEnd().Trim();
                string error = process.StandardError.ReadToEnd().Trim();
                process.WaitForExit();

                Console.WriteLine($"Process {i} output: {output}"); // Display standard output
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine($"Process {i} error: {error}"); // Display error output if any
                }

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Process {i} exited with code {process.ExitCode}. Check error output for details.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting process {i}: {ex.Message}");
            }
        }

        // Read results from temporary files
        foreach (var file in tempFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    // Read the result file using UTF-8 encoding
                    string result = File.ReadAllText(file, System.Text.Encoding.UTF8).Trim();
                    totalSum += long.Parse(result);
                    File.Delete(file); // Clean up temporary file
                }
                else
                {
                    Console.WriteLine($"File not found: {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {file}: {ex.Message}");
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Processes result: {totalSum}");
        Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }

}
