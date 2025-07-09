using System;
using System.Threading;

namespace DryIocEx.Core.Common;

public class SGLock : IDisposable
{
    private AutoResetEvent _waiterLock = new(false);
    private int _waiters;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private void Dispose(bool disposing)
    {
        throw new NotImplementedException();
    }

    ~SGLock()
    {
        throw new NotImplementedException();
    }

    public void Enter()
    {
        throw new NotImplementedException();
    }

    public void Leave()
    {
       throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void Leave()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void Leave()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _waiterLock?.Dispose();
        _waiterLock = default;
    }
}