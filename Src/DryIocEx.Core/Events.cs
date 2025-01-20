using System;
using DryIocEx.Core.Event;

/* 项目“DryIocEx.CorePro (netstandard2.1)”的未合并的更改
在此之前:
using DryIocEx.Core.Event;
在此之后:
using DryIocEx;
using DryIocEx.Core;
using DryIocEx.Core;
using DryIocEx.Core.Event;
*/

namespace DryIocEx.Core;

public enum CloseReason
{
    /// <summary>
    ///     未知原因
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     服务器关闭
    /// </summary>
    ServerShutdown = 1,

    /// <summary>
    ///     远程端关闭
    /// </summary>
    RemoteClosing = 2,

    /// <summary>
    ///     本地端关闭
    /// </summary>
    LocalClosing = 3,

    /// <summary>
    ///     应用错误
    /// </summary>
    ApplicationError = 4,

    /// <summary>
    ///     Socket错误关闭
    /// </summary>
    SocketError = 5,

    /// <summary>
    ///     Socket超时
    /// </summary>
    TimeOut = 6,

    /// <summary>
    ///     协议错误
    /// </summary>
    ProtocolError = 7,

    /// <summary>
    ///     内部错误
    /// </summary>
    InternalError = 8
}

public class CloseEventArgs : EventArgs
{
    public CloseEventArgs(CloseReason reason)
    {
        throw new NotImplementedException();
    }

    public CloseReason Reason { get; }
}

public class EventClosed : PubSubEvent<object, CloseEventArgs>
{
}