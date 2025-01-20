using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Log;

public interface ILogInfoFormater<T>
{
    T FormatInfo(LogInfo value);

    LogInfo UnformatInfo(T data);
}

public static class LoggerExtension
{
    public static void Log(this ILogger logger, string keyword, string text, EnumLogDegree degree)
    {
        throw new NotImplementedException();
    }

    public static void LogInfo(this ILogger logger, string keyword, string text)
    {
        throw new NotImplementedException();
    }

    public static void LogWarn(this ILogger logger, string keyword, string text)
    {
        throw new NotImplementedException();
    }

    public static LogInfo ToLogInfo(this string text, EnumLogDegree degree, string keywords)
    {
        throw new NotImplementedException();
    }

    public static LogInfo ToLogInfo(this Exception exception, string keyword)
    {
        throw new NotImplementedException();
    }
}

public interface ILogger
{
    bool IsRunning { get; }
    void Log(LogInfo info);

    void Log(EnumLogDegree degree, string keywords, string text, int threadid);
}

public abstract class BaseLogger
{
    private readonly ConcurrentQueue<LogInfo> _logs = new();

    private int _status;

    public bool IsRunning => _status != 0;

    public void Log(EnumLogDegree degree, string keywords, string text, int threadid)
    {
        throw new NotImplementedException();
    }

    public void Log(LogInfo info)
    {
        throw new NotImplementedException();
    }

    protected virtual void StopProcessLogInfo()
    {
    }

    protected virtual void StartProcessLogInfo()
    {
    }

    protected abstract void ProcessLogInfo(LogInfo loginfo);
}