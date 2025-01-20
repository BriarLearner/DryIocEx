﻿using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Log;
using DryIocEx.Core.NetworkPro;

namespace DryIocEx.Core.HISPro;


public class HISPackageBuilder<Self>
    where Self : HISPackageBuilder<Self>
{
    protected HISPackage _collection;

    public HISPackageBuilder()
    {
    }

    public HISPackageBuilder(HISPackage collection)
    {
        throw new NotImplementedException();
    }

    public virtual Self Initial(HISPackage package)
    {
        throw new NotImplementedException();
    }

    public Self Set<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public Self SetCmd(string str)
    {
        throw new NotImplementedException();
    }

    public HISPackage Build()
    {
        throw new NotImplementedException();
    }
}


public class DefaultPackageBuilder : HISPackageBuilder<DefaultPackageBuilder>
{
    public DefaultPackageBuilder(HISPackage collection) : base(collection)
    {
    }
}

/// <summary>
///     创建Package比较复杂，所以Builder模式创建PackageBuilder
/// </summary>
public struct HISPackageBuilder
{
    private readonly HISPackage _collection;

    public HISPackageBuilder(HISPackage package)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder Set<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder Set(Action<HISPackage> action)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder SetCmd(string str)
    {
        throw new NotImplementedException();
    }

    public HISPackage Build()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     因为HISPackage在通讯层大量复用，所以使用Pool模式
/// </summary>
public class HISPackagePool : Pool<HISPackage>
{
    public override void Return(HISPackage sub)
    {
        throw new NotImplementedException();
    }

    public override HISPackage Rent()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     内部使用Pool复用
///     所以调用完成要使用Dispose
/// </summary>
public class HISPackage : IDisposable
{
    private static int staticid;

    public readonly KeyValueCollection Data;


    private int disflag;

    /// <summary>
    ///     不需要用户调用
    /// </summary>
    public HISPackage()
    {
        throw new NotImplementedException();
    }

    internal HISPackage(KeyValueCollection collection)
    {
        throw new NotImplementedException();
    }


    internal static IPool<HISPackage> Pool { get; } = new HISPackagePool();
    public bool Disposable { get; private set; }

    /// <summary>
    ///     内部回收机制
    ///     不调用Dispose不会复用，由GC回收
    /// </summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public HISPackage Clone()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     不要外部调用
    /// </summary>
    internal void Initial()
    {
        throw new NotImplementedException();
    }

    public static HISPackageBuilder Create(string cmd = "")
    {
        throw new NotImplementedException();
    }

   

    #region 属性

    /// <summary>
    ///     发送次数
    /// </summary>
    public int SendCount
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public DateTime NotifyTime
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     版本
    /// </summary>
    public int Version
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     id
    ///     2024-03-18:由uint改成int,因为Interlocked.Increment在Framework中不支持uint
    /// </summary>
    public int Id
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     uid
    /// </summary>
    public string Uid
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int Sid
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int SendId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int WorkId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int BufferId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     优先级
    /// </summary>
    public CommandPriority Priority
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     获取该指令的名称。
    /// </summary>
    public string Cmd
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public DateTime CreateTime
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public bool IsAck => throw new NotImplementedException();

    public bool IsNak => throw new NotImplementedException();

    public bool IsHeartBeat => throw new NotImplementedException();

    public bool IsWorkCmd => throw new NotImplementedException();

    #endregion
}

public static class HISPackageExtension
{
    public static IEnumerable<KeyValuePair<string, string>> GetAll(this HISPackage package)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Remove(this HISPackage package, string key)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Add<T>(this HISPackage package, string key, T value)
    {
        throw new NotImplementedException();
    }

    public static bool TryGet<T>(this HISPackage package, string key, out T value)
    {
        throw new NotImplementedException();
    }

    public static T Get<T>(this HISPackage package, string key)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Clone(this HISPackage packge)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     指令的优先级。
/// </summary>
public enum CommandPriority : byte
{
    /// <summary>
    ///     高优先级。
    /// </summary>
    High = 10,

    /// <summary>
    ///     普通优先级。
    /// </summary>
    Normal = 20,

    /// <summary>
    ///     低优先级。
    /// </summary>
    Low = 30
}

public class HISPackageFilter : IPackageFilter<HISPackage>
{
    public const byte STX = 0x02;

    public const byte ETX = 0x03;

    public const byte GS = 0x1D;

    public const byte RS = 0x1E;

    public ReadOnlyMemory<byte> Converter(HISPackage pack)
    {
        throw new NotImplementedException();
    }

    public HISPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
    }

    private HISPackage Decoder(ref ReadOnlySequence<byte> buffer, object context)
    {
        throw new NotImplementedException();
    }

    private void ParseKeyValue(ref ReadOnlySequence<byte> bytes, ref HISPackageBuilder builder)
    {
        throw new NotImplementedException();
    }
}

public class
    HISServerBuilder : TcpServerBuilder<HISPackage, HISServerBuilder>
{
    public HISServerBuilder(IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }
}

public class HISClientBuilder : TcpClientBuilder<HISPackage, HISClientBuilder>
{
    public HISClientBuilder(IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }
}

public class HISUDPServerBuilder : UdpServerBuilder<HISPackage, HISUDPServerBuilder>
{
    public HISUDPServerBuilder(IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }
}

public class HISUDPClientBuilder : UdpClientBuilder<HISPackage, HISUDPClientBuilder>
{
    public HISUDPClientBuilder(IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }
}

public static class HISExtension
{
    /// <summary>
    ///     ip和端口都比较
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="dis"></param>
    /// <returns></returns>
    public static bool IPEndPointFullCompare(IPEndPoint ori, IPEndPoint dis)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     只比较ip
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="dis"></param>
    /// <returns></returns>
    public static bool IPEndPointIpAddressComare(IPEndPoint ori, IPEndPoint dis)
    {
        throw new NotImplementedException();
    }

}

public interface IHISCommunicationServer : IDisposable
{
    /// <summary>
    ///     处理命令
    /// </summary>
    /// <param name="session"></param>
    /// <param name="package"></param>
    /// <returns></returns>
    internal ValueTask HandleReceiveCmd(ISession<HISPackage> session, HISPackage package);

