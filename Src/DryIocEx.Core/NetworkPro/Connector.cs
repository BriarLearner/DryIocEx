using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.NetworkPro;

public interface IConnector<TPackage>
{
    ValueTask<ISession<TPackage>> ConnectAsync();
}

public interface IConnectorOption : IOption
{
}

public class ConnectorOption : IConnectorOption
{
    public string Ip { set; get; }
    public int Port { set; get; }
    public string LocalIp { set; get; }
    public int LocalPort { set; get; }

    public int TimeOut { set; get; } = 5;

    public EnumSessionKey SessionKey { set; get; } = EnumSessionKey.LocalEndPoint;
}

public enum EnumSessionKey
{
    LocalEndPoint,
    RemoteEndPoint,
    GUID
}

public class Connector<TPackage> : IConnector<TPackage>
{
    private readonly IContainer _container;

    private readonly ConnectorOption _option;

    public Connector(IContainer container)
    {
        _container = container;
        _option = container.Resolve<IConnectorOption>().As<ConnectorOption>();
    }


    public async ValueTask<ISession<TPackage>> ConnectAsync()
    {
        var connectendpoint = GetEndPoint(_option.Ip, _option.Port);
        if (connectendpoint == null)
            throw new ArgumentNullException("remote ip or port is empty");
        var socket = new Socket(connectendpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var localendpoint = GetEndPoint(_option.LocalIp, _option.LocalPort);
        if (localendpoint != null)
        {
            socket.ExclusiveAddressUse = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            socket.Bind(localendpoint);
        }

        try
        {
            //当问询模式的时候，断开网络无法确认客户端已经断开，所以加上这个keepAlive
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            
            var keepAliveValues = new byte[12];

            
            BitConverter.GetBytes(1).CopyTo(keepAliveValues, 0);
            
            BitConverter.GetBytes(60 * 1000).CopyTo(keepAliveValues, 4);
            
            BitConverter.GetBytes(60 * 1000).CopyTo(keepAliveValues, 8);

            socket.IOControl(
                IOControlCode.KeepAliveValues,
                keepAliveValues,
                null
            );


            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_option.TimeOut));


#if NET
            await socket.ConnectAsync(connectendpoint, cts.Token);

#else
                var task = socket.ConnectAsync(connectendpoint);
                Task.WaitAny(new[] { task }, cts.Token);
                var connected = socket.Connected;
                if (!connected)
                {
                    socket.Close();
                    return null;
                }
#endif
            return Create(socket);
        }
        catch (Exception e)
        {
            if (socket.Connected)
                socket?.Shutdown(SocketShutdown.Both); //在连接之后发生异常，需要断开连接
            socket?.Close();
        }

        return null;
    }

    private int GetAvailablePort()
    {
        
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            
            var endPoint = (IPEndPoint)socket.LocalEndPoint;
            return endPoint.Port;
        }
    }


    private IPEndPoint GetEndPoint(string ip, int port)
    {
        if (string.IsNullOrEmpty(ip)) return null;
        if (port == 0) port = GetAvailablePort();
        IPAddress ipaddress;
        if ("any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.Any; //0.0.0.0 不能用于连接
        if ("loopback".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.Loopback; //127.0.0.1 
        else if ("IpV6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
            ipaddress = IPAddress.IPv6Any;
        else
            ipaddress = IPAddress.Parse(ip);
        //var ipbytes = ipaddress.GetAddressBytes();
        //ipbytes[3] = 255;
        //ipaddress = new IPAddress(ipbytes);
        return new IPEndPoint(ipaddress, port);
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

public class UdpConnector<TPackage> : IConnector<TPackage>
{
    private readonly IContainer _container;

    private readonly ConnectorOption _option;

    public UdpConnector(IContainer container)
    {
        throw new NotImplementedException();
    }


    public async ValueTask<ISession<TPackage>> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    private IPEndPoint GetEndPoint(string ip, int port)
    {
        throw new NotImplementedException();
    }


    private object GetSessionKey(Socket socket)
    {
        throw new NotImplementedException();
    }

    private ISession<TPackage> Create(Socket socket, IPEndPoint remoteendpoint)
    {
        throw new NotImplementedException();
    }
}