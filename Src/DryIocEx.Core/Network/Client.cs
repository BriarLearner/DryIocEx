#if NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Event;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Network;

public interface INetworkContainer
{
    IContainer Container { get; }

    /// <summary>
    ///     T 可以是类 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetMiddleware<T>();

    ValueTask StopAsync();
    ValueTask<bool> StartAsync();
}

public interface IClient : INetworkContainer
{
    EnumClientState State { get; }

    ValueTask SendAsync(ReadOnlyMemory<byte> data);

    ValueTask<bool> ConnectAsync();

    ValueTask DisconnectedAsync();
}

public interface IClient<TPackage> : IClient
{
    ISession<TPackage> Session { get; }
    List<IClientMiddleware<TPackage>> Middlewares { get; }
    ValueTask<TPackage> ReceiveAsync();
    ValueTask SendAsync(TPackage package);
}

public enum EnumClientState
{
    Disconnected,
    Connected
}

public interface IClientStateChanged<TClient>
{
    Action<TClient, EnumClientState> StateChanged { set; get; }

    ValueTask Handle(TClient server, EnumClientState state);
}

public class ClientStateChanged<TClient> : IClientStateChanged<TClient>
    where TClient : IClient
{
    public Action<TClient, EnumClientState> StateChanged { get; set; }

    public ValueTask Handle(TClient client, EnumClientState state)
    {
        throw new NotImplementedException();
    }
}

public class Client<TPackage> : IClient<TPackage>
{
    public Client(IContainer container)
    {
        throw new NotImplementedException();
    }

    public IPackageEncoder<TPackage> Encoder { get; protected set; }

    public ISessionStateChanged<ISession<TPackage>> SessionStateChanged { get; set; }


    private IClientStateChanged<IClient<TPackage>> ClientStateChanged { get; }
    public IContainer Container { get; protected set; }

    public EnumClientState State { protected set; get; }


    public async ValueTask<TPackage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }


    public ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }


    public async ValueTask<bool> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask DisconnectedAsync()
    {
        throw new NotImplementedException();
    }


    public ISession<TPackage> Session { get; protected set; }

    public async ValueTask<bool> ConnectAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public void OnStopped(object obj, StopEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void OnClientStateChanged(EnumClientState state)
    {
        throw new NotImplementedException();
    }

    private void OnSessionStateChanged(ISession<TPackage> session, EnumConnectState state)
    {
        throw new NotImplementedException();
    }

    private void OnMiddlewareDisconnected()
    {
        throw new NotImplementedException();
    }

    #region Middleware

    public List<IClientMiddleware<TPackage>> Middlewares => _middlewareDict.Values.ToList();

    private readonly Dictionary<Type, IClientMiddleware<TPackage>> _middlewareDict = new();

    /// <summary>
    ///     T 可以是类 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetMiddleware<T>()
    {
        throw new NotImplementedException();
    }

    #endregion
}

public interface IClientOption
{
}

public class ClientOption : IClientOption
{
}

public abstract class ClientBuilder<Self, TPackage>
    where Self : ClientBuilder<Self, TPackage>
{
    protected List<Func<IContainer, IContainer>> _list = new();

    public ClientBuilder()
    {
        throw new NotImplementedException();
    }

    protected IContainer Container { get; }

    public virtual Self Config(Action<IContainer> action)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseOption(Action<ClientOption> action)
    {
        throw new NotImplementedException();
    }


    public virtual Self UseSessionOption(Action<SessionOption> action)
    {
        throw new NotImplementedException();
    }

    public virtual Self UsePackageFilterFactory<TPackageFilterFactory>()
        where TPackageFilterFactory : class, IPackageFilterFactory<TPackage>
    {
        throw new NotImplementedException();
    }

    public virtual Self UsePackageFilter<TPackageFilter>() where TPackageFilter : class, IPackageFilter<TPackage>

    {
        throw new NotImplementedException();
    }

    public virtual Self UseMiddleware<TMiddleware>() where TMiddleware : class, IClientMiddleware<TPackage>
    {
        throw new NotImplementedException();
    }

    public virtual Self UseSessionChanged(Action<ISession<TPackage>, EnumConnectState> handle)
    {
        throw new NotImplementedException();
        ;
    }

    public virtual Self UseServerChanged(Action<IClient<TPackage>, EnumClientState> action)
    {
        throw new NotImplementedException();
    }


    public virtual Self UsePackageHandle<TPackage>(Action<ISession<TPackage>, TPackage> handle)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseConsoleLog()
    {
        throw new NotImplementedException();
    }

    public virtual Self UseFileLog(Action<FileLogOption> action)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseLog(Func<IContainer, ILogger> logfactory)
    {
        throw new NotImplementedException();
    }


    public IClient<TPackage> Build()
    {
        throw new NotImplementedException();
    }

    protected virtual void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}

public class SimpleClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> : ClientBuilder<
    SimpleClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder>, TPackage>
    where TPackageFilter : IPackageFilter<TPackage>
    where TEncoder : IPackageEncoder<TPackage>
    where TDecoder : IPackageDecoder<TPackage>
{
    public SimpleClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> ConfigureConnector(
        Action<IContainer, ConnectorOption> action)
    {
        throw new NotImplementedException();
    }

    protected override void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}

public class UdpClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> : ClientBuilder<
    UdpClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder>, TPackage>
    where TPackageFilter : IPackageFilter<TPackage>
    where TEncoder : IPackageEncoder<TPackage>
    where TDecoder : IPackageDecoder<TPackage>
{
    public UdpClientBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> ConfigureConnector(
        Action<IContainer, ConnectorOption> action)
    {
        throw new NotImplementedException();
    }

    protected override void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}

#endif