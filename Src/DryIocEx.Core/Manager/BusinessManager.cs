using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IBusiness
{
}

public interface IBusinessManager
{
    TBusiness GetBusiness<TBusiness>() where TBusiness : IBusiness;
    void RegisterBusiness(Type type);
}

public class BusinessManager : IBusinessManager
{
    private readonly Dictionary<Type, BusinessInfo> _BusinessDict = new();


    public TBusiness GetBusiness<TBusiness>() where TBusiness : IBusiness
    {
        throw new NotImplementedException();
    }

    public void RegisterBusiness(Type type)
    {
        throw new NotImplementedException();
    }


    private class BusinessInfo
    {
        private IBusiness _instance;

        public BusinessInfo(Func<IBusiness> factory)
        {
            Factory = factory;
        }

        public Func<IBusiness> Factory { get; }

        public IBusiness Instance
        {
            get { return _instance ??= Factory.Invoke(); }
        }
    }
}

public static class BusinessLocator
{
    private static IBusinessManager _manager;

    public static IBusinessManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetBusinessManager(IBusinessManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BusinessAttribute : Attribute
{
}

public static class BusinessExtension
{
    public static TBusiness GetBusiness<TBusiness>(this object obj) where TBusiness : IBusiness
    {
        throw new NotImplementedException();
    }

    public static void RegisterBusiness(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterBusiness(this IBusinessManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}