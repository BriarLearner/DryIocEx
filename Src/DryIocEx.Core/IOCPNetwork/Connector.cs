using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuddenGale.Core.IOC;
using SuddenGale.Core.IOCPNetwork;

namespace SuddenGale.Core.IOCPNetwork
{
    public interface IConnector<TPackage>
    {
        ValueTask<ISession<TPackage>> ConnectAsync();
    }


    public interface IConnectorOption
    {
        public string Ip { set; get; }
        public int Port { set; get; }
        public string LocalIp { set; get; }
        public int LocalPort { set; get; }
    }

    public class ConnectorOption: IConnectorOption
    {
        public string Ip { set; get; }
        public int Port { set; get; }
        public string LocalIp { set; get; }
        public int LocalPort { set; get; }
    }

    public class Connector<TPackage> : IConnector<TPackage>
    {
        private IContainer Container;

        private ConnectorOption Option;
        public Connector(IContainer container)
        {
            this.Container = container;
            this.Option = container.Resolve<IConnectorOption>().As<ConnectorOption>();
        }


        public async ValueTask<ISession<TPackage>> ConnectAsync()
        {
            var connectendpoint = NetworkUtil.GetEndPoint(Option.Ip, Option.Port);
            if (connectendpoint == null)
            {
                throw new ArgumentNullException("remote ip and port is empty");
            }
            var socket = new Socket(connectendpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var localendpoint = NetworkUtil.GetEndPoint(Option.LocalIp, Option.LocalPort);
            if (localendpoint != null)
            {
                socket.ExclusiveAddressUse = false;
                socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,1);
                socket.Bind(localendpoint);
            }

            try
            {
                var cts=new CancellationTokenSource(TimeSpan.FromSeconds(5));
#if NET
               await socket.ConnectAsync(connectendpoint,cts.Token);
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

                return null;

            }
        }
        private ISession<TPackage> Create(Socket socket)
        {
            return new Session<TPackage>(new TcpChannel<TPackage>(Container,socket));
        }


    }

}
