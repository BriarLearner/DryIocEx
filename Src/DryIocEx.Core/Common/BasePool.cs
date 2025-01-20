using System;
using System.Collections.Concurrent;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Common;

public class Pool<TSubject> : IPool<TSubject> where TSubject : class, new()
{
    protected ConcurrentQueue<TSubject> _queue = new();

    protected Func<TSubject> Factory { get; set; }

    public virtual TSubject Rent()
    {
        throw new NotImplementedException();
    }

    public virtual void Return(TSubject sub)
    {
        throw new NotImplementedException();
    }
}

public interface IPool<TSubject> where TSubject : class, new()
{
    TSubject Rent();
    void Return(TSubject sub);
}