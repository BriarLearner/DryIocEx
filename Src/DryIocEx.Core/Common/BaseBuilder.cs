using System;
using System.Collections.Generic;
using System.Linq;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Common;

/// <summary>
///     工厂基类
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TSubject"></typeparam>
public class BaseBuilder<TSelf, TSubject>
    where TSelf : BaseBuilder<TSelf, TSubject>
    where TSubject : class
{
    protected List<Func<TSubject, TSubject>> _list = new();

    public virtual TSelf Do(Action<TSubject> action)
    {
        throw new NotImplementedException();
    }

    public virtual TSubject Build()
    {
        throw new NotImplementedException();
    }
}

public class BaseAction<TSelf, TSubject> where TSelf : BaseAction<TSelf, TSubject>
{
    public BaseAction(TSubject subject)
    {
        throw new NotImplementedException();
    }

    protected TSubject Subject { get; set; }
}