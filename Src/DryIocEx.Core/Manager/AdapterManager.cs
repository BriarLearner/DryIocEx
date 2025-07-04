using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IAdapter
{
}

public interface IAdapterManager
{
    TAdapter GetAdapter<TAdapter>() where TAdapter : IAdapter;
    void RegisterAdapter(Type type);
}
/// <summary>
/// 适配器管理器
/// </summary>
public class AdapterManager : IAdapterManager
{
    private readonly Dictionary<Type, AdapterInfo> _adapterDict = new();


    public TAdapter GetAdapter<TAdapter>() where TAdapter : IAdapter
    {
        throw new NotImplementedException();
    }

    public void RegisterAdapter(Type type)
    {
        throw new NotImplementedException();
    }


    private class AdapterInfo
    {
        private IAdapter _instance;

        public AdapterInfo(Func<IAdapter> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IAdapter> Factory { get; }

        public IAdapter Instance
        {
            get { return _instance ??= Factory.Invoke(); }
        }
    }
}

public static class AdapterLocator
{
    private static IAdapterManager _manager;

    public static IAdapterManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetAdapterManager(IAdapterManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AdapterAttribute : Attribute
{
}

public static class AdapterExtension
{
    public static TAdapter GetAdapter<TAdapter>(this object obj) where TAdapter : IAdapter
    {
        throw new NotImplementedException();
    }

    public static void RegisterAdapter(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterAdapter(this IAdapterManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}