#if NET


using System;
using System.Runtime.CompilerServices;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Network
{
    public static class NetworkExtensions
    {
        public static void CheckMustNeed<TPackage>(this IServer server)
        {
            throw new NotImplementedException();
        }

        public static void CheckMustNeed<TPackage>(this IClient client)
        {
            throw new NotImplementedException();
        }


        public static bool HasN<T>(this IServer server, string name = "")
        {
            throw new NotImplementedException();
        }

        public static bool HasN<T>(this IClient client, string name = "")
        {
            throw new NotImplementedException();
        }


        public static T GetNetwork<T>(this IServer server, string name = "")
        {
            throw new NotImplementedException();
        }

        public static T GetNetwork<T>(this IClient client, string name = "")
        {
            throw new NotImplementedException();
        }

        public static T GetNetwork<T>(this INetworkContainer container, string name = "")
        {
            throw new NotImplementedException();
        }

        public static void LogError(this INetworkContainer container, Exception ex, [CallerMemberName] string key = "",
            [CallerLineNumber] int num = 0)
        {
            throw new NotImplementedException();
        }

        public static void LogError(this INetworkContainer container, string text, [CallerMemberName] string key = "",
            [CallerLineNumber] int num = 0)
        {
            throw new NotImplementedException();
        }

        public static void LogDebug(this INetworkContainer container, string text, [CallerMemberName] string key = "",
            [CallerLineNumber] int num = 0)
        {
            throw new NotImplementedException();
        }

        public static void LogWarn(this INetworkContainer container, string text, [CallerMemberName] string key = "",
            [CallerLineNumber] int num = 0)
        {
            throw new NotImplementedException();
        }

        public static void LogInfo(this INetworkContainer container, string text, [CallerMemberName] string key = "",
            [CallerLineNumber] int num = 0)
        {
            throw new NotImplementedException();
        }
    }
}
#endif