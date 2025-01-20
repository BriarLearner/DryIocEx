using System;
using System.Collections.Concurrent;
using System.Threading;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Event;

public static class EventLocator
{
    private static IEventManager _manager;

    private static string _innerLogName;

    public static IEventManager EventManager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetEventManager(IEventManager manager)
    {
        throw new NotImplementedException();
    }
}

public static class EventExtensions
{
    public static TEvent GetEvent<TEvent>(this object obj) where TEvent : BaseEvent, new()
    {
        throw new NotImplementedException();
    }

    public static IContainer UserEventManager(this IContainer container)
    {
        throw new NotImplementedException();
    }
}

public interface IEventManager
{
    TEvent GetEvent<TEvent>() where TEvent : BaseEvent, new();
}

public class EventManager : IEventManager
{
    private readonly ConcurrentDictionary<Type, IEvent> _eventDict = new();
    private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

    /// <summary>
    ///     所有从管理器中获得的事件其Context都是创建管理器的Context。
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public TEvent GetEvent<TEvent>() where TEvent : BaseEvent, new()
    {
        throw new NotImplementedException();
    }
}