#if NET


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Util;

namespace DryIocEx.Core.Network;

public interface ISessionOption
{
    string Name { get; set; }
}

public interface ISessionStateChanged<TSession>
{
    Action<TSession, EnumConnectState> StateChanged { set; get; }

    ValueTask Handle(TSession session, EnumConnectState state);
}

public class SessionStateChanged<TSession> : ISessionStateChanged<TSession>
    where TSession : ISession
{
    public Action<TSession, EnumConnectState> StateChanged { get; set; }

    public ValueTask Handle(TSession session, EnumConnectState state)
    {
        throw new NotImplementedException();
    }
}

public enum EnumConnectState
{
    Started,
    Stoped
}

public class SessionOption : ISessionOption
{
    public string Name { get; set; }
}

public interface ISessionFactory
{
    ISession<TPackage> Create<TPackage>(INetworkContainer container, IChannel<TPackage> channel);
}

public interface ISession
{
    ISessionOption Option { set; get; }
    string ID { get; }
    bool IsStarted { get; }
    INetworkContainer Container { get; }

    DateTime StartTime { get; set; }
    DateTime EndTime { get; set; }

    ValueTask SendAsync(ReadOnlyMemory<byte> data);
    void Start();

    ValueTask StopAsync(StopReason reason);

    IChannel GetChannel();
}

public interface ISession<TPackage> : ISession
{
    IChannel<TPackage> Channel { get; set; }
    ValueTask SendAsync(TPackage package);

    ValueTask<TPackage> ReceiveAsync();
    IAsyncEnumerable<TPackage> RunAsync();
}

public class SessionFactory : ISessionFactory
{
    public ISession<TPackage> Create<TPackage>(INetworkContainer container, IChannel<TPackage> channel)
    {
        var session = new Session<TPackage>(container);
        return session;
    }
}

public class Session<TPackage> : ISession<TPackage>
{
    public Session(INetworkContainer container)
    {
        throw new NotImplementedException();
    }

    public string ID { get; }
    public bool IsStarted => !Channel.IsStopped;
    public INetworkContainer Container { get; }
    public IChannel<TPackage> Channel { get; set; }

    public ISessionOption Option { get; set; }

    public IChannel GetChannel()
    {
        return Channel;
    }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(TPackage data)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TPackage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TPackage> RunAsync()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public async ValueTask StopAsync(StopReason reason)
    {
        throw new NotImplementedException();
    }


    public async ValueTask StopAsync()
    {
        throw new NotImplementedException();
    }
}

#endif