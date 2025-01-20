using System;
using DryIocEx.Core.Common;

namespace DryIocEx.Core.Log;

public class LogInfo : BasePrototype<LogInfo>
{
    /// <summary>
    ///     时间
    /// </summary>
    public DateTime Time { set; get; }

    /// <summary>
    ///     等级
    /// </summary>
    public EnumLogDegree Degree { set; get; }

    /// <summary>
    ///     关键字
    /// </summary>
    public string Keywords { set; get; }

    /// <summary>
    ///     文本
    /// </summary>
    public string Text { set; get; }

    /// <summary>
    ///     线程ID
    /// </summary>
    public int ThreadId { set; get; }


    protected override void Copy(LogInfo subject)
    {
        throw new NotImplementedException();
    }
}

public enum EnumLogDegree
{
    None = 1,
    Fatal = 2,
    Error = 3,
    Warn = 4,
    Info = 5,

    /// <summary>
    ///     All
    /// </summary>
    Debug = 6
}