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
        throw new NotImplementedException();
    }

    private int GetAvailablePort()
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

    private ISession<TPackage> Create(Socket socket)
    {
        throw new NotImplementedException();
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