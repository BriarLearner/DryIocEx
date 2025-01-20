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
        throw new NotImplementedException();
    }

    public TMiddleware GetMiddleware<TMiddleware>()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public virtual ValueTask UnRegister(ISession<TPackage> session)
    {
        throw new NotImplementedException();
    }

    public virtual ValueTask Handle(ISession<TPackage> session, TPackage package)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }


    public bool IsStop { get; set; }

    public ValueTask Reconnector(Func<ValueTask<bool>> reconector)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public override ValueTask Handle(ISession<TPackage> session, TPackage package)
    {
        throw new NotImplementedException();
    }
}