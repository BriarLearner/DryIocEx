using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.NetworkPro;

public interface ISessionOption : IOption
{
}

public class SessionOption : ISessionOption
{
    public object Tag { set; get; }
}

public interface ISession<TPackage>
{
    public delegate ValueTask SessionStopHandle(ISession<TPackage> session, StopReason reason);

    bool IsStop { get; }

    public IChannel<TPackage> Channel { get; }

    public event SessionStopHandle SessionStop;

    ValueTask SendAsync(byte[] buffer);

    ValueTask SendAsync(TPackage package);

    void Start();
    void Stop(StopReason reason = StopReason.LocalClosing);
    IAsyncEnumerable<TPackage> RunAsync();
    ValueTask<TPackage> ReceiveAsync();
    public TKey GetKey<TKey>();
    public TOption GetOption<TOption>() where TOption : ISessionOption;
}

public class Session<TPackage> : ISession<TPackage>
{
    private readonly object _key;
    private readonly SessionOption _option;

    public Session(IChannel<TPackage> channel, object key)
    {
        throw new NotImplementedException();
    }

    public IContainer Container { get; }

    /// <summary>
    ///     Udp Listener需要访问
    /// </summary>
    public IChannel<TPackage> Channel { get; }

    public event ISession<TPackage>.SessionStopHandle SessionStop;

    public bool IsStop { get; private set; }

    public ValueTask SendAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop(StopReason reason = StopReason.LocalClosing)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TPackage> RunAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask<TPackage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }

    public TKey GetKey<TKey>()
    {
        throw new NotImplementedException();
    }

    public TOption GetOption<TOption>() where TOption : ISessionOption
    {
        throw new NotImplementedException();
    }

    private ValueTask OnChannelStop(IChannel<TPackage> channel, StopReason reason)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnSessionStop(StopReason reason)
    {
        throw new NotImplementedException();
    }
}