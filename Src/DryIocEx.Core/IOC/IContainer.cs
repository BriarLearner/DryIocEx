using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.IOC;

public interface IContainer
{
    /// <summary>
    ///     根
    /// </summary>
    IContainer Root { get; }

    /// <summary>
    ///     注入信息
    /// </summary>
    IDictionary<KeyInfo, RegistryInfo> Registries { get; }

    /// <summary>
    ///     Disposables
    /// </summary>
    ConcurrentBag<WeakReference<IDisposable>> Disposables { get; }

    /// <summary>
    ///     所有创建的实例
    /// </summary>
    IDictionary<InstanceKey, object> Instances { get; }

    /// <summary>
    ///     注入
    /// </summary>
    /// <param name="registry"></param>
    /// <returns></returns>
    IContainer Register(RegistryInfo registry);

    /// <summary>
    ///     解析
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    object Resolve(KeyInfo key);

    /// <summary>
    ///     是否注入,无论生命周期
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    bool HasRegister(KeyInfo info);
}

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Interface | AttributeTargets.Field)]
public class RegisterAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AutoRegisterAttribute : Attribute
{
    public AutoRegisterAttribute(Type fromType, EnumLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }

    public string Name { get; }
    public Type FromType { get; }

    public EnumLifetime Lifetime { get; }
}

public static class ContainerLocator
{
    private static IContainer _container;
    public static IContainer Container => _container ??= new Container();

    public static void SetContainer(IContainer container)
    {
        throw new NotImplementedException();
    }
}

public static class ContainerExtension
{
    /// <summary>
    ///     从ContainerLocator中获取对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetInstance<T>(this object obj, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     从ContainerLocator中获取对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Get<T>(this object obj, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     检测某类型是否已经注入，不检查生命周期
    /// </summary>
    /// <param name="container"></param>
    /// <param name="from"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool HasRegister(this IContainer container, Type from, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     检测某类型是否已经注入，不检查生命周期
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool HasRegister<TFrom>(this IContainer container, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     检测注入，如果没有注入过再进行注入，不检测生命周期
    /// </summary>
    /// <param name="container"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer TryRegister(this IContainer container, Type from, Type to, EnumLifetime lifetime,
        string name = "")
    {
        throw new NotImplementedException();
    }

    public static IContainer TryRegister<TFrom, TTo>(this IContainer container, EnumLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     注入基础实现
    /// </summary>
    /// <param name="container"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register(this IContainer container, Type from, Type to, EnumLifetime lifetime,
        string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     注入泛型扩展
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="container"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register<TFrom, TTo>(this IContainer container, EnumLifetime lifetime,
        string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     自动检测接口注入 基础实现
    /// </summary>
    /// <param name="container"></param>
    /// <param name="fromto"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register(this IContainer container, Type fromto, EnumLifetime lifetime,
        string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     自动注入类接口 泛型实现
    /// </summary>
    /// <typeparam name="TFromTo"></typeparam>
    /// <param name="container"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register<TFromTo>(this IContainer container, EnumLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     单例模式基础实现
    /// </summary>
    /// <param name="container"></param>
    /// <param name="fromtype"></param>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register(this IContainer container, Type fromtype, object instance, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     单例模式泛型模式
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <param name="container"></param>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register<TFrom>(this IContainer container, TFrom instance, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     工厂基础模式
    /// </summary>
    /// <param name="container"></param>
    /// <param name="fromtype"></param>
    /// <param name="factory"></param>
    /// <param name="lifetime"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IContainer Register(this IContainer container, Type fromtype, Func<IContainer, object> factory,
        EnumLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    public static IContainer Register<TFrom>(this IContainer container, Func<IContainer, TFrom> factory,
        EnumLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     注入整个程序集
    /// </summary>
    /// <param name="contaienr"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IContainer Register(this IContainer contaienr, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static IContainer CreateChild(this IContainer container)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     InnerFactory
    /// </summary>
    /// <param name="container"></param>
    /// <param name="totype"></param>
    /// <param name="genericargs"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static object Create(IContainer container, Type totype, Type[] genericargs)
    {
        throw new NotImplementedException();
    }

    public static object Resolve(this IContainer container, Type TFrom, string name = "")
    {
        throw new NotImplementedException();
    }

    public static TFrom Resolve<TFrom>(this IContainer container, string name = "")
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<TFrom> Resolves<TFrom>(this IContainer container, string name = "")
    {
        throw new NotImplementedException();
    }
}