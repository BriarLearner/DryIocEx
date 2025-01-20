#if NET

//https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/preprocessor-directives?devlangs=csharp&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-CN%26k%3Dk(%2523define)%3Bk(DevLang-csharp)%26rd%3Dtrue

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Network;

public interface IServer : INetworkContainer
{
    /// <summary>
    ///     状态
    /// </summary>
    EnumServerState State { get; }
}

public interface IServer<TPackage> : IServer
{
    IPackageHandle<ISession<TPackage>, TPackage> PackageHandle { get; }
    List<IServerMiddleware<TPackage>> Middlewares { get; }

    IDictionary<string, ISession<TPackage>> Sessions { get; }

    /// <summary>
    ///     获取对应Session
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ISession<TPackage> GetSession(string key);
}

public enum EnumServerState
{
    Stopped = 0,
    Started = 1
}

public interface IServerStateChanged<TServer>
{
    Action<TServer, EnumServerState> StateChanged { set; get; }

    ValueTask Handle(TServer server, EnumServerState state);
}

public class ServerStateChanged<TServer> : IServerStateChanged<TServer>
    where TServer : IServer
{
    public Action<TServer, EnumServerState> StateChanged { get; set; }

    public ValueTask Handle(TServer server, EnumServerState state)
    {
        throw new NotImplementedException();
    }
}

public class Server<TPackage> : IServer<TPackage>
{
    public Server(IContainer container)
    {
        throw new NotImplementedException();
    }

    private IServerStateChanged<IServer<TPackage>> ServerStateChanged { get; }
    public IContainer Container { get; }
    public EnumServerState State { get; protected set; }

    #region Start/Stop

    public virtual async ValueTask<bool> StartAsync()
    {
        throw new NotImplementedException();
    }

    private async ValueTask<bool> CreateChannelCreator()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     服务停止
    /// </summary>
    /// <returns></returns>
    public virtual async ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }

    public IPackageHandle<ISession<TPackage>, TPackage> PackageHandle { get; set; }

    #endregion


    #region ChannelCreator

    public IList<IChannelCreator> Creators { get; } = new List<IChannelCreator>();

    private async void OnNewChannelCreator(IChannelCreator creator, IChannel channel)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Session

    public ISessionStateChanged<ISession<TPackage>> SessionStateChanged { get; set; }
    public IDictionary<string, ISession<TPackage>> Sessions { get; } = new Dictionary<string, ISession<TPackage>>();

    public ISession<TPackage> GetSession(string key)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Middleware

    private Dictionary<Type, IServerMiddleware<TPackage>> _middlewareDict { get; } = new();


    public List<IServerMiddleware<TPackage>> Middlewares => _middlewareDict.Values.ToList();

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

public abstract class ServerBuilder<Self, TPackage>
    where Self : ServerBuilder<Self, TPackage>
{
    protected List<Func<IContainer, IContainer>> _list = new();

    public ServerBuilder()
    {
        throw new NotImplementedException();
    }

    public ServerBuilder(IContainer container)
    {
        throw new NotImplementedException();
    }

    protected IContainer Container { get; }

    public virtual Self Config(Action<IContainer> action)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseOption(Action<ServerOption> action)
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

    public virtual Self UseSessionFactory<TSessionFactory>() where TSessionFactory : class, ISessionFactory
    {
        throw new NotImplementedException();
    }

    public virtual Self UseSession<TSession>() where TSession : class, ISession
    {
        throw new NotImplementedException();
    }

    public virtual Self UseMiddleware<TMiddleware>() where TMiddleware : class, IServerMiddleware<TPackage>
    {
        throw new NotImplementedException();
    }

    public virtual Self UseChannelCreator<TChannelCreator>() where TChannelCreator : class, IChannelCreator
    {
        throw new NotImplementedException();
    }

    public virtual Self UsePackageHandle(Action<ISession<TPackage>, TPackage> handle)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseSessionChanged(Action<ISession<TPackage>, EnumConnectState> handle)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseServerChanged(Action<IServer<TPackage>, EnumServerState> action)
    {
        throw new NotImplementedException();
    }


    public virtual Self UseConsoleLog()
    {
        throw new NotImplementedException();
    }

    public virtual Self UserFileLog(Action<FileLogOption> action)
    {
        throw new NotImplementedException();
    }

    public virtual Self UseLog(Func<IContainer, ILogger> logfactory)
    {
        throw new NotImplementedException();
    }

    public IServer<TPackage> Build()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     该方法中不能使用Use等方法
    /// </summary>
    /// <param name="container"></param>
    protected virtual void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}

public class SimpleServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> : ServerBuilder<
    SimpleServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder>, TPackage>
    where TPackageFilter : IPackageFilter<TPackage>
    where TEncoder : IPackageEncoder<TPackage>
    where TDecoder : IPackageDecoder<TPackage>
{
    public SimpleServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> ConfigureListen(
        Action<IContainer, TcpChannelCreatorOption> action)
    {
        throw new NotImplementedException();
    }

    protected override void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}

public class UdpServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> : ServerBuilder<
    UdpServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder>, TPackage>
    where TPackageFilter : IPackageFilter<TPackage>
    where TEncoder : IPackageEncoder<TPackage>
    where TDecoder : IPackageDecoder<TPackage>
{
    public UdpServerBuilder<TPackage, TPackageFilter, TEncoder, TDecoder> ConfigureListen(
        Action<IContainer, ChannelCreatorOption> action)
    {
        throw new NotImplementedException();
    }

    protected override void DefaultRegister(IContainer container)
    {
        throw new NotImplementedException();
    }
}
#endif