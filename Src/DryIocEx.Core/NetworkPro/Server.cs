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
        Container = container;
        MiddlewareManager = container.Resolve<IMiddlewareManager<TPackage>>();
        _logManager = container.Resolve<ILogManager>();
        var middlewares = container.Resolve<IEnumerable<IMiddleware<TPackage>>>();
        if (middlewares != null && middlewares.Any())
            try
            {
                MiddlewareManager.Register(middlewares);
            }
            catch (Exception e)
            {
                _logManager.BroadcastLog(e.ToLogInfo("MiddlewareManager Register"));
            }
    }

    public IMiddlewareManager<TPackage> MiddlewareManager { get; }
    public EnumNetworkState State { get; private set; }
    public IContainer Container { get; }
    public List<ISession<TPackage>> Sessions { get; } = new();

    public async ValueTask<bool> StartAsync()
    {
        if (State == EnumNetworkState.Start) return true;
        var listener = _listener = Container.Resolve<IListener<TPackage>>();
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        var result = await listener.StartAsync();
        if (result)
        {
            listener.EventNewSession += OnNewSession;
            State = EnumNetworkState.Start;
        }

        return result;
    }

    public async ValueTask StopAsync()
    {
        var state = State;
        if (state == EnumNetworkState.Stop) return;
        var listen = _listener;
        if (listen != null)
        {
            await listen.StopAsync();
            _listener = null;
        }

        foreach (var session in Sessions)
        {
            foreach (var middleware in MiddlewareManager.Middlewares)
                try
                {
                    await middleware.UnRegister(session);
                }
                catch (Exception e)
                {
                    _logManager.BroadcastLog(e.ToLogInfo("Middleware unregister session"));
                }

            if (!session.IsStop)
                session.Stop();
        }

        Sessions.Clear();
        State = EnumNetworkState.Stop;
    }

    public TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>
    {
        return MiddlewareManager.GetMiddleware<TMiddleware>();
    }

    private async ValueTask OnNewSession(ISession<TPackage> session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));
        try
        {
            session.Start();
            Sessions.Add(session);
            foreach (var middleware in MiddlewareManager.Middlewares)
                try
                {
                    await middleware.Register(session);
                }
                catch (Exception e)
                {
                    _logManager.BroadcastLog(e.ToLogInfo("Middleware register session"));
                }

            await foreach (var package in session.RunAsync())
                foreach (var middleware
                         in MiddlewareManager.Middlewares)
                    try
                    {
                        await middleware.Handle(session, package);
                    }
                    catch (Exception e)
                    {
                        _logManager.BroadcastLog(e.ToLogInfo("Middleware handle package"));
                    }
        }
        catch (Exception e)
        {
            _logManager.BroadcastLog(e.ToLogInfo("OnNewSession"));
        }
        finally
        {
            foreach (var middleware in MiddlewareManager.Middlewares)
                try
                {
                    await middleware.UnRegister(session);
                }
                catch (Exception e)
                {
                    _logManager.BroadcastLog(e.ToLogInfo("Middleware unregister session"));
                }

            if (!session.IsStop)
                session.Stop();
            Sessions.Remove(session);
        }
    }
}

public class ServerBuilder<TPackage, TSelf> where TSelf : ServerBuilder<TPackage, TSelf>
{
    protected readonly IContainer _container;

    protected List<Func<IContainer, IContainer>> _funcs = new();

    public ServerBuilder()
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

    public IServer<TPackage> Build()
    {
        var container = _funcs.Aggregate(_container
            , (c, action) => action(c));
        var server = new Server<TPackage>(container);
        container.Register<IServer<TPackage>>(server);
        return server;
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

public class TcpServerBuilder<TPackage, TSelf> : ServerBuilder<TPackage, TSelf>
    where TSelf : TcpServerBuilder<TPackage, TSelf>
{
    public TcpServerBuilder()
    {
        _funcs.Add(c =>
        {
            c.Register<IChannelOption>(new ChannelOption());
            c.Register<ISessionOption>(new SessionOption());
            c.Register<IListener<TPackage>, Listener<TPackage>>(EnumLifetime.Transient);
            return c;
        });
    }

    public TSelf UseListen(Action<ListenOption> optionaction)
    {
        _funcs.Add(c =>
        {
            var option = new ListenOption();
            optionaction(option);
            c.Register<IListenOption>(option);
            return c;
        });
        return (TSelf)this;
    }


    public TSelf UseTcp(string ip, int port, Action<ISession<TPackage>, TPackage> handle)
    {
        UseListen(o =>
        {
            o.Ip = ip;
            o.Port = port;
        });
        HandlePackage(handle);
        return (TSelf)this;
    }

    public TSelf UseTcp(string ip, int port)
    {
        UseListen(o =>
        {
            o.Ip = ip;
            o.Port = port;
        });
        return (TSelf)this;
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