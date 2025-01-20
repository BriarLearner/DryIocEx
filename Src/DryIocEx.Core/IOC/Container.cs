using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.Common;

namespace DryIocEx.Core.IOC;

public class Container : IDisposable, IContainer
{
    /// <summary>
    ///     所有已经创建的实例，包含Singleton和Scoped
    /// </summary>
    private readonly ConcurrentDictionary<InstanceKey, object> _instances = new();


    private volatile bool _disposed;

    public Container()
    {
        throw new NotImplementedException();
    }

    public Container(IContainer parent)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     根容器
    /// </summary>
    public IContainer Root { get; }


    /// <summary>
    ///     所有注入的对象，
    /// </summary>
    public IDictionary<KeyInfo, RegistryInfo> Registries { get; }

    public IDictionary<InstanceKey, object> Instances => _instances;


    /// <summary>
    ///     临时创建的对象 并实现了IDisposable接口
    /// </summary>
    public ConcurrentBag<WeakReference<IDisposable>> Disposables { get; } = new();


    /// <summary>
    ///     是否已经注入
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool HasRegister(KeyInfo info)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     只需要实现一个核心的、功能最全的方法，其余方法都使用扩展方法实现就可以了
    /// </summary>
    /// <param name="registry"></param>
    /// <returns></returns>
    public IContainer Register(RegistryInfo registry)
    {
        throw new NotImplementedException();
    }


    public object Resolve(KeyInfo key)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private object InnerGetService(RegistryInfo registry, Type[] genericargs)
    {
        throw new NotImplementedException();
    }

    private void NotDisposed()
    {
        throw new NotImplementedException();
    }
}

public class ContainerBuilder : BaseBuilder<ContainerBuilder, IContainer>
{
    private readonly IContainer _container;

    public ContainerBuilder() : this(new Container())
    {
    }


    public ContainerBuilder(IContainer container)
    {
        throw new NotImplementedException();
    }

    public override IContainer Build()
    {
        throw new NotImplementedException();
    }

    public ContainerBuilder Register(Assembly assembly)
    {
        throw new NotImplementedException();
    }
}