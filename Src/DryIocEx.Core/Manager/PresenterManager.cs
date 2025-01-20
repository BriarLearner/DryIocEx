using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IPresenter
{
}

public interface IPresenterManager
{
    TPresenter GetPresenter<TPresenter>() where TPresenter : IPresenter;
    void RegisterPresenter(Type type);
}

public class PresenterManager : IPresenterManager
{
    private readonly Dictionary<Type, PresenterInfo> _presenterDict = new();


    public TPresenter GetPresenter<TPresenter>() where TPresenter : IPresenter
    {
        throw new NotImplementedException();
    }

    public void RegisterPresenter(Type type)
    {
        throw new NotImplementedException();
    }


    private class PresenterInfo
    {
        private IPresenter _instance;

        public PresenterInfo(Func<IPresenter> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IPresenter> Factory { get; }

        public IPresenter Instance
        {
            get { throw new NotImplementedException(); }
        }
    }
}

public static class PresenterLocator
{
    private static IPresenterManager _manager;

    public static IPresenterManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetPresenterManager(IPresenterManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PresenterAttribute : Attribute
{
}

public static class PresenterExtension
{
    public static TPresenter GetPresenter<TPresenter>(this object obj) where TPresenter : IPresenter
    {
        throw new NotImplementedException();
    }

    public static void RegisterPresenter(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterPresenter(this IPresenterManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}