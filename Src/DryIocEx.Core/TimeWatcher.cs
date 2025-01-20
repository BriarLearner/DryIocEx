using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DryIocEx.Core;

public class TimeWatcher
{
    [Flags]
    public enum DuplicateOptions : uint
    {
        DUPLICATE_CLOSE_SOURCE =
            0x00000001, // Closes the source handle. This occurs regardless of any error status returned.

        DUPLICATE_SAME_ACCESS =
            0x00000002 // Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
    }

    private readonly int[] gcCollectionCounts;
    private readonly int[] gcStartCollectionCounts;

    //private long cpuSpanElapsed=0;
    private readonly Stopwatch watch = new();
    private uint runCount = 1;

    private long timeElapsed;

    public TimeWatcher()
    {
        gcCollectionCounts = new int[GC.MaxGeneration + 1];
        gcStartCollectionCounts = new int[GC.MaxGeneration + 1];
    }

    public long TimeElapsed { get; private set; }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime,
        out long lpKernelTime, out long lpUserTime);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);


    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
        IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle,
        uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    public static void RaisePriority()
    {
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
    }


    public static long GetCurrentThreadTimes()
    {
        var processHandle = GetCurrentProcess();
        DuplicateHandle(processHandle, GetCurrentThread(), processHandle, out var _threadHandle, 0, false,
            (uint)DuplicateOptions.DUPLICATE_SAME_ACCESS);
        GetThreadTimes(_threadHandle, out var ct, out var et, out var kernelTime, out var userTimer);
        return kernelTime + userTimer;
    }

    public static ulong GetCurrentThreadCycleTime()
    {
        try
        {
            ulong count = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref count);
            return count;
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    //private ulong cycleTime = 0;
    public void Start()
    {
        GC.Collect(GC.MaxGeneration);
        timeElapsed = 0;

        runCount = 1;

        for (var gccount = 0; gccount <= GC.MaxGeneration; gccount++)
            gcStartCollectionCounts[gccount] = GC.CollectionCount(gccount);
        watch.Restart();
    }

    public void Stop(uint count)
    {
        watch.Stop();
        TimeElapsed = timeElapsed = watch.ElapsedMilliseconds;
        for (var gccount = 0; gccount <= GC.MaxGeneration; gccount++)
            gcCollectionCounts[gccount] = GC.CollectionCount(gccount) - gcStartCollectionCounts[gccount];
        runCount = count;
    }

    public string GetStatistics(IWatcherDescripter descripter)
    {
        return descripter.Descriptor(timeElapsed, gcCollectionCounts, runCount);
    }

    /// <summary>
    ///     返回从开始到结束的毫秒数
    /// </summary>
    /// <returns></returns>
    public long Stop()
    {
        Stop(1);
        return TimeElapsed;
    }
}

public class TimeWatcherDescriptor : IWatcherDescripter
{
    public string Descriptor(long timeelapsed, int[] gccount, uint count = 1)
    {
        var sb = new StringBuilder();
        sb.Append($"[Time Elasped: {timeelapsed:F} ms]");
        if (count > 1) sb.Append($"[Each Time Elasped: {timeelapsed / count:F} ms]");
        for (var i = 0; i <= GC.MaxGeneration; i++) sb.Append($"[Gen {i}: {gccount[i]}]");

        sb.Append(Environment.NewLine);
        return sb.ToString();
    }
}

public class ZhTimeWatcherDescriptor : IWatcherDescripter
{
    public string Descriptor(long timeelapsed, int[] gccount, uint count = 1)
    {
        var sb = new StringBuilder();
        sb.Append($"[总耗时: {timeelapsed:F} ms]");
        if (count > 1) sb.Append($"[平均耗时: {timeelapsed / count:F} ms]");
        for (var i = 0; i <= GC.MaxGeneration; i++) sb.Append($"[Gen {i}: {gccount[i]}]");

        sb.Append(Environment.NewLine);
        return sb.ToString();
    }
}

public interface IWatcherDescripter
{
    string Descriptor(long timeelapsed, int[] gccount, uint count = 1);
}