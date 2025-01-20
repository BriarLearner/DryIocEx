using System;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Event;

public interface ISubscription
{
    Delegate Delegate { get; }
    Action<object[]> GetExecutionAction();
}

public enum EnumThreadType
{
    Background,
    Publisher,
    Context
}

public abstract class Subscription : ISubscription
{
    private readonly IDelegateReference _delegateReference;

    protected Subscription(IDelegateReference delegateReference)
    {
        throw new NotImplementedException();
    }

    public Delegate Delegate => _delegateReference.Method;

    public virtual Action<object[]> GetExecutionAction()
    {
        throw new NotImplementedException();
    }

    protected abstract void InvokeAction(Action action);
}

public abstract class Subscription<TArgs> : ISubscription
{
    private readonly IDelegateReference _delegateReference;

    protected Subscription(IDelegateReference delegateReference)
    {
        throw new NotImplementedException();
    }

    public Delegate Delegate => _delegateReference.Method;

    public Action<object[]> GetExecutionAction()
    {
        throw new NotImplementedException();
    }

    protected abstract void InvokeAction(Action<TArgs> action, TArgs args);
}

public abstract class Subscription<TArg1, TArg2> : ISubscription
{
    private readonly IDelegateReference _delegateReference;

    protected Subscription(IDelegateReference delegateReference)
    {
        throw new NotImplementedException();
    }

    public Delegate Delegate => _delegateReference.Method;

    public Action<object[]> GetExecutionAction()
    {
        throw new NotImplementedException();
    }

    protected abstract void InvokeAction(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2);
}

public abstract class Subscription<TArg1, TArg2, TArg3> : ISubscription
{
    private readonly IDelegateReference _delegateReference;

    protected Subscription(IDelegateReference delegateReference)
    {
        throw new NotImplementedException();
    }

    public Delegate Delegate => _delegateReference.Method;

    public Action<object[]> GetExecutionAction()
    {
        throw new NotImplementedException();
    }

    protected abstract void InvokeAction(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3);
}

public class BackgroundSubscription : Subscription
{
    public BackgroundSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action action)
    {
        throw new NotImplementedException();
    }
}

public class BackgroundSubscription<TArgs> : Subscription<TArgs>
{
    public BackgroundSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArgs> action, TArgs args)
    {
        throw new NotImplementedException();
    }
}

public class BackgroundSubscription<TArg1, TArg2> : Subscription<TArg1, TArg2>
{
    public BackgroundSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        throw new NotImplementedException();
    }
}

public class BackgroundSubscription<TArg1, TArg2, TArg3> : Subscription<TArg1, TArg2, TArg3>
{
    public BackgroundSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        throw new NotImplementedException();
    }
}

public class PublishSubscription : Subscription
{
    public PublishSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action action)
    {
        throw new NotImplementedException();
    }
}

public class PublishSubscription<TArgs> : Subscription<TArgs>
{
    public PublishSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArgs> action, TArgs args)
    {
        throw new NotImplementedException();
    }
}

public class PublishSubscription<TArg1, TArg2> : Subscription<TArg1, TArg2>
{
    public PublishSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        throw new NotImplementedException();
    }
}

public class PublishSubscription<TArg1, TArg2, TArg3> : Subscription<TArg1, TArg2, TArg3>
{
    public PublishSubscription(IDelegateReference delegateReference) : base(delegateReference)
    {
    }

    protected override void InvokeAction(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        throw new NotImplementedException();
    }
}

public class ContextSubscription : Subscription
{
    private readonly SynchronizationContext _synchronizationContext;

    public ContextSubscription(IDelegateReference delegateReference, SynchronizationContext context) : base(
        delegateReference)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeAction(Action action)
    {
        throw new NotImplementedException();
    }
}

public class ContextSubscription<TArgs> : Subscription<TArgs>
{
    private readonly SynchronizationContext _synchronizationContext;

    public ContextSubscription(IDelegateReference delegateReference, SynchronizationContext context) : base(
        delegateReference)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeAction(Action<TArgs> action, TArgs args)
    {
        throw new NotImplementedException();
    }
}

public class ContextSubscription<TArg1, TArg2> : Subscription<TArg1, TArg2>
{
    private readonly SynchronizationContext _synchronizationContext;

    public ContextSubscription(IDelegateReference delegateReference, SynchronizationContext context) : base(
        delegateReference)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeAction(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        throw new NotImplementedException();
    }
}

public class ContextSubscription<TArg1, TArg2, TArg3> : Subscription<TArg1, TArg2, TArg3>
{
    private readonly SynchronizationContext _synchronizationContext;

    public ContextSubscription(IDelegateReference delegateReference, SynchronizationContext context) : base(
        delegateReference)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeAction(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        throw new NotImplementedException();
    }
}

public static class SubscriptionExtensions
{
    public static ISubscription ToSubscription(this IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context = null)
    {
        throw new NotImplementedException();
    }


    public static ISubscription ToSubscription<TArg>(this IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context = null)
    {
        throw new NotImplementedException();
    }

    public static ISubscription ToSubscription<TArg1, TArg2>(this IDelegateReference reference, EnumThreadType type,
        SynchronizationContext context = null)
    {
        throw new NotImplementedException();
    }

    public static ISubscription ToSubscription<TArg1, TArg2, TArg3>(this IDelegateReference reference,
        EnumThreadType type,
        SynchronizationContext context = null)
    {
        throw new NotImplementedException();
    }
}