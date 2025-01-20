using System;
using System.Reflection;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Event;

public interface IDelegateReference
{
    Delegate Method { get; }
}

public class DelegateReference : IDelegateReference
{
    private readonly Delegate _delegate;
    private readonly Type _delegateType;
    private readonly MethodInfo _method;
    private readonly WeakReference _weakReference;

    public DelegateReference(Delegate action, bool keepalive)
    {
        throw new NotImplementedException();
    }

    public Delegate Method
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool TargetEquals(Delegate action)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     弱引用和静态尝试获取方法
    /// </summary>
    /// <returns></returns>
    private Delegate tryGetDelegate()
    {
        throw new NotImplementedException();
    }
}

public static class ReferenceExtension
{
    public static IDelegateReference ToReference(this Delegate action, bool keepalive)
    {
        throw new NotImplementedException();
    }
}