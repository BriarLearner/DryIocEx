using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IOperator
{
}

public interface IOperatorManager
{
    TOperator GetOperator<TOperator>() where TOperator : IOperator;
    void RegisterOperator(Type type);
}

public class OperatorManager : IOperatorManager
{
    private readonly Dictionary<Type, OperatorInfo> _OperatorDict = new();


    public TOperator GetOperator<TOperator>() where TOperator : IOperator
    {
        throw new NotImplementedException();
    }

    public void RegisterOperator(Type type)
    {
        throw new NotImplementedException();
    }


    private class OperatorInfo
    {
        private IOperator _instance;

        public OperatorInfo(Func<IOperator> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IOperator> Factory { get; }

        public IOperator Instance
        {
            get => throw new NotImplementedException();
        }
    }
}

public static class OperatorLocator
{
    private static IOperatorManager _manager;

    public static IOperatorManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetOperatorManager(IOperatorManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OperatorAttribute : Attribute
{
}

public static class OperatorExtension
{
    public static TOperator GetOperator<TOperator>(this object obj) where TOperator : IOperator
    {
        throw new NotImplementedException();
    }

    public static void RegisterOperator(this IContainer container, Assembly assembly)
    {
        throw new NotImplementedException();
    }

    public static void RegisterOperator(this IOperatorManager store, Assembly assembly)
    {
        throw new NotImplementedException();
    }
}