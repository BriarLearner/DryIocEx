using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Common;

public interface IHandle
{
    /// <summary>
    ///     添加Handle
    /// </summary>
    /// <param name="pfc"></param>
    /// <param name="handle"></param>
    void AddHandle(int pfc, Func<Operate, Operate> handle);

    /// <summary>
    ///     添加handle
    /// </summary>
    /// <param name="pfc"></param>
    /// <param name="type">必须实现IOperateHandle</param>
    void AddHandle(int pfc, Type type);

    /// <summary>
    ///     执行某个操作
    /// </summary>
    /// <param name="operate">操作封装</param>
    /// <returns></returns>
    Operate Execute(Operate operate);

    /// <summary>
    ///     异步执行某个操作
    /// </summary>
    /// <param name="operate">操作封装</param>
    /// <returns></returns>
    Task<Operate> ExecuteAsync(Operate operate);
}

public interface IManager<TSubject> : IHandle
{
    Dictionary<string, TSubject> Subjects { get; }

    /// <summary>
    ///     注入被管理的组件
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="name"></param>
    void Register(TSubject subject, string name);

    /// <summary>
    ///     移除被管理的组件
    /// </summary>
    /// <param name="name"></param>
    void UnRegister(string name);
}

/// <summary>
///     管理器基类
///     管理器实现动态加载所有职责，执行使用Operate模式进行
/// </summary>
/// <typeparam name="TSubject"></typeparam>
public class BaseManager<TSubject> : IManager<TSubject>
{
    protected Dictionary<int, OperateHandleReference> _actions = new();

    public Dictionary<string, TSubject> Subjects { get; } = new();

    public void Register(TSubject subject, string name)
    {
        throw new NotImplementedException();
    }

    public void UnRegister(string name)
    {
        throw new NotImplementedException();
    }

    public Operate Execute(Operate operate)
    {
        throw new NotImplementedException();
    }

    public Task<Operate> ExecuteAsync(Operate operate)
    {
        throw new NotImplementedException();
    }

    public void AddHandle(int pfc, Func<Operate, Operate> handle)
    {
        throw new NotImplementedException();
    }

    public void AddHandle(int pfc, Type handletype)
    {
        throw new NotImplementedException();
    }
}

public static class HandleExtension
{
    public static void Register(Assembly assembly)
    {
        throw new NotImplementedException();
    }
}