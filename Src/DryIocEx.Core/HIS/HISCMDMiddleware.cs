#if NET
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Log;
using DryIocEx.Core.Network;

namespace DryIocEx.Core.HIS;

public static class HISExtension
{
    public static bool IPEndPointFullCompare(IPEndPoint ori, IPEndPoint dis)
    {
        throw new NotImplementedException();
    }

    public static bool IPEndPointIpAddressComare(IPEndPoint ori, IPEndPoint dis)
    {
        throw new NotImplementedException();
    }


    public static EndPoint GetEndpoint<TPackage>(this IClient<TPackage> client)
    {
        throw new NotImplementedException();
    }

    public static EndPoint GetEndpoint<TPackage>(this ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    public static IPEndPoint GetIpEndPoint<TPackage>(this ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }


    public static EndPoint GetEndpoint<TPackage>(this IChannel<TPackage> channel)
    {
        throw new NotImplementedException();
    }
}

public interface IHISCommunicationServer : IDisposable
{
    /// <summary>
    ///     处理接收的所有Packages
    ///     不要进行耗时操作，没有异步
    /// </summary>
    /// <param name="handler"></param>
    void HandleReceivePackages(Action<IPEndPoint, HISPackage> handler);

    /// <summary>
    ///     处理命令
    /// </summary>
    /// <param name="session"></param>
    /// <param name="package"></param>
    /// <returns></returns>
    internal ValueTask HandleReceiveCmd(ISession<HISPackage> session, HISPackage package);

    /// <summary>
    ///     处理未知命令
    /// </summary>
    /// <param name="handler"></param>
    public void HandleUnknownCmd(Action<IPEndPoint, HISPackage> handler);

    /// <summary>
    ///     发送命令，如果没有节点就缓存，等待节点连接后再发送。因为连接后端口已经改变，所以只需要提供Ip地址
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="package"></param>
    /// <returns></returns>
    ValueTask<EnumHISCmdState> SendCmdIPCache(HISPackage package, string ip = "", int timeout = 5000,
        int cahcetime = 1 * 3600 * 1000, Action<HISPackage, bool> cacheissend = null);

    /// <summary>
    ///     对应Ip上所有端口对象都会发送
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="package"></param>
    /// <returns></returns>
    ValueTask<EnumHISCmdState> SendCmdIP(HISPackage package, string ip = "", int timeout = 5000);

    ValueTask<EnumHISCmdState> SendCmdCache(HISPackage package, IPEndPoint endpoint = null, int timeout = 5000,
        int cahcetime = 1 * 3600 * 1000, Func<IPEndPoint, IPEndPoint, bool> compare = null,
        Action<HISPackage, bool> cacheissend = null);

    ValueTask<ValueTuple<EnumHISCmdState, HISPackage>> SendHopeCmd(HISPackage package, string hopecmd,
        IPEndPoint endpoint = null, int timeout = 10000, Func<IPEndPoint, IPEndPoint, bool> compare = null);

    /// <summary>
    ///     发送接收希望命令不需要ACK
    /// </summary>
    /// <param name="package"></param>
    /// <param name="hopecmd"></param>
    /// <param name="endpoint"></param>
    /// <param name="timeout"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    ValueTask<ValueTuple<EnumHISCmdState, HISPackage>> SendHopeCmdNoACK(HISPackage package, string hopecmd,
        IPEndPoint endpoint = null, int timeout = 10000, Func<IPEndPoint, IPEndPoint, bool> compare = null);

    /// <summary>
    ///     发送指令
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="package"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    ValueTask<EnumHISCmdState> SendCmd(HISPackage package, IPEndPoint endpoint = null, int timeout = 5000,
        Func<IPEndPoint, IPEndPoint, bool> compare = null);

    internal void RegisterSession(ISession<HISPackage> session);
    internal void UnregisterSession(ISession<HISPackage> session);

    /// <summary>
    ///     注入ReceiveCmdHandler
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="type"></param>
    IHISCommunicationServer RegisterCmdHandlerType<Type>() where Type : IHISCmdHandler;

    /// <summary>
    ///     注入对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IHISCommunicationServer RegisterCmdHandler(IHISCmdHandler obj);


    IHISCommunicationServer RegisterCmdHandler(Action<IPEndPoint, HISPackage> action);


    /// <summary>
    ///     全Assembly扫描
    /// </summary>
    /// <param name="assembly"></param>
    void RegisterAssembly(Assembly assembly);

    
}

public class HISCommunicationBuilder
{
    internal HISCommunicationBuilder()
    {
    }

