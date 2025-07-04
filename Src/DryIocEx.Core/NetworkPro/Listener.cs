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
        _container = container;
        _option = container.Resolve<IListenOption>().As<ListenOption>();
        _logManager = container.Resolve<ILogManager>();
    }

    public event IListener<TPackage>.NewSessionHandle EventNewSession;

    public ValueTask<bool> StartAsync()
    {
        if (_isStart) return new ValueTask<bool>(true);
        try
        {
            var option = _option;
            if (!ValidOption(option))
                throw new ArgumentException(nameof(option));
            var listenaddress = GetEndPoint(option.Ip, option.Port);
            if (listenaddress == null) throw new ArgumentException(nameof(listenaddress));
            var listensocket = _listenSocket =
                new Socket(listenaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //SocketType https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.sockettype?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(System.Net.Sockets.SocketType)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0
            //ProtocolType https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.protocoltype?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(System.Net.Sockets.ProtocolType)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0
            listensocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            //SocketOptionLevel https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.socketoptionlevel?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(System.Net.Sockets.SocketOptionLevel)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0
            listensocket.LingerState = new LingerOption(false, 0);
            if (option.NoDelay) listensocket.NoDelay = true;
            listensocket.Bind(listenaddress);
            listensocket.Listen(option.BackLog);
            _ctsKeepAccept = new CancellationTokenSource();
            KeepAccept(listensocket).DoNotAwait();
        }
        catch (Exception e)
        {
            _logManager.BroadcastLog(e.ToLogInfo("listener start error"));
            return new ValueTask<bool>(false);
        }

        _isStart = true;
        return new ValueTask<bool>(true);
    }

    public ValueTask StopAsync()
    {
        if (!_isStart) return new ValueTask();
        var socket = _listenSocket;
        if (socket == null) return new ValueTask();
        _ctsKeepAccept.Cancel();
        socket.Close();
        socket.Dispose();
        _listenSocket = null;
        _ctsKeepAccept = null;
        _isStart = false;
        return new ValueTask();
    }

    protected bool ValidOption(ListenOption option)
    {
        if (option == null || string.IsNullOrEmpty(option.Ip) || option.Port < 0 || option.Port > 65535)
            return false;
        return true;
    }

    protected IPEndPoint GetEndPoint(string ip, int port)
    {
        if (string.IsNullOrEmpty(ip) || port == 0) return null;
        IPAddress ipaddress;
        if ("any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.Any;
        else if ("IpV6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.IPv6Any;
        else
            ipaddress = IPAddress.Parse(ip);
        return new IPEndPoint(ipaddress, port);
    }

    public async ValueTask KeepAccept(Socket socket)
    {
        while (!_ctsKeepAccept.IsCancellationRequested)
            try
            {
                var client = await socket.AcceptAsync().ConfigureAwait(false);
                var session = Create(client);
                OnNewSession(session).DoNotAwait();
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException || e is NullReferenceException) break;

                if (e is SocketException se)
                {
                    var errorcode = se.ErrorCode;
                    //ListenSocket 被关闭了
                    if (errorcode == 125 ||
                        errorcode == 89 ||
                        errorcode == 995 ||
                        errorcode == 10004 ||
                        errorcode == 10038)
                        break;
                }
            }
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        if (EventNewSession != null && session != null)
            try
            {
                await EventNewSession(session);
            }
            catch (Exception e)
            {
                _logManager.BroadcastLog(e.ToLogInfo("Listen onnewsession error"));
            }
    }

    private object GetSessionKey(Socket socket)
    {
        switch (_option.SessionKey)
        {
            case EnumSessionKey.LocalEndPoint:
                return socket.LocalEndPoint;
            case EnumSessionKey.RemoteEndPoint:
                return socket.RemoteEndPoint;
            default:
                return Guid.NewGuid();
        }
    }

    private ISession<TPackage> Create(Socket socket)
    {
        return new Session<TPackage>(new TcpChannel<TPackage>(_container, socket), GetSessionKey(socket));
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
        _container = container;
        _sessions = container.Resolve<IServer<TPackage>>().Sessions;
        _option = container.Resolve<IListenOption>().As<ListenOption>();
        _logManager = container.Resolve<ILogManager>();
    }

    public event IListener<TPackage>.NewSessionHandle EventNewSession;

    public ValueTask<bool> StartAsync()
    {
        if (_isStart) return new ValueTask<bool>(true);
        try
        {
            var option = _option;
            if (!ValidOption(option))
                throw new ArgumentException(nameof(option));
            var listenaddress = GetEndPoint(option.Ip, option.Port);
            if (listenaddress == null) throw new ArgumentException(nameof(listenaddress));
            var listensocket = _listenSocket =
                new Socket(listenaddress.AddressFamily, SocketType.Stream, ProtocolType.Udp);
            //SocketType https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.sockettype?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(System.Net.Sockets.SocketType)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0
            //ProtocolType https://learn.microsoft.com/zh-cn/dotnet/api/system.net.sockets.protocoltype?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(System.Net.Sockets.ProtocolType)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0
            if (option.NoDelay)
                listensocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

            //_acceptRemoteEndPoint = listenaddress.AddressFamily == AddressFamily.InterNetworkV6
            //    ? new IPEndPoint(IPAddress.IPv6Any, 0)
            //    : new IPEndPoint(IPAddress.Any, 0);
            var IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            var SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

            byte[] optionInValue = { Convert.ToByte(false) };
            var optionOutValue = new byte[4];
            listensocket.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
            listensocket.Bind(listenaddress);
            _ctsKeepAccept = new CancellationTokenSource();
            KeepAccept(listensocket).DoNotAwait();
        }
        catch (Exception e)
        {
            _logManager.BroadcastLog(e.ToLogInfo("listener start error"));
            return new ValueTask<bool>(false);
        }

        _isStart = true;
        return new ValueTask<bool>(true);
    }

    public ValueTask StopAsync()
    {
        if (!_isStart) return new ValueTask();
        var socket = _listenSocket;
        if (socket == null) return new ValueTask();
        _ctsKeepAccept.Cancel();
        socket.Close();
        socket.Dispose();
        _listenSocket = null;
        _ctsKeepAccept = null;
        _isStart = false;
        return new ValueTask();
    }

    protected bool ValidOption(ListenOption option)
    {
        if (option == null || string.IsNullOrEmpty(option.Ip) || option.Port < 0 || option.Port > 65535)
            return false;
        return true;
    }

    protected IPEndPoint GetEndPoint(string ip, int port)
    {
        if (string.IsNullOrEmpty(ip) || port == 0) return null;
        IPAddress ipaddress;
        if ("any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.Any;
        else if ("IpV6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.IPv6Any;
        else
            ipaddress = IPAddress.Parse(ip);
        return new IPEndPoint(ipaddress, port);
    }

    public async ValueTask KeepAccept(Socket socket)
    {
        var acceptendpoint = socket.AddressFamily == AddressFamily.InterNetworkV6
            ? new IPEndPoint(IPAddress.IPv6Any, 0)
            : new IPEndPoint(IPAddress.Any, 0);

        while (!_ctsKeepAccept.IsCancellationRequested)
        {
            var buffer = default(byte[]);
            try
            {
                buffer = ArrayPool<byte>.Shared.Rent(4 * 1024);

                var result = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                    SocketFlags.None, acceptendpoint).ConfigureAwait(false);
                var packagedata = new ArraySegment<byte>(buffer, 0, result.ReceivedBytes);


                var key = GetSessionKey(result.RemoteEndPoint);
                var selectchannel =
                    (UdpServerChannel<TPackage>)_sessions.FirstOrDefault(s => s.GetKey<object>().Equals(key))
                        ?.Channel;
                if (selectchannel == null)
                {
                    var session = Create(socket, (IPEndPoint)result.RemoteEndPoint, key);
                    await OnNewSession(session);
                    selectchannel = (UdpServerChannel<TPackage>)session.Channel;
                }

                await selectchannel.WriteReceiveDataAsync(packagedata.AsMemory(), _ctsKeepAccept.Token);
            }
            catch (Exception e)
            {
                if (e is ObjectDisposedException || e is NullReferenceException) break;

                if (e is SocketException se)
                {
                    var errorcode = se.ErrorCode;
                    //ListenSocket 被关闭了
                    if (errorcode == 125 ||
                        errorcode == 89 ||
                        errorcode == 995 ||
                        errorcode == 10004 ||
                        errorcode == 10038)
                        break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        if (EventNewSession != null && session != null)
            try
            {
                await EventNewSession(session);
            }
            catch (Exception e)
            {
                _logManager.BroadcastLog(e.ToLogInfo("Listen onnewsession error"));
            }
    }

    private object GetSessionKey(EndPoint endpoint)
    {
        switch (_option.SessionKey)
        {
            case EnumSessionKey.LocalEndPoint:
            case EnumSessionKey.RemoteEndPoint:
                return endpoint;
            default:
                return Guid.NewGuid();
        }
    }

    private ISession<TPackage> Create(Socket socket, IPEndPoint ipendpoint, object key)
    {
        return new Session<TPackage>(new UdpServerChannel<TPackage>(_container, socket, ipendpoint), key);
    }
}