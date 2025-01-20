using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.NetworkPro;

public interface IOption
{
}

public enum EnumNetworkState
{
    Stop,
    Start
}

public interface IClient<TPackage>
{
    public delegate ValueTask StateChangedHandle(ISession<TPackage> session, EnumNetworkState state);

    EnumNetworkState State { get; }
    IContainer Container { get; }
    ValueTask<bool> StartAsync();

    ValueTask StopAsync();

    ValueTask SendAsync(byte[] buffer);

    ValueTask SendAsync(TPackage package);

    TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>;

    public event StateChangedHandle StateChanged;
}

public class Client<TPackage> : IClient<TPackage>
{
    private ILogManager _logManager;

    private ISession<TPackage> _session;

    public Client(IContainer container)
    {
        throw new NotImplementedException();
    }

    public IMiddlewareManager<TPackage> MiddlewareManager { get; }
    public EnumNetworkState State { get; private set; }
    public IContainer Container { get; }


    public ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }

    public TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>
    {
        throw new NotImplementedException();
    }

    public event IClient<TPackage>.StateChangedHandle StateChanged;

    private async ValueTask OnMiddlewareHandle(ISession<TPackage> session, TPackage package)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnMiddlewareRegister(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnMiddlewareUnRegister(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }


    private async ValueTask<bool> InnerStartAsync()
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnSessionStop(ISession<TPackage> session, StopReason reason)
    {
        throw new NotImplementedException();
    }

    private async void InnerHandlePackage(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    public async ValueTask HandleStop()
    {
        throw new NotImplementedException();
    }

    private void OnStateChanged(ISession<TPackage> session, EnumNetworkState state)
    {
        throw new NotImplementedException();
    }
}

public class ClientBuilder<TPackage, TSelf> where TSelf : ClientBuilder<TPackage, TSelf>
{
    protected readonly IContainer _container;

    protected List<Func<IContainer, IContainer>> _funcs = new();

    public ClientBuilder()
    {
        throw new NotImplementedException();
    }

    public TSelf AddAction(Action<IContainer> action)
    {
        throw new NotImplementedException();
    }

    public IClient<TPackage> Build()
    {
        throw new NotImplementedException();
    }

    public TSelf UseMiddleware<TMiddleware>() where TMiddleware : class, IMiddleware<TPackage>
    {
        throw new NotImplementedException();
    }

    public TSelf UseMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : class, IMiddleware<TPackage>
    {
        throw new NotImplementedException();
    }

    public TSelf HandlePackage(Action<ISession<TPackage>, TPackage> action)
    {
        throw new NotImplementedException();
    }

    public TSelf UseConnect(Action<ConnectorOption> optionaction)
    {
        throw new NotImplementedException();
    }

    public TSelf UseSession(Action<SessionOption> optionaction)
    {
        throw new NotImplementedException();
    }

    public TSelf UseConsoleLog()
    {
        throw new NotImplementedException();
    }

    public TSelf UseLog<TLogger>(Func<IContainer, TLogger> factory) where TLogger : ILogger
    {
        throw new NotImplementedException();
    }

    public TSelf UseFilter<TPackageFilter>() where TPackageFilter : IPackageFilter<TPackage>
    {
        throw new NotImplementedException();
    }
}

public class TcpClientBuilder<TPackage, TSelf> : ClientBuilder<TPackage, TSelf>
    where TSelf : TcpClientBuilder<TPackage, TSelf>
{
    public TcpClientBuilder()
    {
        throw new NotImplementedException();
    }


    public TSelf UseTcp(string ip, int port, Action<ISession<TPackage>, TPackage> handle)
    {
        throw new NotImplementedException();
    }

    public TSelf UseTcp(string ip, int port)
    {
        throw new NotImplementedException();
    }

    public TSelf UseTcp(string ip, int port, string localip)
    {
        throw new NotImplementedException();
    }

    public TSelf UseReconnect()
    {
        throw new NotImplementedException();
    }
}

public class SimpleTcpClientBuilder<TPackage> : TcpClientBuilder<TPackage, SimpleTcpClientBuilder<TPackage>>
{
}

public class UdpClientBuilder<TPackage, TSelf> : ClientBuilder<TPackage, TSelf>
    where TSelf : UdpClientBuilder<TPackage, TSelf>
{
    public UdpClientBuilder()
    {
        throw new NotImplementedException();
    }


    public TSelf UseUdp(string ip, int port, Action<ISession<TPackage>, TPackage> handle)
    {
        throw new NotImplementedException();
    }

    public TSelf UseUdp(string ip, int port)
    {
        throw new NotImplementedException();
    }

    public TSelf UseReconnect()
    {
        throw new NotImplementedException();
    }
}

public class SimpleUdpClientBuilder<TPackage> : UdpClientBuilder<TPackage, SimpleUdpClientBuilder<TPackage>>
{
}