    public async ValueTask<IHISCommunicationServer> CreateClientAndReConnect(string connectip, int connectport,
        Action<IPEndPoint, EnumConnectState> connectchanged)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<IHISCommunicationServer> CreateUDPClientAndReConnect(string connectip, int connectport,
        Action<IPEndPoint, EnumConnectState> connectchanged)
    {
        throw new NotImplementedException();
    }


    public async ValueTask<IHISCommunicationServer> CreateServerAndListen(string ip, int port,
        Action<IPEndPoint, EnumConnectState> connectchanged)
    {
        throw new NotImplementedException();
    }


    public async ValueTask<IHISCommunicationServer> CreateUDPServerAndListen(int port,
        Action<IPEndPoint, EnumConnectState> connectchanged)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     业务层通讯服务
/// </summary>
public class HISCommunicationServer : IHISCommunicationServer
{
    /// <summary>
    ///     所有等待发送指令字典
    /// </summary>
    private readonly ConcurrentDictionary<uint, HISPackageCacheItem> CacheDict = new();


    /// <summary>
    ///     保存所有指令的处理方法
    /// </summary>
    private readonly Dictionary<string, HISCmdHandleReference> HandleReceiceCMDDict =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     保存所有等待期望目标指令
    /// </summary>
    private readonly ConcurrentDictionary<CmdEndpointWrapper, HISSendCmdWrapper> hopeDict = new();


    /// <summary>
    ///     保存所有等待ACK的指令
    /// </summary>
    private readonly Dictionary<uint, HISSendCmdWrapper> SendWaitAckCmd = new();


    private readonly ConcurrentDictionary<IPEndPoint, (string, ISession<HISPackage>)> SessionCache = new();


    private List<IReceiveFilter> Filters = new()
    {
        new HeartCmdFilter(),
        new WorkCmdFilter(),
        new AckCmdFilter()
    };

    internal HISCommunicationServer()
    {
    }

    public static HISCommunicationBuilder Builder { get; } = new();

    private Action<IPEndPoint, HISPackage> HandleUnknownAction { set; get; }


    private Action<IPEndPoint, HISPackage> HandleAllAction { set; get; }

    void IHISCommunicationServer.RegisterSession(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    void IHISCommunicationServer.UnregisterSession(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    async ValueTask IHISCommunicationServer.HandleReceiveCmd(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }

    public void HandleUnknownCmd(Action<IPEndPoint, HISPackage> handler)
    {
        throw new NotImplementedException();
    }

    public void HandleReceivePackages(Action<IPEndPoint, HISPackage> handler)
    {
        throw new NotImplementedException();
    }


    public IHISCommunicationServer RegisterCmdHandlerType<Type>()
        where Type : IHISCmdHandler
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer RegisterCmdHandler(IHISCmdHandler obj)
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer RegisterCmdHandler(Action<IPEndPoint, HISPackage> action)
    {
        throw new NotImplementedException();
    }

    public void RegisterAssembly(Assembly assembly)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     找到对应IP地址的端口发送
    /// </summary>
    /// <param name="package"></param>
    /// <param name="ip"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async ValueTask<EnumHISCmdState> SendCmdIP(HISPackage package, string ip = "", int timeout = 5000)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<EnumHISCmdState> SendCmdIPCache(HISPackage package, string ip = "", int timeout = 5000,
        int cachetime = 1 * 3600 * 1000, Action<HISPackage, bool> cacheissend = null)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     发送数据，如果没有连接则缓存，默认缓存时间1小时
    /// </summary>
    /// <param name="package"></param>
    /// <param name="cachetime"></param>
    /// <param name="endpoint"></param>
    /// <param name="timeout"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    public async ValueTask<EnumHISCmdState> SendCmdCache(HISPackage package, IPEndPoint endpoint = null,
        int timeout = 5000, int cachetime = 1 * 3600 * 1000, Func<IPEndPoint, IPEndPoint, bool> compare = null,
        Action<HISPackage, bool> cacheissend = null)
    {
        throw new NotImplementedException();
    }


    public async ValueTask<ValueTuple<EnumHISCmdState, HISPackage>> SendHopeCmdNoACK(HISPackage package, string hopecmd,
        IPEndPoint endpoint = null, int timeout = 10000, Func<IPEndPoint, IPEndPoint, bool> compare = null)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<ValueTuple<EnumHISCmdState, HISPackage>> SendHopeCmd(HISPackage package, string hopecmd,
        IPEndPoint endpoint = null, int timeout = 10000, Func<IPEndPoint, IPEndPoint, bool> compare = null)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     Endpoint=null,全部发送
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="package"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async ValueTask<EnumHISCmdState> SendCmd(HISPackage package, IPEndPoint endpoint = null, int timeout = 5000,
        Func<IPEndPoint, IPEndPoint, bool> compare = null)
    {
        throw new NotImplementedException();
    }


    

