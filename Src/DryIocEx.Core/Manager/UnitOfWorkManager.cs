using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IUnitOfWork
{
}

public interface IUnitOfWorkManager
{
    TUnitOfWork GetUnitOfWork<TUnitOfWork>() where TUnitOfWork : IUnitOfWork;
    void RegisterUnitOfWork(Type type);
}

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly Dictionary<Type, UnitOfWorkInfo> _UnitOfWorkDict = new();


    public TUnitOfWork GetUnitOfWork<TUnitOfWork>() where TUnitOfWork : IUnitOfWork
    {
        throw new NotImplementedException();
    }

    public void RegisterUnitOfWork(Type type)
    {
        throw new NotImplementedException();
    }


    private class UnitOfWorkInfo
    {
        private IUnitOfWork _instance;

        public UnitOfWorkInfo(Func<IUnitOfWork> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IUnitOfWork> Factory { get; }

        public IUnitOfWork Instance
        {
            get { throw new NotImplementedException(); }
        }
    }
}

public static class UnitOfWorkLocator
{
    private static IUnitOfWorkManager _manager;

    public static IUnitOfWorkManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetUnitOfWorkManager(IUnitOfWorkManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class UnitOfWorkAttribute : Attribute
{
}

public static class UnitOfWorkExtension
{
    public static TUnitOfWork GetUnitOfWork<TUnitOfWork>(this object obj) where TUnitOfWork : IUnitOfWork
    {
        throw new NotImplementedException();
    }

    public static void RegisterUnitOfWork(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterUnitOfWork(this IUnitOfWorkManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}