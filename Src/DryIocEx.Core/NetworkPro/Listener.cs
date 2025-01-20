using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.NetworkPro;

public interface IListenOption : IOption
{
}

public class ListenOption : IListenOption
{
    public string Ip { get; set; }

    public int Port { get; set; }

    /// <summary>
    ///     指定流 Socket 是否正在使用 Nagle 算法
    /// </summary>
    /// <remarks>
    ///     Nagle 算法旨在通过使套接字缓冲小数据包，然后在特定情况下将它们合并并发送到一个数据包，从而减少网络流量。 TCP 数据包包含40字节的标头以及要发送的数据。 当使用 TCP 发送小型数据包时，TCP
    ///     标头产生的开销可能会成为网络流量的重要部分。 在负载较重的网络上，由于这种开销导致的拥塞会导致丢失数据报和重新传输，以及拥塞导致的传播时间过大。 如果在连接上以前传输的数据保持未确认的情况，则 Nagle 算法将禁止发送新的
    ///     TCP 段.大多数网络应用程序都应使用 Nagle 算法。(UDP) 套接字在用户数据报协议上设置此属性将不起作用。
    /// </remarks>
    public bool NoDelay { get; set; }

    /// <summary>
    ///     挂起连接队列的最大长度
    /// </summary>
    public int BackLog { set; get; } = 100;

    public EnumSessionKey SessionKey { set; get; } = EnumSessionKey.RemoteEndPoint;
}

public interface IListener<TPackage>
{
    public delegate ValueTask NewSessionHandle(ISession<TPackage> session);

    ValueTask<bool> StartAsync();
    ValueTask StopAsync();
    event NewSessionHandle EventNewSession;
}

public class Listener<TPackage> : IListener<TPackage>
{
    private readonly IContainer _container;
    protected CancellationTokenSource _ctsKeepAccept;
    protected bool _isStart;
    protected Socket _listenSocket;
    private readonly ILogManager _logManager;
    private readonly ListenOption _option;

    public Listener(IContainer container)
    {
        throw new NotImplementedException();
    }

    public event IListener<TPackage>.NewSessionHandle EventNewSession;

    public ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    protected bool ValidOption(ListenOption option)
    {
        throw new NotImplementedException();
    }

    protected IPEndPoint GetEndPoint(string ip, int port)
    {
        throw new NotImplementedException();
    }

    public async ValueTask KeepAccept(Socket socket)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    private object GetSessionKey(Socket socket)
    {
        throw new NotImplementedException();
    }

    private ISession<TPackage> Create(Socket socket)
    {
        throw new NotImplementedException();
    }
}

public class UdpListener<TPackage> : IListener<TPackage>
{
    private readonly IContainer _container;
    protected CancellationTokenSource _ctsKeepAccept;
    protected bool _isStart;
    protected Socket _listenSocket;
    private readonly ILogManager _logManager;
    private readonly ListenOption _option;

    private readonly List<ISession<TPackage>> _sessions;

    public UdpListener(IContainer container)
    {
        throw new NotImplementedException();
    }

    public event IListener<TPackage>.NewSessionHandle EventNewSession;

    public ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    protected bool ValidOption(ListenOption option)
    {
        throw new NotImplementedException();
    }

    protected IPEndPoint GetEndPoint(string ip, int port)
    {
        throw new NotImplementedException();
    }

    public async ValueTask KeepAccept(Socket socket)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    private object GetSessionKey(EndPoint endpoint)
    {
        throw new NotImplementedException();
    }

    private ISession<TPackage> Create(Socket socket, IPEndPoint ipendpoint, object key)
    {
        throw new NotImplementedException();
    }
}