    public async void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     内部异步处理包
    /// </summary>
    /// <param name="session"></param>
    /// <param name="package"></param>
    /// <param name="endpoint"></param>
    private void InnerHandlePackage(ISession<HISPackage> session, HISPackage package, IPEndPoint endpoint)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Type must IHISCmdHandler and HISCmdAttribute
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    private void RegisterCmdHandlerType(Type type)
    {
        throw new NotImplementedException();
    }


    private async ValueTask<EnumHISCmdState> InnerSendCmd(ISession<HISPackage> session, IPEndPoint endpoint,
        HISPackage package, int timeout)
    {
        throw new NotImplementedException();
    }

    private void OnRemoveCacheItem(HISPackageCacheItem item)
    {
        throw new NotImplementedException();
    }
}

internal struct CmdEndpointWrapper : IEquatable<CmdEndpointWrapper>
{
    public string Cmd { get; }

    public IPEndPoint Endpoint { get; }

    public CmdEndpointWrapper(string cmd, IPEndPoint endpoint)
    {
        throw new NotImplementedException();
    }

    public bool Equals(CmdEndpointWrapper other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(CmdEndpointWrapper left, CmdEndpointWrapper right)
    {
        throw new NotImplementedException();
    }

    public static bool operator !=(CmdEndpointWrapper left, CmdEndpointWrapper right)
    {
        throw new NotImplementedException();
    }
}

internal class HoldSendCmd
{
}

internal class HISCmdHandleReference
{
    public HISCmdHandleReference(Type handleType, IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }

    public HISCmdHandleReference(IHISCmdHandler handle, IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }

    public HISCmdHandleReference(Action<IPEndPoint, HISPackage> handleaction, IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }


    public HISCmdHandleReference(HISCmdHandleReference reference)
    {
        throw new NotImplementedException();
    }

    public Type HandleType { get; }

    private IHISCommunicationServer Server { get; }


    private Action<IPEndPoint, HISPackage> HandleAction { get; }

    private IHISCmdHandler handler { get; set; }


    public HISCmdHandleReference NextReference { set; get; }


    public Action<IPEndPoint, HISPackage> GetHandle()
    {
        throw new NotImplementedException();
    }
}

public interface IReceiveFilter
{
    /// <summary>
    ///     是否需要异步
    /// </summary>
    bool NeedAsync { get; }

    /// <summary>
    ///     处理包
    /// </summary>
    /// <param name="session"></param>
    /// <param name="package"></param>
    /// <returns>true是过滤不再处理，false继续处理</returns>
    ValueTask<bool> FilterPackages(ISession<HISPackage> session, HISPackage package);
}

public class HeartCmdFilter : IReceiveFilter
{
    public bool NeedAsync { get; } = false;

