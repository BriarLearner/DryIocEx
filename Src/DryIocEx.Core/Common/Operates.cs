using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Common;

public class OperateHandleReference
{
    public OperateHandleReference(Type handlertype)
    {
        throw new NotImplementedException();
    }

    public OperateHandleReference(Func<Operate, Operate> handle)
    {
        throw new NotImplementedException();
    }

    public Func<Operate, Operate> Handle { get; }

    public Type HandlerType { get; }


    public Func<Operate, Operate> GetHandle()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     所有需要自动注入的OperateHandle
///     需要实现AutoRegister的单例
///     Owner必须为控制器的接口类
/// </summary>
public interface IOperateHandle
{
    Operate Handle(Operate operate);
}

public struct OperateParameter
{
    private readonly Operate _operate;

    public OperateParameter(Operate operate)
    {
        throw new NotImplementedException();
    }

    public int PFC => _operate.GetData<int>(SGConStr.PFC);

    public T GetParameter<T>(string key)
    {
        throw new NotImplementedException();
    }


    public bool TryGetParameter<T>(string key, out T parameter)
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveParameter(string key)
    {
        throw new NotImplementedException();
    }

    public OperateParameter AddParameter<T>(string key, T value)
    {
        throw new NotImplementedException();
    }
}

public class OperatePool : Pool<Operate>
{
    public override void Return(Operate sub)
    {
        throw new NotImplementedException();
    }

    public override Operate Rent()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     Operate核心
///     参数和结果职责合并
///     因为要适配所有的接口，内部使用两个字典存放所有参数，使用Key-Value模式
///     唯一确定的值是PFC，在构造和初始化的时候加入默认为0
///     构造委托给OperateBuilder
///     结果构造委托给OperateResultBuilder
///     访问结果委托给OperateResult
///     参数访问委托给OperateParameter
///     委托方式可以变化，这边是写死，后期可以使用静态方法，动态配置的方式应对变化，目前还没有变化。
///     只有dispose进行回收，没有其他接口，需要手动调用，或使用Using
/// </summary>
public class Operate : IDisposable
{
    //修改1:这边用不了弱引用了，因为值类型弱引用在用的时候被回收了（通过测试），因为由Dispose的缘故,不会内存泄漏
    //修改2:检测一下值类型，如果是值类型直接引用，如果是引用类型就弱引用

    private readonly Dictionary<string, object> _data = new(10);


    private readonly Dictionary<string, object> _result = new(10);

    private int disflag;


    public OperateResultBuildPro NewResult => new(this);

    public OperateResult Result => new(this);
    public OperateParameter Parameter => new(this);
    public bool Disposable { get; private set; }

    public static OperateBuilderPro New => new();
    public static Operate Null { get; } = new();
    internal static IPool<Operate> Pool { get; } = new OperatePool();

    /// <summary>
    ///     内部回收机制
    ///     不调用Dispose不会复用，由GC回收
    /// </summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool AddData(string str, object obj)
    {
        throw new NotImplementedException();
    }

    public T GetData<T>(string str)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     不包含key返回false
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryGetData<T>(string str, out T result)
    {
        throw new NotImplementedException();
    }


    public bool TryRemoveData(string str)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     值类型转换，string和各种值类型强转
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    private T ConvertValue<T>(object obj)
    {
        throw new NotImplementedException();
    }


    public bool AddResult(string str, object obj)
    {
        throw new NotImplementedException();
    }

    public T GetResult<T>(string str)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     不包含key返回false
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryGetResult<T>(string str, out T result)
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveResult(string str)
    {
        throw new NotImplementedException();
    }

    public void Initial()
    {
        throw new NotImplementedException();
    }
}

public struct OperateResult
{
    private readonly Operate _operate;

    public OperateResult(Operate operate)
    {
        throw new NotImplementedException();
    }

    public int PFC => _operate.GetData<int>(SGConStr.PFC);

    public bool Success => _operate.GetResult<bool>(SGConStr.Success);

    public static implicit operator bool(OperateResult result)
    {
        throw new NotImplementedException();
    }

    public T GetResult<T>(string key)
    {
        throw new NotImplementedException();
    }

    public bool TryGetResult<T>(string key, out T result)
    {
        throw new NotImplementedException();
    }

    public bool TryRemoveResult(string key)
    {
        throw new NotImplementedException();
    }

    public void Return()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// </summary>
public struct OperateResultBuildPro
{
    //在OperateResultBuild上进行改进，将class改成struct，消耗更小
    //所有的扩展通过扩展方法来进行，不使用继承的方式，对原来的入侵更小，但是代价是所有的方法的业务方法都编程静态方法暴露


    public OperateResultBuildPro(Operate operate)
    {
        throw new NotImplementedException();
    }

    private readonly Operate _operate;
    private readonly List<Func<Operate, Operate>> _list = new();

    public OperateResultBuildPro SetResult(bool success)
    {
        throw new NotImplementedException();
    }

    public OperateResultBuildPro AddResult<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public Operate Build()
    {
        throw new NotImplementedException();
    }

    public OperateResultBuildPro Do(Action<Operate> action)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     普通使用OperateResultBuildPro 结构体模式，消耗更小
/// </summary>
public class OperateResultBuild : BaseBuilder<OperateResultBuild, Operate>
{
    public OperateResultBuild(Operate operate)
    {
        throw new NotImplementedException();
    }

    protected Operate Operate { get; }

    public OperateResultBuild SetResult(bool success)
    {
        throw new NotImplementedException();
    }

    public OperateResultBuild AddResult<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public override Operate Build()
    {
        throw new NotImplementedException();
    }
}

public struct OperateBuilderPro
{
    public OperateBuilderPro AddParameter<T>(string key, T param)
    {
        throw new NotImplementedException();
    }

    public OperateBuilderPro()
    {
    }

    public OperateBuilderPro SetPFC(int pfc)
    {
        throw new NotImplementedException();
    }

    public OperateBuilderPro Do(Action<Operate> action)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     AutoDispose和Dispose不能重复调用，没有同步机制
    ///     优先手动Dispose，使用AutoDispose就不要手动使用Dispose
    /// </summary>
    /// <param name="millisecond"></param>
    /// <returns></returns>
    public OperateBuilderPro AutoDispose(int millisecond)
    {
        throw new NotImplementedException();
    }

    public Operate Build()
    {
        throw new NotImplementedException();
    }

    private readonly List<Func<Operate, Operate>> _list = new();
}

public class OperateBuilder : BaseBuilder<OperateBuilder, Operate>
{
    public OperateBuilder AddParameter<T>(string key, T param)
    {
        throw new NotImplementedException();
    }

    public OperateBuilder SetPFC(int pfc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     AutoDispose和Dispose不能重复调用，没有同步机制
    ///     优先手动Dispose，使用AutoDispose就不要手动使用Dispose
    /// </summary>
    /// <param name="millisecond"></param>
    /// <returns></returns>
    public OperateBuilder AutoDispose(int millisecond)
    {
        throw new NotImplementedException();
    }


    public override Operate Build()
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class OperateAttribute : Attribute
{
    public OperateAttribute(int pfc, Type owner, string name = "")
    {
        throw new NotImplementedException();
    }

    public int PFC { set; get; }
    public Type Owner { set; get; }

    public string Name { set; get; }
}