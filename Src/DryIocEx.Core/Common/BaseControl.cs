using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Common;

/// <summary>
///     控制器的功能比Manager更弱一点，实际上就是操作的组合，不管理实体
/// </summary>
public interface IControl : IHandle
{
}

public abstract class BaseControl : IControl
{
    protected Dictionary<int, OperateHandleReference> _actions = new();

    public virtual Operate Execute(Operate operate)
    {
        throw new NotImplementedException();
    }

    public virtual Task<Operate> ExecuteAsync(Operate operate)
    {
        throw new NotImplementedException();
    }

    public virtual void AddHandle(int pfc, Func<Operate, Operate> handle)
    {
        throw new NotImplementedException();
    }

    public virtual void AddHandle(int pfc, Type handletype)
    {
        throw new NotImplementedException();
    }
}