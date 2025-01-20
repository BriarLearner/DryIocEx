using System;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Log;

public static class LogExtension
{
    public static void LogError(this string message, string keyword = "")
    {
        throw new NotImplementedException();
    }

    public static void LogInfo(this string message, string keyword = "")
    {
        throw new NotImplementedException();
    }

    public static void LogDebug(this string message, string keyword = "")
    {
        throw new NotImplementedException();
    }


    public static void LogError(this Exception exception, string keyword = "")
    {
        throw new NotImplementedException();
    }
}

public interface ISGLogger : ILogger
{
}