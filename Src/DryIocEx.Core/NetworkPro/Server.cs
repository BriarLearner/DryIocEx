using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.NetworkPro;

public interface IServer<TPackage>
{
    EnumNetworkState State { get; }

    IContainer Container { get; }


    public List<ISession<TPackage>> Sessions { get; }

    ValueTask<bool> StartAsync();
    ValueTask StopAsync();

    TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>;
}

public class Server<TPackage> : IServer<TPackage>
{
    private IListener<TPackage> _listener;
    private readonly ILogManager _logManager;

    public Server(IContainer container)
    {
        throw new NotImplementedException();
    }

    public IMiddlewareManager<TPackage> MiddlewareManager { get; }
    public EnumNetworkState State { get; private set; }
    public IContainer Container { get; }
    public List<ISession<TPackage>> Sessions { get; } = new();

    public async ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    public TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>
    {
        return MiddlewareManager.GetMiddleware<TMiddleware>();
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }
}

public class ServerBuilder<TPackage, TSelf> where TSelf : ServerBuilder<TPackage, TSelf>
{
    protected readonly IContainer _container;

    protected List<Func<IContainer, IContainer>> _funcs = new();

    public ServerBuilder()
    {
        throw new NotImplementedException();
    }

    public TSelf AddAction(Action<IContainer> action)
    {
        throw new NotImplementedException();
    }

    public IServer<TPackage> Build()
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

    public TSelf UseConsoleLog()
    {
        throw new NotImplementedException();
    }

    public TSelf UseLog<TLogger>(Func<IContainer, TLogger> factory) where TLogger : ILogger
    {
        throw new NotImplementedException();
    }

    public TSelf HandlePackage(Action<ISession<TPackage>, TPackage> action)
    {
        throw new NotImplementedException();
    }

    public TSelf UseSession(Action<SessionOption> optionaction)
    {
       throw new NotImplementedException();
    }

    public TSelf UseFilter<TPackageFilter>() where TPackageFilter : IPackageFilter<TPackage>
    {
       throw new NotImplementedException();
    }
}

public class TcpServerBuilder<TPackage, TSelf> : ServerBuilder<TPackage, TSelf>
    where TSelf : TcpServerBuilder<TPackage, TSelf>
{
    public TcpServerBuilder()
    {
        throw new NotImplementedException();
    }

    public TSelf UseListen(Action<ListenOption> optionaction)
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
}

public class SimpleTcpServerBuilder<TPackage> : TcpServerBuilder<TPackage, SimpleTcpServerBuilder<TPackage>>
{
}

public class UdpServerBuilder<TPackage, TSelf> : ServerBuilder<TPackage, TSelf>
    where TSelf : UdpServerBuilder<TPackage, TSelf>
{
    public UdpServerBuilder()
    {
        throw new NotImplementedException();
    }

    public TSelf UseListen(Action<ListenOption> optionaction)
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
}

public class SimpleUdpServerBuilder<TPackage> : UdpServerBuilder<TPackage, SimpleUdpServerBuilder<TPackage>>
{
}