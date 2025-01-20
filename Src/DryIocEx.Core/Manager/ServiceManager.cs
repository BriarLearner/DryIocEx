using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IService
{
}

public interface IServiceManager
{
    TService GetService<TService>() where TService : IService;
    void RegisterService(Type type);
}

public class ServiceManager : IServiceManager
{
    private readonly Dictionary<Type, ServiceInfo> _ServiceDict = new();


    public TService GetService<TService>() where TService : IService
    {
        throw new NotImplementedException();
    }

    public void RegisterService(Type type)
    {
        throw new NotImplementedException();
    }


    private class ServiceInfo
    {
        private IService _instance;

        public ServiceInfo(Func<IService> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IService> Factory { get; }

        public IService Instance
        {
            get { throw new NotImplementedException(); }
        }
    }
}

public static class ServiceLocator
{
    private static IServiceManager _manager;

    public static IServiceManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetServiceManager(IServiceManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
}

public static class ServiceExtension
{
    public static TService GetService<TService>(this object obj) where TService : IService
    {
        throw new NotImplementedException();
    }

    public static void RegisterService(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterService(this IServiceManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}