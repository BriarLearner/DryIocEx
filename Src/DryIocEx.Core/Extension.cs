using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Log;

namespace DryIocEx.Core;

public static class Extension
{
    public static T To<T>(this object obj)
    {
        throw new NotImplementedException();
    }

    // Handles IPv4 and IPv6 notation.
    private static IPEndPoint CreateIPEndPoint(string endPoint)
    {
        throw new NotImplementedException();
    }

    public static void DoNotAwait(this ValueTask<bool> task)
    {
    }

    internal static void LogInfo<TLogger>(this string str, [CallerMemberName] string name = null)
        where TLogger : ILogger
    {
        throw new NotImplementedException();
    }

    internal static void LogError<TLogger>(this Exception ex, [CallerMemberName] string name = null)
        where TLogger : ILogger
    {
        throw new NotImplementedException();
    }

    internal static void LogError<TLogger>(this string error, [CallerMemberName] string name = null)
        where TLogger : ILogger
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     不会检测父类，在类中不同位置的初始化，
    /// </summary>
    /// <param name="obj"></param>
    public static void InnerInitial<T>(this T obj) where T : class
    {
        throw new NotImplementedException();
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class InnerInitialAttribute : Attribute
    {
    }
}