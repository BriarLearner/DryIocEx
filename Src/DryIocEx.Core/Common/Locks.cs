using System;
using System.Threading;

namespace DryIocEx.Core.Common;

public class SGLock : IDisposable
{
    private AutoResetEvent _waiterLock = new(false);
    private int _waiters;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        _waiterLock?.Dispose();
        _waiterLock = null;
    }

    ~SGLock()
    {
        Dispose(false);
    }

    public void Enter()
    {
        if (Interlocked.Increment(ref _waiters) == 1) return;
        _waiterLock.WaitOne();
    }

    public void Leave()
    {
        if (Interlocked.Decrement(ref _waiters) == 0) return;
        _waiterLock.Set();
    }
}

public class SGSpinLock
{
    /// <summary>
    ///     自旋5pin
    /// </summary>
    private readonly int _spinCount = 50;

    private AutoResetEvent _waiterLock = new(false);
    private int _waiters;

    public void Enter()
    {
        if (Interlocked.CompareExchange(ref _waiters, 1, 0) == 0) return;
        var spinwait = new SpinWait();
        for (var spinCount = 0; spinCount < _spinCount; spinCount++)
        {
            if (Interlocked.CompareExchange(ref _waiters, 1, 0) == 0) return;
            spinwait.SpinOnce();
        }

        if (Interlocked.Increment(ref _waiters) > 1) _waiterLock.WaitOne();
    }

    public void Leave()
    {
        if (Interlocked.Decrement(ref _waiters) == 0) return;
        _waiterLock.Set();
    }

    public void Dispose()
    {
        _waiterLock?.Dispose();
        _waiterLock = default;
    }
}

public class SGSpinRecursionLock
{
    private readonly int _spincount = 50; //20ms
    private int _owningThreadId, _recursion;
    private AutoResetEvent _waiterLock = new(false);
    private int _waiters;

    public void Enter()
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        if (threadId == _owningThreadId)
        {
            _recursion++;
            return;
        }

        if (Interlocked.CompareExchange(ref _waiters, 1, 0) == 0) goto GotLock;
        var spinwait = new SpinWait();
        for (var spinCount = 0; spinCount < _spincount; spinCount++)
        {
            if (Interlocked.CompareExchange(ref _waiters, 1, 0) == 0) goto GotLock;
            spinwait.SpinOnce();
        }

        if (Interlocked.Increment(ref _waiters) > 1) _waiterLock.WaitOne();
        GotLock:
        _owningThreadId = threadId;
        _recursion = 1;
    }

    public void Leave()
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        if (threadId != _owningThreadId)
            throw new SynchronizationLockException("SpinRecursionHybridLock not owned by calling thread");
        if (--_recursion > 0) return;
        _owningThreadId = 0;
        if (Interlocked.Decrement(ref _waiters) == 0)
            return;
        _waiterLock.Set();
    }

    public void Dispose()
    {
        _waiterLock?.Dispose();
        _waiterLock = default;
    }
}