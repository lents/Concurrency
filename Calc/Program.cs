using System;
using System.IO;
namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: CalculateSumOfSquaresApp <start> <end> <outputFile>");
                return;
            }

            try
            {
                int start = int.Parse(args[0]);
                int end = int.Parse(args[1]);
                string outputFile = args[2];

                long sum = CalculateSumOfSquares(start, end);

                // Write the result to the specified file
                File.WriteAllText(outputFile, sum.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static long CalculateSumOfSquares(int start, int end)
        {
            long sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += (long)i * i;
            }
            return sum;
        }
    }

}
