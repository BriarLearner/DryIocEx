using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface ITransformer
{
}

public interface ITransformerManager
{
    TTransformer GetTransformer<TTransformer>() where TTransformer : ITransformer;
    void RegisterTransformer(Type type);
}

public class TransformerManager : ITransformerManager
{
    private readonly Dictionary<Type, TransformerInfo> _TransformerDict = new();


    public TTransformer GetTransformer<TTransformer>() where TTransformer : ITransformer
    {
        throw new NotImplementedException();
    }

    public void RegisterTransformer(Type type)
    {
        throw new NotImplementedException();
    }


    private class TransformerInfo
    {
        private ITransformer _instance;

        public TransformerInfo(Func<ITransformer> factory)
        {
            throw new NotImplementedException();
        }

        public Func<ITransformer> Factory { get; }

        public ITransformer Instance
        {
            get { throw new NotImplementedException(); }
        }
    }
}

public static class TransformerLocator
{
    private static ITransformerManager _manager;

    public static ITransformerManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetTransformerManager(ITransformerManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TransformerAttribute : Attribute
{
}

public static class TransformerExtension
{
    public static TTransformer GetTransformer<TTransformer>(this object obj) where TTransformer : ITransformer
    {
        throw new NotImplementedException();
    }

    public static void RegisterTransformer(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterTransformer(this ITransformerManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}