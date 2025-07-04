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
        Container = container;
        MiddlewareManager = container.Resolve<IMiddlewareManager<TPackage>>();
        _logManager = container.Resolve<ILogManager>();
        var middlewares = container.Resolve<IEnumerable<IMiddleware<TPackage>>>();
        if (middlewares != null && middlewares.Any()) MiddlewareManager.Register(middlewares);
    }

    public IMiddlewareManager<TPackage> MiddlewareManager { get; }
    public EnumNetworkState State { get; private set; }
    public IContainer Container { get; }


    public ValueTask<bool> StartAsync()
    {
        var middleware = MiddlewareManager.GetMiddleware<IReconnectorMiddleware<TPackage>>();
        if (middleware != null) middleware.IsStop = false;

        return InnerStartAsync();
    }

    public async ValueTask StopAsync()
    {
        try
        {
            //主动关闭重连
            var middleware = MiddlewareManager.GetMiddleware<IReconnectorMiddleware<TPackage>>();
            if (middleware != null) middleware.IsStop = true;

            await HandleStop();//这边是否需要触发
        }
        catch (Exception e)
        {
        }
    }

    public ValueTask SendAsync(byte[] buffer)
    {
        return _session.SendAsync(buffer);
    }

    public ValueTask SendAsync(TPackage package)
    {
        return _session.SendAsync(package);
    }

    public TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>
    {
        return MiddlewareManager.GetMiddleware<TMiddleware>();
    }

    public event IClient<TPackage>.StateChangedHandle StateChanged;

    private async ValueTask OnMiddlewareHandle(ISession<TPackage> session, TPackage package)
    {
        foreach (var middleware in MiddlewareManager.Middlewares)
            try
            {
                await middleware.Handle(session, package);
            }
            catch (Exception e)
            {
            }
    }

    private async ValueTask OnMiddlewareRegister(ISession<TPackage> session)
    {
        foreach (var middleware in MiddlewareManager.Middlewares)
            try
            {
                await middleware.Register(session);
            }
            catch (Exception e)
            {
            }
    }

    private async ValueTask OnMiddlewareUnRegister(ISession<TPackage> session)
    {
        foreach (var middleware in MiddlewareManager.Middlewares)
            try
            {
                await middleware.UnRegister(session);
            }
            catch (Exception e)
            {
            }
    }


    private async ValueTask<bool> InnerStartAsync()
    {
        if (State == EnumNetworkState.Start) return true;
        var connector = Container.Resolve<IConnector<TPackage>>();
        var session = _session = await connector.ConnectAsync();
        if (session == null)
        {
            var middleware = MiddlewareManager.GetMiddleware<IReconnectorMiddleware<TPackage>>();
            if (middleware != null)
            {
                await middleware.Reconnector(InnerStartAsync);
                return true;
            }

            return false;
        }
        
        session.SessionStop += OnSessionStop;
        try
        {
            session.Start();
        }
        catch (Exception e)
        {
            return false;
        }

        State = EnumNetworkState.Start;
        try
        {
            await OnMiddlewareRegister(session);
            OnStateChanged(session, EnumNetworkState.Start);
        }
        catch (Exception e)
        {
        }

        InnerHandlePackage(session);
        return true;
    }

    private async ValueTask OnSessionStop(ISession<TPackage> session, StopReason reason)
    {
        try
        {
            await HandleStop();
            //重新连接业务
            var middleware = MiddlewareManager.GetMiddleware<IReconnectorMiddleware<TPackage>>();
            if (middleware != null) await middleware.Reconnector(InnerStartAsync);//这边重连
        }
        catch (Exception e)
        {
        }
    }

    private async void InnerHandlePackage(ISession<TPackage> session)
    {
        try
        {
            await foreach (var p in session.RunAsync())
                if (p != null)
                    await OnMiddlewareHandle(session, p);
        }
        catch (Exception e)
        {
        }
        finally
        {
            await HandleStop();
        }
    }

    public async ValueTask HandleStop()
    {
        if (State == EnumNetworkState.Stop) return;
        State = EnumNetworkState.Stop;
        var session = _session;
        _session = null;
        await OnMiddlewareUnRegister(session);
        if (!session.IsStop)
        {

            session.Stop();
        }
        OnStateChanged(session, EnumNetworkState.Stop);
    }

    private void OnStateChanged(ISession<TPackage> session, EnumNetworkState state)
    {
        if (StateChanged != null && session != null) StateChanged(session, state);
    }
}

public class ClientBuilder<TPackage, TSelf> where TSelf : ClientBuilder<TPackage, TSelf>
{
    protected readonly IContainer _container;

