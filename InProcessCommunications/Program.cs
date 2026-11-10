using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;


public sealed class StopwatchScope : IDisposable
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private readonly string _label;
    private readonly Action<string> _log;

    public StopwatchScope(string label, Action<string> log = null)
    {
        _label = label;
        _log = log ?? (msg => Console.WriteLine(msg));
    }

    public TimeSpan Elapsed => _sw.Elapsed;

    public void Dispose()
    {
        _sw.Stop();
        _log($"| {_label} | {_sw.Elapsed} |");
    }
}
class Program
{
    static void Main()
    {
        var arrays = new List<int[]>();

        arrays.Add(getArray(100000));
        arrays.Add(getArray(1000000));
        arrays.Add(getArray(10000000));
        PrintEnvironment();
        foreach(var arr in arrays)
        {
            Console.WriteLine($"## {arr.Length}");
            Console.WriteLine($"|Тип|Stopwatch.Elapsed|");
            Console.WriteLine($"|---|---|");
            using (new StopwatchScope("LINQ")) { arr.Sum(x => (long)x); }
            using (new StopwatchScope("simple foreach")) { CalculateSum(arr); }
            using (new StopwatchScope("parallel List<Thread>")) { CalculateSumParallel(arr); }
            using (new StopwatchScope("PLINQ")) { arr.AsParallel().Sum(x => (long)x); }
        }
    }

    public static void PrintEnvironment()
    {
        Console.WriteLine($"## Environment");
        Console.WriteLine($"OS: {RuntimeInformation.OSDescription} \\");
        Console.WriteLine($"Framework: {Environment.Version} \\");
        Console.WriteLine($"Processors: {Environment.ProcessorCount} \\");
        Console.WriteLine($"Machine Name: {Environment.MachineName} \\");
        Console.WriteLine($"64-bit Process: {Environment.Is64BitProcess} \\");
        Console.WriteLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
    }
    static int[] getArray(int size)
    {
        int[] result = new int[size];

        for(int i = 0; i < size; i++)
        {
            result[i] = i;
        }

        return result; 
    }

    static long CalculateSum(int[] array)
    {
        long sum = 0;
        foreach (int number in array)
        {
            sum += number;
        }
        return sum;
    }
    static long CalculateSumParallel(int[] array)
    {
        int threadCount = Environment.ProcessorCount;

        var ranges = SplitArray(array.Length, threadCount);

        long sum = 0;

        var threads = new List<Thread>();
        var lsums = new long[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int index = i;
            Thread t = new Thread(() =>
            {
                long lsum = 0;
                var range = ranges[index];
                for (int j = range.Start; j < range.End; j++)
                {
                    lsum += array[j];
                }
                lsums[index] = lsum;
            });
            threads.Add(t);
            t.Start();
        }

        foreach(var t in threads)
        {
            t.Join();
        }
        foreach(var s in lsums)
        {
            sum += s;
        }

        return sum;
    }
    static List<(int Start, int End)> SplitArray(int length, int count)
    {
        var result = new List<(int Start, int End)>();

        var bSize = length / count;
        var rem = length % count;

        int start = 0;
        for (int i =0; i< count; i++)
        {
            int size = bSize + (i < rem ? 1 : 0);
            int end = start + size;
            result.Add((start,end));
            start = end;
        }

        return result;
    }
}