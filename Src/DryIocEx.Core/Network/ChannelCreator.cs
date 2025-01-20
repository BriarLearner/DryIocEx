#if NET


using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Network;

public interface IChannelCreator
{
    //相当于Listen
    Action<IChannelCreator, IChannel> NewChannelCallBack { get; set; }
    public bool IsStart { get; }
    ValueTask<bool> StartAsync();
    Task StopAsync();
}

public interface IChannelCreator<TPackage> : IChannelCreator
{
    IChannel<TPackage> Create(params object[] objs);
}

public interface IChannelCreatorFactory
{
    //Factory 可以对创建的ChannelCreator进行参数设置
    IChannelCreator Create();
}

public class ChannelCreatorFactory : IChannelCreatorFactory
{
    protected IServer Server;

    public ChannelCreatorFactory(IServer server)
    {
        throw new NotImplementedException();
    }

    public virtual IChannelCreator Create()
    {
        throw new NotImplementedException();
    }
}

public abstract class ChannelCreator<TPackage> : IChannelCreator<TPackage>
{
    public ChannelCreator(IServer server)
    {
        throw new NotImplementedException();
    }

    public IServer Server { get; }
    public bool IsStart { get; protected set; }
    public abstract IChannel<TPackage> Create(params object[] objs);

    public Action<IChannelCreator, IChannel> NewChannelCallBack { get; set; }

    public abstract ValueTask<bool> StartAsync();

    public abstract Task StopAsync();

    protected IPEndPoint GetEndPoint(string ip, int port)
    {
        throw new NotImplementedException();
    }

    protected bool ValidOption(ChannelCreatorOption option)
    {
        throw new NotImplementedException();
    }
}

public class TcpChannelCreator<TPackage> : ChannelCreator<TPackage>
{
    protected CancellationTokenSource ctsKeepAccept;


    protected Socket listenSocket;
    protected TaskCompletionSource<bool> tcsStop;

    public TcpChannelCreator(IServer server) : base(server)
    {
        throw new NotImplementedException();
    }

    public TcpChannelCreatorOption Option { get; }

    public override IChannel<TPackage> Create(params object[] objs)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask KeepAccept(Socket socket)
    {
        throw new NotImplementedException();
    }

    private void OnNewChannel(IChannel channel)
    {
        throw new NotImplementedException();
    }


    public override Task StopAsync()
    {
        throw new NotImplementedException();
    }
}

public class UdpChannelCreator<TPackage> : ChannelCreator<TPackage>
{
    private IPEndPoint _acceptRemoteEndPoint;
    protected CancellationTokenSource _cancellationTokenSource;
    private Socket _listenSocket;
    private TaskCompletionSource<bool> _stopTaskCompletionSource;

    public UdpChannelCreator(IServer server) : base(server)
    {
        throw new NotImplementedException();
    }

    public ChannelCreatorOption Option { get; }

    public PipeChannelOption ChannelOption { get; }

    public override IChannel<TPackage> Create(params object[] objs)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask KeepAccept(Socket listenSocket)
    {
        throw new NotImplementedException();
    }

    private IChannel Create<TPackage>(Socket listenSocket, IPEndPoint remoteendpoint, string key)
    {
        throw new NotImplementedException();
    }

    public override Task StopAsync()
    {
        throw new NotImplementedException();
    }
}

#endif