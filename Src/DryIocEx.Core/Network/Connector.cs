#if NET


using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DryIocEx.Core.Network
{
    public interface IConnectorOption
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        public string LocalIp { set; get; }

        public int LocalPort { set; get; }
    }

    public class ConnectorOption : IConnectorOption
    {
        public string Ip { set; get; }
        public int Port { set; get; }
        public string LocalIp { set; get; }

        public int LocalPort { set; get; }
    }

    public interface IConnector
    {
        IClient Client { get; }
        IConnectorOption Option { get; }

        ValueTask<IChannel> Connect();
    }

    public interface IConnector<TPackage> : IConnector
    {
        IChannel<TPackage> Create(params object[] objs);
    }

    public abstract class Connector<TPackage> : IConnector<TPackage>
    {
        public Connector(IClient client)
        {
            throw new NotImplementedException();
        }

        public IClient Client { get; }

        public IConnectorOption Option { get; }

        public abstract ValueTask<IChannel> Connect();


        public abstract IChannel<TPackage> Create(params object[] objs);

        protected virtual EndPoint GetEndPoint(string ip, int port)
        {
            throw new NotImplementedException();
        }
    }

    public class TcpConnector<TPackage> : Connector<TPackage>
    {
        public TcpConnector(IClient client) : base(client)
        {
        }

        public override async ValueTask<IChannel> Connect()
        {
            throw new NotImplementedException();
        }

        public override IChannel<TPackage> Create(params object[] objs)
        {
            throw new NotImplementedException();
        }
    }


    public class UdpConnector<TPackage> : Connector<TPackage>
    {
        public UdpConnector(IClient client) : base(client)
        {
        }

        private IPEndPoint RemoteEndpoint { set; get; }

        public override async ValueTask<IChannel> Connect()
        {
            throw new NotImplementedException();
        }

        public override IChannel<TPackage> Create(params object[] objs)
        {
            throw new NotImplementedException();
        }
    }
}
#endif