    /// <summary>
    ///     处理未知消息
    /// </summary>
    event Action<IPEndPoint, HISPackage> HandleUnknownAction;

    /// <summary>
    ///     处理全部消息，在返回Ack之前，请不要有任何阻塞
    /// </summary>
    event Action<IPEndPoint, HISPackage> HandleAllAction;

    event Action<IPEndPoint, ISession<HISPackage>, bool> SessionChanged;

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

    IHISCommunicationServer RegisterCmdHandler(string cmdstr, Action<IPEndPoint, HISPackage> action);

    /// <summary>
    ///     全Assembly扫描
    /// </summary>
    /// <param name="assembly"></param>
    void RegisterAssembly(Assembly assembly);

    
}


/// <summary>
///     业务层通讯服务
/// </summary>
public class HISCommunicationServer : IHISCommunicationServer
{
    /// <summary>
    ///     所有等待发送指令字典
    /// </summary>
    private readonly ConcurrentDictionary<int, HISPackageCacheItem> CacheDict = new();


    /// <summary>
    ///     保存所有指令的处理方法
    ///     通过string来识别和注入
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
    private readonly ConcurrentDictionary<int, HISSendCmdWrapper> SendWaitAckCmd = new();


    private readonly ConcurrentDictionary<IPEndPoint, (string, ISession<HISPackage>)> SessionCache = new();


    private List<IReceiveFilter> Filters = new()
    {
        new HeartCmdFilter(),
        new WorkCmdFilter(),
        new AckCmdFilter()
    };


    public event Action<IPEndPoint, HISPackage> HandleUnknownAction;


    public event Action<IPEndPoint, HISPackage> HandleAllAction;


    void IHISCommunicationServer.RegisterSession(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    public event Action<IPEndPoint, ISession<HISPackage>, bool> SessionChanged;


    void IHISCommunicationServer.UnregisterSession(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    async ValueTask IHISCommunicationServer.HandleReceiveCmd(ISession<HISPackage> session, HISPackage package)
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

    public IHISCommunicationServer RegisterCmdHandler(string cmdstr, Action<IPEndPoint, HISPackage> action)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     通过注入程序集来识别方法
    /// </summary>
    /// <param name="assembly"></param>
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
    ///     如果没有指定endpoint，所有连接都会发送
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


    private void OnHandleAllAction(IPEndPoint ipendpoint, HISPackage package)
    {
        throw new NotImplementedException();
    }

    private void OnHandleUnknownAction(IPEndPoint ipendpoint, HISPackage package)
    {
        throw new NotImplementedException();
    }

    private void OnSessionChanged(IPEndPoint ipendpoint, ISession<HISPackage> session, bool isconnected)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     发送业务核心
    /// </summary>
    /// <param name="session"></param>
    /// <param name="endpoint"></param>
    /// <param name="package"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    private async ValueTask<EnumHISCmdState> InnerSendCmd(ISession<HISPackage> session, IPEndPoint endpoint,
        HISPackage package, int timeout)
    {
        throw new NotImplementedException();
    }


   
    /// <summary>
    ///     内部异步处理包
    ///     用于处理Hope接收的包
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

public interface IHISClientHeartMiddleware : IMiddleware<HISPackage>
{
}

public interface IHISHeartMiddleware : IMiddleware<HISPackage>
{
}

/// <summary>
///     每隔两秒发送一次心跳电文
/// </summary>
public class HISHeartMiddleware : IHISHeartMiddleware
{
    public int Order => 0;

    public ValueTask Register(ISession<HISPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask UnRegister(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }


    public ValueTask Handle(ISession<HISPackage> session, HISPackage package)
    {
        throw new NotImplementedException();
    }
}

public interface IHISCMDMiddleware : IMiddleware<HISPackage>
{
}

public class HISCMDMiddleware : IHISCMDMiddleware
{
    public HISCMDMiddleware(IHISCommunicationServer server)
    {
        throw new NotImplementedException();
    }

    public IHISCommunicationServer CommunicationServerServer { get; }

    public int Order => 0;

    public ValueTask Register(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }

    public ValueTask UnRegister(ISession<HISPackage> channel)
    {
        throw new NotImplementedException();
    }


    public ValueTask Handle(ISession<HISPackage> session, HISPackage package)
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

/// <summary>
///     状态包装
/// </summary>
internal class HISCmdStateStateWrapper
{
    public EnumHISCmdState State { set; get; }
}

/// <summary>
///     希望接收命令包装
/// </summary>
internal class HISHopeReceiveCmdWrapper
{
    public string HopeCmd { set; get; }

    public HISPackage ReceivePackage { set; get; }
}

public class HISPackageCacheItem
{
    private static int id;

    private readonly Action<HISPackageCacheItem> callback;

    private readonly TaskCompletionSource<bool> TCS;

    public HISPackageCacheItem(HISPackage package, IPEndPoint endpoint, Func<IPEndPoint, IPEndPoint, bool> compare,
        int cachetime, Action<HISPackageCacheItem> completedcallback)
    {
        throw new NotImplementedException();
    }

    public int ID { get; }
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

/// <summary>
///     发送命令包装，包装发送命令，TCS，状态，
/// </summary>
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
}