    protected List<Func<IContainer, IContainer>> _funcs = new();

    public ClientBuilder()
    {
        _container = new Container();
        _container.Register<ILogManager>(new LogManager());
        _container.Register<IMiddlewareManager<TPackage>>(new MiddlewareManager<TPackage>());
    }

    public TSelf AddAction(Action<IContainer> action)
    {
        if (action == null) return (TSelf)this;
        _funcs.Add(s =>
        {
            action(s);
            return s;
        });
        return (TSelf)this;
    }

    public IClient<TPackage> Build()
    {
        var container = _funcs.Aggregate(_container
            , (c, action) => action(c));
        var client = new Client<TPackage>(container);
        container.Register<IClient<TPackage>>(client);
        return client;
    }

    public TSelf UseMiddleware<TMiddleware>() where TMiddleware : class, IMiddleware<TPackage>
    {
        _funcs.Add(c => { return c.Register<IMiddleware<TPackage>, TMiddleware>(EnumLifetime.Singleton); });
        return (TSelf)this;
    }

    public TSelf UseMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : class, IMiddleware<TPackage>
    {
        _funcs.Add(c => { return c.Register<IMiddleware<TPackage>>(middleware); });
        return (TSelf)this;
    }

    public TSelf HandlePackage(Action<ISession<TPackage>, TPackage> action)
    {
        _funcs.Add(c =>
        {
            var middle = new HandleMiddleware<TPackage>(action);
            c.Register<IMiddleware<TPackage>>(middle);
            return c;
        });
        return (TSelf)this;
    }

    public TSelf UseConnect(Action<ConnectorOption> optionaction)
    {
        _funcs.Add(c =>
        {
            var option = new ConnectorOption();
            optionaction(option);
            c.Register<IConnectorOption>(option);
            return c;
        });
        return (TSelf)this;
    }

    public TSelf UseSession(Action<SessionOption> optionaction)
    {
        _funcs.Add(c =>
        {
            var option = c.Resolve<ISessionOption>().As<SessionOption>();
            optionaction(option);
            return c;
        });
        return (TSelf)this;
    }

    public TSelf UseConsoleLog()
    {
        _funcs.Add(c =>
        {
            var logmanager = c.Resolve<ILogManager>();
            var logger = new ConsoleLogBuilder().SetDefaultOption().Build();
            logmanager.Register<IConsoleLogger>(logger);
            return c;
        });
        return (TSelf)this;
    }

    public TSelf UseLog<TLogger>(Func<IContainer, TLogger> factory) where TLogger : ILogger
    {
        _funcs.Add(c =>
        {
            var logmanager = c.Resolve<ILogManager>();
            var logger = factory.Invoke(c);
            logmanager.Register(logger);
            return c;
        });
        return (TSelf)this;
    }

    public TSelf UseFilter<TPackageFilter>() where TPackageFilter : IPackageFilter<TPackage>
    {
        _funcs.Add(c =>
        {
            c.Register<IPackageFilter<TPackage>, TPackageFilter>(EnumLifetime.Transient);
            return c;
        });
        return (TSelf)this;
    }
}

public class TcpClientBuilder<TPackage, TSelf> : ClientBuilder<TPackage, TSelf>
    where TSelf : TcpClientBuilder<TPackage, TSelf>
{
    public TcpClientBuilder()
    {
        _funcs.Add(c =>
        {
            c.Register<IChannelOption>(new ChannelOption());
            c.Register<ISessionOption>(new SessionOption());
            c.Register<IConnector<TPackage>, Connector<TPackage>>(EnumLifetime.Transient);
            return c;
        });
    }


    public TSelf UseTcp(string ip, int port, Action<ISession<TPackage>, TPackage> handle)
    {
        UseConnect(o =>
        {
            o.Ip = ip;
            o.Port = port;
        });
        HandlePackage(handle);
        return (TSelf)this;
    }

    public TSelf UseTcp(string ip, int port)
    {
        UseConnect(o =>
        {
            o.Ip = ip;
            o.Port = port;
        });
        return (TSelf)this;
    }

    public TSelf UseTcp(string ip, int port, string localip)
    {
        UseConnect(o =>
        {
            o.Ip = ip;
            o.Port = port;
            o.LocalIp = localip;
        });
        return (TSelf)this;
    }

    public TSelf UseReconnect()
    {
        _funcs.Add(c =>
        {
            c.Register<IMiddleware<TPackage>>(new ReconnectorMiddleware<TPackage>(c));
            return c;
        });
        return (TSelf)this;
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