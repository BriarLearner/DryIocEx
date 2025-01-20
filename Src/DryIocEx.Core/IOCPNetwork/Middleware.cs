using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuddenGale.Core.IOC;

namespace SuddenGale.Core.IOCPNetwork
{


    public interface IMiddleware<TPackage>
    {
        int Order { get; }

        ValueTask Register(ISession<TPackage> session);

        ValueTask UnRegister(ISession<TPackage> session);

        ValueTask Handle(ISession<TPackage> session, TPackage package);

    }

    public interface IReconnectorMiddleware<TPackage> : IMiddleware<TPackage>
    {
        ValueTask Reconnector(Func<ValueTask<bool>> reconnector);
    }


    public class BaseMiddleware<TPackage> 
    {
        public virtual int Order { get; }
        public virtual ValueTask StartAsync()
        {
#if NET
             return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif

        }

        public virtual ValueTask StopAsync()
        {
#if NET
             return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif
        }

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

    public class ReconnectorMiddleware<TPackage>:BaseMiddleware<TPackage>,IReconnectorMiddleware<TPackage>
    {
        
        public ReconnectorMiddleware(IContainer container)
        {
            
        }


        public ValueTask Reconnector(Func<ValueTask<bool>> reconector)
        {
            Task.Run(async() =>
            {
                await Task.Delay(2000);
                await reconector();
            });
            return new ValueTask();
        }
    }

    public interface IClientHandleMiddleware<TPackage> : IMiddleware<TPackage>
    {

    }
    public class ClientHandleMiddleware<TPackage> : BaseMiddleware<TPackage>, IClientHandleMiddleware<TPackage>
    {
        private Action<ISession<TPackage>, TPackage> _handle;
        public ClientHandleMiddleware(Action<ISession<TPackage>,TPackage> action)
        {
            _handle = action;
        }

        public override ValueTask Handle(ISession<TPackage> session, TPackage package)
        {
            _handle.Invoke(session,package);
            return new ValueTask();
        }
    }


    public interface IMiddlewareManager<TPackage>
    {
        TMiddleware GetMiddleware<TMiddleware>();
        void Register(IEnumerable<IMiddleware<TPackage>> middlewares);
        IEnumerable<IMiddleware<TPackage>> Middlewares { get; }
    }


    public class MiddlewareManager<TPackage>:IMiddlewareManager<TPackage>
    {

        private Dictionary<Type, IMiddleware<TPackage>> middlewaredict = new Dictionary<Type, IMiddleware<TPackage>>();
        
        public IEnumerable<IMiddleware<TPackage>> Middlewares=>middlewaredict.Values.OrderBy(s=>s.Order);

        public void Register(IEnumerable<IMiddleware<TPackage>> middlewares)
        {
            if (middlewares != null && middlewares.Any())
            {
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
        }

        public TMiddleware GetMiddleware<TMiddleware>()
        {
            if(middlewaredict.ContainsKey(typeof(TMiddleware)))
                return (TMiddleware) middlewaredict[typeof(TMiddleware)];
            return default;
        }
    }
}
