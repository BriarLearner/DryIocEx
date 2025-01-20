using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Util;

/// <summary>
/// </summary>
[Obsolete("建议使用this.GetUtil<T>")]
public class UtilStore
{
    //方法类的修改是针对方法类方法中的实现，如果出现一个新的方法怎么升级修改，原来方法接口不变，使用适配器模式适配新的方法。
    //使用接口方式可能不太行，不能老修改接口吧
    //使用key来定位方法也不太好，不能一直记着方法的key吧

    //先使用继承的方式，通过外观来实现 xxxxUtil为外观最外层 继承的方式不能多重继承 目前还没有影响。


    private static readonly Lazy<GuidUtil> _guid = new(() => new GuidUtil());

    private static readonly Lazy<CommandLineUtil> _commandline = new(() => new CommandLineUtil());
    private static readonly Lazy<SingleUtil> _single = new(() => new SingleUtil());
    public static GuidUtil Guid => _guid.Value;
    public static CommandLineUtil CommandLine => _commandline.Value;
    public static SingleUtil Single => _single.Value;
}

public static class UtilLocator
{
    private static IUtilManager _manager;

    public static IUtilManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }


    public static void SetLogManager(IUtilManager manager)
    {
        throw new NotImplementedException();
    }

    private static IUtilManager LoadDefaultUtil(this IUtilManager manager)
    {
        throw new NotImplementedException();
    }
}

public interface IUtilManager
{
    TUtil GetUtil<TUtil>() where TUtil : IUtil;
    void RegisterUtil(Type type);
}

[AutoRegister(typeof(IUtilManager), EnumLifetime.Singleton)]
public class UtilManager : IUtilManager
{
    private readonly Dictionary<Type, UtilInfo> _utilstore = new();

    public TUtil GetUtil<TUtil>() where TUtil : IUtil
    {
        throw new NotImplementedException();
    }

    public void RegisterUtil(Type type)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     所有Util必须实现自己的接口,并继承该接口
/// </summary>
public interface IUtil
{
}

internal class UtilInfo
{
    private IUtil instance;

    public UtilInfo(Func<IUtil> factory)
    {
        throw new NotImplementedException();
    }

    public Func<IUtil> Factory { set; get; }

    public IUtil Instance
    {
        get { throw new NotImplementedException(); }
    }
}

public static class UtilExtension
{
    public static TUtil GetUtil<TUtil>(this object obj) where TUtil : IUtil
    {
        throw new NotImplementedException();
    }

    public static void RegisterUtil(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterUtil(this IUtilManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class UtilAttribute : Attribute
{
}