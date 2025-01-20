using SuddenGale.Core.Event;
using SuddenGale.Core.IOC;
using SuddenGale.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuddenGale.Core.IOCPNetwork
{

    public class ClientOption
    {
        public string Name { get; set; }
    }

    public interface IClient<TPackage>
    {
        IContainer Container { get; }

        EnumNetworkState State { get; }


        ValueTask<bool> StartAsync();

        ValueTask StopAsync();


        ValueTask SendAsync(byte[] buffer);

        ValueTask SendAsync(TPackage package);



        TMiddleware GetMiddleware<TMiddleware>() where TMiddleware : IMiddleware<TPackage>;
    }





    public enum EnumNetworkState
    {
        Stop,
        Start,

    }

    public class EventState<TPackage> : PubSubEvent<ISession<TPackage>, EnumNetworkState>
    {

    }
    public class Client<TPackage> : IClient<TPackage>
    {
        public IContainer Container { get; }
        public EventState<TPackage> EventState { get; }

        public EnumNetworkState State { get; private set; }

        #region Middleware



        private async ValueTask OnMiddlewareHandle(ISession<TPackage> session, TPackage package)
        {
            foreach (var middleware in MiddlewareManager.Middlewares)
            {
                try
                {
                    await middleware.Handle(session, package);
                }
                catch (Exception e)
                {
                }
            }
        }

        private async ValueTask OnMiddlewareRegister(ISession<TPackage> session)
        {

            foreach (var middleware in MiddlewareManager.Middlewares)
            {
                try
                {
                    await middleware.Register(session);
                }
                catch (Exception e)
                {
                }

            }
        }




        private async ValueTask OnMiddlewareUnRegister(ISession<TPackage> session)
        {
            foreach (var middleware in MiddlewareManager.Middlewares)
            {
                try
                {
                    await middleware.UnRegister(session);
                }
                catch (Exception e)
                {

                }

            }
        }
        #endregion
        private ISession<TPackage> _session;
        public async ValueTask<bool> StartAsync()
        {
            var connector = Container.Resolve<IConnector<TPackage>>();
            var session = _session = await connector.ConnectAsync();
            if (session == null)
            {
                var middleware = MiddlewareManager.GetMiddleware<IReconnectorMiddleware<TPackage>>();
                if (middleware != null)
                {
                    await middleware.Reconnector(StartAsync);
                    return true;
                }
                return false;
            }
            session.Stopped.Subscribe(OnStopped, EnumThreadType.Publisher, false);
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
                EventState.Publish(session, EnumNetworkState.Start);
            }
            catch (Exception e)
            {
            }

            InnerHandlePackage(session);
            return true;
        }

        private async void InnerHandlePackage(ISession<TPackage> session)
        {
            try
            {
                await foreach (var p in session.RunAsync())
                {

                    if (p != null) await OnMiddlewareHandle(session, p);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                await HandleStop();
            }


        }


        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reason"></param>
        private async void OnStopped(ISession<TPackage> session, StopReason reason)
        {
            try
            {
                await HandleStop();
            }
            catch (Exception e)
            {

            }
        }


        public async ValueTask HandleStop()
        {
            if (State == EnumNetworkState.Stop) return;
            State = EnumNetworkState.Stop;
            await OnMiddlewareUnRegister(_session);
            if (!_session.IsStop)
            {
                _session.Stop();
            }
            EventState.Publish(_session, EnumNetworkState.Stop);
        }

        public async ValueTask StopAsync()
        {
            try
            {
                await HandleStop();
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

        public IMiddlewareManager<TPackage> MiddlewareManager { get; }

        private ILogManager _logManager;

        public Client(IContainer container)
        {
            Container = container;
            EventState = new EventState<TPackage>();
            MiddlewareManager = container.Resolve<IMiddlewareManager<TPackage>>();
            _logManager = container.Resolve<ILogManager>();
            var middlewares = container.Resolve<IEnumerable<IMiddleware<TPackage>>>();
            if (middlewares != null && middlewares.Any())
            {
                MiddlewareManager.Register(middlewares);
            }
        }
    }


    public class ClientBuilder<TPackage, TSelf> where TSelf : ClientBuilder<TPackage, TSelf>
    {
        protected readonly IContainer _container;

        public ClientBuilder()
        {
            _container = new Container();

            _container.Register<ILogManager>(new LogManager());
            _container.Register<IMiddlewareManager<TPackage>>(new MiddlewareManager<TPackage>());
        }

        protected List<Func<IContainer, IContainer>> _funcs = new List<Func<IContainer, IContainer>>();

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
            var container = _funcs.Aggregate(_container, (c, action) => action(c));
            return new Client<TPackage>(container);
        }
    }

    public class TcpClientBuilder<TPackage> : ClientBuilder<TPackage, TcpClientBuilder<TPackage>>
    {

        public TcpClientBuilder()
        {
            _funcs.Add(c =>
            {
                c.Register<IChannelOption>(new ChannelOption());
                c.Register<IConnector<TPackage>, Connector<TPackage>>(EnumLifetime.Transient);
                return c;
            });
        }

        public TcpClientBuilder<TPackage> HandlePackage(Action<ISession<TPackage>, TPackage> action)
        {
            _funcs.Add(c =>
            {
                var middle = new ClientHandleMiddleware<TPackage>(action);
                c.Register<IMiddleware<TPackage>>(middle);
                return c;
            });
            return this;
        }


        public TcpClientBuilder<TPackage> UseConnect(Action<ConnectorOption> optionaction)
        {
            _funcs.Add(c =>
            {
                var option = new ConnectorOption();
                optionaction(option);
                c.Register<IConnectorOption>(option);
                return c;
            });
            return this;
        }

        public TcpClientBuilder<TPackage> UseRamp<TPackageRamp>() where TPackageRamp : IPackageRamp<TPackage>
        {
            _funcs.Add(c =>
            {

                c.Register<IPackageRamp<TPackage>, TPackageRamp>(EnumLifetime.Transient);
                return c;
            });
            return this;
        }

        public TcpClientBuilder<TPackage> UseTcp(string ip, int port, Action<ISession<TPackage>, TPackage> handle)
        {
            UseConnect((o) =>
            {
                o.Ip = ip;
                o.Port = port;
            });
            HandlePackage(handle);
            return this;
        }


    }






}
