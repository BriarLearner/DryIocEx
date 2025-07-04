using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.NetworkPro;

public interface IMiddlewareManager<TPackage>
{
    IEnumerable<IMiddleware<TPackage>> Middlewares { get; }
    TMiddleware GetMiddleware<TMiddleware>();
    void Register(IEnumerable<IMiddleware<TPackage>> middlewares);
}

public class MiddlewareManager<TPackage> : IMiddlewareManager<TPackage>
{
    private readonly Dictionary<Type, IMiddleware<TPackage>> middlewaredict = new();

    public IEnumerable<IMiddleware<TPackage>> Middlewares => middlewaredict.Values.OrderBy(s => s.Order);

    public void Register(IEnumerable<IMiddleware<TPackage>> middlewares)
    {
        if (middlewares != null && middlewares.Any())
            foreach (var middleware in middlewares.OrderBy(s => s.Order))
            {
                var inter = middleware.GetType().GetInterfaces()
                    .FirstOrDefault(s => s != typeof(IMiddleware<TPackage>));
                if (inter == null)
                    throw new ArgumentException(
                        $"this {middleware.GetType().FullName} not have single self interface");
                middlewaredict[inter] = middleware;
            }
    }

    public TMiddleware GetMiddleware<TMiddleware>()
    {
        if (middlewaredict.ContainsKey(typeof(TMiddleware)))
            return (TMiddleware)middlewaredict[typeof(TMiddleware)];
        return default;
    }
}

public interface IMiddleware<TPackage>
{
    int Order { get; }

    ValueTask Register(ISession<TPackage> session);

    ValueTask UnRegister(ISession<TPackage> session);

    ValueTask Handle(ISession<TPackage> session, TPackage package);
}

public class BaseMiddleware<TPackage>
{
    public virtual int Order { get; }

    public virtual ValueTask Register(ISession<TPackage> session)
    {
#if NET
        return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif
    }

    public virtual ValueTask UnRegister(ISession<TPackage> session)
    {
#if NET
        return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif
    }

    public virtual ValueTask Handle(ISession<TPackage> session, TPackage package)
    {
#if NET
        return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif
    }
}

public interface IReconnectorMiddleware<TPackage> : IMiddleware<TPackage>
{
    /// <summary>
    ///     用来控制停止重连
    /// </summary>
    bool IsStop { set; get; }

    ValueTask Reconnector(Func<ValueTask<bool>> reconnector);
}

public class ReconnectorMiddleware<TPackage> : BaseMiddleware<TPackage>, IReconnectorMiddleware<TPackage>
{
    public ReconnectorMiddleware(IContainer container)
    {
        IsStop = false;
    }


    public bool IsStop { get; set; }

    public ValueTask Reconnector(Func<ValueTask<bool>> reconector)
    {
        if (!IsStop)
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(3000);
                    await reconector();
                }
                catch (Exception e)
                {
                }
            });
        return new ValueTask();
    }
}

public interface IHandleMiddleware<TPackage> : IMiddleware<TPackage>
{
}

public class HandleMiddleware<TPackage> : BaseMiddleware<TPackage>, IHandleMiddleware<TPackage>
{
    private readonly Action<ISession<TPackage>, TPackage> _handle;

    public HandleMiddleware(Action<ISession<TPackage>, TPackage> action)
    {
        _handle = action;
    }

    public override ValueTask Handle(ISession<TPackage> session, TPackage package)
    {
        _handle.Invoke(session, package);
        return new ValueTask();
    }
}