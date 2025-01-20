using System;
using System.Collections.Generic;
using System.Threading;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Event;

public interface IEvent
{
    SynchronizationContext SynchronizationContext { get; set; }
    void Clear();
}

public static class EventExtension
{
    /// <summary>
    ///     默认发布线程调用
    ///     默认keepalive为false
    /// </summary>
    /// <param name="pse"></param>
    /// <param name="action"></param>
    public static void Subscribe(this PubSubEvent pse, Action action)
    {
        throw new NotImplementedException();
    }

    public static void Subscribe(this PubSubEvent pse, Action action, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public static void Subscribe(this PubSubEvent pse, Action action, EnumThreadType type)
    {
        throw new NotImplementedException();
    }

    public static void Subscribe<TArg>(this PubSubEvent<TArg> pse, Action<TArg> action)
    {
        throw new NotImplementedException();
    }

    public static void Subscribe<TArg>(this PubSubEvent<TArg> pse, Action<TArg> action, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public static void Subscribe<TArg1, TArg2>(this PubSubEvent<TArg1, TArg2> pse, Action<TArg1, TArg2> action,
        EnumThreadType type)
    {
        throw new NotImplementedException();
    }
}

public abstract class BaseEvent : IEvent
{
    /// <summary>
    ///     使用 Lock效率不够高，但是足够优雅，也不需要手动Dispose
    /// </summary>
    protected readonly List<ISubscription> _subscriptions = new();

    protected SynchronizationContext _synchronizationContext;

    public SynchronizationContext SynchronizationContext
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     取消订阅
    /// </summary>
    /// <param name="delegate"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void InnerUnSubscribe(Delegate @delegate)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     订阅
    /// </summary>
    /// <param name="delegate"></param>
    /// <param name="type"></param>
    /// <param name="keepalive"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void InnerSubscribe(Delegate @delegate, EnumThreadType type, bool keepalive)
    {
        throw new NotImplementedException();
    }

    protected abstract ISubscription GetSubscription(IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context);

    /// <summary>
    ///     发布
    /// </summary>
    /// <param name="args"></param>
    protected void InnerPublish(params object[] args)
    {
        throw new NotImplementedException();
    }

    private void PureSubscribe()
    {
        throw new NotImplementedException();
    }
}

public class PubSubEvent : BaseEvent
{
    public PubSubEvent() : this(SynchronizationContext.Current)
    {
    }

    public PubSubEvent(SynchronizationContext synchronizationContext)
    {
        throw new NotImplementedException();
    }


    public void Subscribe(Action action, EnumThreadType type, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public void UnSubscribe(Action action)
    {
        throw new NotImplementedException();
    }

    public void Publish()
    {
        throw new NotImplementedException();
    }

    protected override ISubscription GetSubscription(IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     自动保存创建线程的异步上下文
/// </summary>
/// <typeparam name="TArgs"></typeparam>
public class PubSubEvent<TArgs> : BaseEvent
{
    public PubSubEvent() : this(SynchronizationContext.Current)
    {
    }

    public PubSubEvent(SynchronizationContext synchronizationContext)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<TArgs> action, EnumThreadType type, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public void UnSubscribe(Action<TArgs> action)
    {
        throw new NotImplementedException();
    }

    public void Publish(TArgs args)
    {
        throw new NotImplementedException();
    }

    protected override ISubscription GetSubscription(IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context)
    {
        throw new NotImplementedException();
    }
}

public class PubSubEvent<TArg1, TArg2> : BaseEvent
{
    public PubSubEvent() : this(SynchronizationContext.Current)
    {
    }

    public PubSubEvent(SynchronizationContext synchronizationContext)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<TArg1, TArg2> action, EnumThreadType type, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public void UnSubscribe(Action<TArg1, TArg2> action)
    {
        throw new NotImplementedException();
    }

    public void Publish(TArg1 arg1, TArg2 arg2)
    {
        throw new NotImplementedException();
    }

    protected override ISubscription GetSubscription(IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context)
    {
        throw new NotImplementedException();
    }
}

public class PubSubEvent<TArg1, TArg2, TArg3> : BaseEvent
{
    public PubSubEvent() : this(SynchronizationContext.Current)
    {
    }

    public PubSubEvent(SynchronizationContext synchronizationContext)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<TArg1, TArg2, TArg3> action, EnumThreadType type, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public void UnSubscribe(Action<TArg1, TArg2, TArg3> action)
    {
        throw new NotImplementedException();
    }

    public void Publish(TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        throw new NotImplementedException();
    }

    protected override ISubscription GetSubscription(IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context)
    {
        throw new NotImplementedException();
    }
}