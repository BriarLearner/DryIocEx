using System;
using System.Collections.Generic;
using System.Linq;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Log;

public interface ILoggerAccessor
{
    ILogger Logger { get; }
}

public static class LogLocator
{
    private static ILogManager _manager;

    public static ILogManager LogManager
    {
        get
        {
            throw new NotImplementedException();
        }
    }


    public static void SetLogManager(ILogManager manager)
    {
        throw new NotImplementedException();
    }
}

///// <summary>
/////     这个方式只是提供一个组件内的外观模式，
/////     如果使用者自定义了，需要手动new builder
///// </summary>
//public class LogBuilderStore
//{
//    /// <summary>
//    ///     空对象不需要每次创建对象
//    /// </summary>
//    public static NullLogBuilder NullLogBuilder = new();

//    public static FileLogBuilder FileLogBuilder => new();

//    public static ConsoleLogBuilder ConsoleLogBuilder => new();
//}

public interface ILoggerInner : ILoggerFile
{
}

public static class LogManagerExtension
{
    public static ILogger GetLogger<T>(this object obj) where T : ILogger
    {
        throw new NotImplementedException();
    }


    internal static void InnerLogDebug(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }

    internal static void InnerLogError(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }

    internal static void InnerLogInfo(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }

    public static void LogError<T>(this ILogManager manager, Exception ex, string keyword) where T : ILogger
    {
        throw new NotImplementedException();
    }

    public static void LogError<T>(this ILogManager manager, string text, string keyword) where T : ILogger
    {
        throw new NotImplementedException();
    }

    public static void GLogError(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }

    public static void LogInfo<T>(this ILogManager manager, string text, string keyword) where T : ILogger
    {
        throw new NotImplementedException();
    }

    public static void GLogInfo(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }

    public static void LogDebug<T>(this ILogManager manager, string text, string keyword) where T : ILogger
    {
        throw new NotImplementedException();
    }

    public static void GLogDebug(this ILogManager manager, string text, string keyword)
    {
        throw new NotImplementedException();
    }
}

public interface ILoggerBuilder
{
}



public interface ILogManager
{
    List<ILogger> Loggers { get; }

    ILogger GetLogger<T>();

    void BroadcastLog(LogInfo info);
    void Register<T>(ILogger logger);

    void Register<TLogger>(TLogger logger) where TLogger : ILogger;
    TLogger Get<TLogger>() where TLogger : ILogger;
}

[AutoRegister(typeof(ILogManager), EnumLifetime.Singleton)]
public class LogManager : ILogManager
{


    private readonly Dictionary<Type, ILogger> _loggers = new();


    public List<ILogger> Loggers => _loggers.Values.ToList();

    public void BroadcastLog(LogInfo info)
    {
        throw new NotImplementedException();
    }


    public ILogger GetLogger<T>()
    {
        throw new NotImplementedException();
    }

    public TLogger Get<TLogger>() where TLogger : ILogger
    {
        throw new NotImplementedException();
    }


    public void Register<T>(ILogger logger)
    {
        throw new NotImplementedException();
    }

    public void Register<TLogger>(TLogger logger) where TLogger : ILogger
    {
        throw new NotImplementedException();
    }
}

