using System;
using System.Collections.Generic;

namespace DryIocEx.Core.Common;

/// <summary>
///     简单容器
/// </summary>
public class ActorStore
{
    internal Dictionary<Type, ActorInfo> _actors = new();

    public TActor GetActor<TActor>()
    {
        throw new NotImplementedException();
    }


    public void AddActor<TActor>(Func<TActor> func)
    {
        throw new NotImplementedException();
    }

    public void AddActor<TActor>(TActor obj)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     容器基类
/// </summary>
public class BaseActorStore
{
    internal Dictionary<Type, ActorInfo> _modules = new();

    protected TActor GetActor<TActor>()
    {
        throw new NotImplementedException();
    }

    public void AddActor<TActor>(Func<TActor> func)
    {
        throw new NotImplementedException();
    }

    public void AddActor<TActor>(TActor obj)
    {
        throw new NotImplementedException();
    }
}

internal class ActorInfo
{
    private object _instance;

    public ActorInfo(object instance)
    {
        throw new NotImplementedException();
    }

    public Func<object> Factory { get; }

    public object Instance
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}