    public ValueTask<bool> FilterPackages(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public class WorkCmdFilter : IReceiveFilter
{
    public bool NeedAsync { get; } = true;

    public ValueTask<bool> FilterPackages(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public static class HISPackageStore
{
    public static HISPackage CreateAck(HISPackage receivepackage)
    {
        throw new NotImplementedException();
    }


    public static HISPackage CreateHeart()
    {
        throw new NotImplementedException();
    }
}

public class AckCmdFilter : IReceiveFilter
{
    /// <summary>
    ///     是否需要异步
    ///     耗时操作
    /// </summary>
    public bool NeedAsync { get; } = true;

    /// <summary>
    /// </summary>
    /// <param name="session"></param>
    /// <param name="package"></param>
    /// <returns>是否已经过滤不需要进一步处理</returns>
    public ValueTask<bool> FilterPackages(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public interface IHISClientCMDMiddleware : IClientMiddleware<HISPackage>
{
}

public class HISClientCMDMiddleware : IHISClientCMDMiddleware
{
    public HISClientCMDMiddleware()
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer CommunicationServer { set; get; }
    public int Order { get; }
    public IClient Client { get; set; }

    public ValueTask<bool> Connected(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> Disconnected(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }
}

public interface IHISClientHeartMiddleware : IClientMiddleware<HISPackage>
{
}

/// <summary>
///     每隔两秒发送一次心跳电文
/// </summary>
public class HISClientHeartMiddleware : IHISClientHeartMiddleware
{
    private readonly ConcurrentDictionary<string, ISession<HISPackage>>
        sessions = new(StringComparer.OrdinalIgnoreCase);

    private int start;


    public int Order { get; }
    public IClient Client { get; set; }

    public ValueTask<bool> Connected(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> Disconnected(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    private async void OnSendHeartPackage(object state)
    {
        throw new NotImplementedException();
    }
}

public interface IHISHeartMiddleware : IServerMiddleware<HISPackage>
{
}

/// <summary>
///     每隔两秒发送一次心跳电文
/// </summary>
public class HISHeartMiddleware : BaseSessionMiddle<HISPackage>, IHISHeartMiddleware
{
    private readonly ConcurrentDictionary<string, ISession<HISPackage>> sessions = new();

    private int start;

    public override ValueTask<bool> Register(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> Unregister(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }


    public override ValueTask Handle(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }

    private async void OnSendHeartPackage(object state)
    {
        throw new NotImplementedException();
    }
}

public interface IHISCMDMiddleware : IServerMiddleware<HISPackage>
{
}

public class HISCMDMiddleware : BaseSessionMiddle<HISPackage>, IHISCMDMiddleware
{
    public HISCMDMiddleware()
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer CommunicationServerServer { get; }

    public override ValueTask<bool> Register(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> Unregister(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }


    public override async ValueTask Handle(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public interface IHISUDPMiddleware : IServerMiddleware<HISPackage>
{
}

public class HISUDPCMDMiddleware : BaseSessionMiddle<HISPackage>, IHISUDPMiddleware
{
    public HISUDPCMDMiddleware()
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer CommunicationServerServer { get; }

    public override ValueTask<bool> Register(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> Unregister(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }

    public override async ValueTask Handle(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HISCmdAttribute : Attribute
{
    public HISCmdAttribute(string cmd)
    {
        throw new NotImplementedException();
    }

    public string Cmd { get; }
}

public interface IHISCmdHandler
{
    public IHISCommunicationServer Server { get; set; }
    void Handle(IPEndPoint endpoint, HISPackage package);
}

public abstract class BaseHISCmdHandler : IHISCmdHandler
{
    public IHISCommunicationServer Server { get; set; }
    public abstract void Handle(IPEndPoint endpoint, HISPackage package);
}

internal class HISCmdStateStateWrapper
{
    public EnumHISCmdState State { set; get; }
}

internal class HISHopeReceiveCmdWrapper
{
    public string HopeCmd { set; get; }

    public HISPackage ReceivePackage { set; get; }
}

public class HISPackageCacheItem
{
    private static uint id;

    private readonly Action<HISPackageCacheItem> callback;


    private readonly TaskCompletionSource<bool> TCS;

    public HISPackageCacheItem(HISPackage package, IPEndPoint endpoint, Func<IPEndPoint, IPEndPoint, bool> compare,
        int cachetime, Action<HISPackageCacheItem> completedcallback)
    {
        throw new NotImplementedException();
    }

    public uint ID { get; }
    public DateTime StartTime { get; }

    public HISPackage Package { get; }


    public IPEndPoint Endpoint { get; }

    public Func<IPEndPoint, IPEndPoint, bool> Compare { get; }

    public bool IsSend { get; private set; }

    public void IsCompleted()
    {
        throw new NotImplementedException();
    }

    private async void InnerTimer(int cachetime)
    {
        throw new NotImplementedException();
    }
}

public struct HISSendCmdWrapper
{
    public IPEndPoint IpEndPoint { get; }

    public TaskCompletionSource<bool> TCS { get; }

    public HISPackage Package { get; }

    internal HISCmdStateStateWrapper State { get; }

    internal HISHopeReceiveCmdWrapper HopeReceive { get; }


    public void SetState(EnumHISCmdState state)
    {
        throw new NotImplementedException();
    }

    public HISSendCmdWrapper(IPEndPoint endpoint, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public enum EnumHISCmdState
{
    /// <summary>
    ///     初始值,不发送
    /// </summary>
    None = 0,

    /// <summary>
    ///     已发送在等待
    /// </summary>
    WaitAck,

    /// <summary>
    ///     接收到NAK
    /// </summary>
    NAK,

    /// <summary>
    ///     未收到
    /// </summary>
    TimeOut,

    /// <summary>
    ///     发送失败，原因未知
    /// </summary>
    Fail,

    /// <summary>
    ///     没有连接
    /// </summary>
    NotConnected,

    /// <summary>
    ///     没有连接，但是缓存了，等待连接
    /// </summary>
    Cache,

    /// <summary>
    ///     成功,
    ///     成功收到Ack或者收到返回值
    /// </summary>
    Success
    ///// <summary>
    ///// 一条指令成功发送
    ///// </summary>
    //WaitGetTimeOut,
}

#endif