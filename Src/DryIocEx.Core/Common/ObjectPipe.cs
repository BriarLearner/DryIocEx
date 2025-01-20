#if NET


using System;
using System.Buffers;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace DryIocEx.Core.Common;

public interface IObjectPipe<T>
{
    int Write(T target);
    ValueTask<T> ReadAsync();
}

public class ObjectPipe<T> : IObjectPipe<T>, IValueTaskSource<T>, IDisposable
{
    /// <summary>
    ///     每一小段内存的初始大小
    /// </summary>
    private const int _segmentSize = 5;

    /// <summary>
    ///     锁对象
    /// </summary>
    private readonly object _syncRoot = new();

    /// <summary>
    ///     当前指向的内存段
    ///     写链表指针
    /// </summary>
    private BufferSegment _current;

    /// <summary>
    ///     链表头
    ///     读链表指针
    /// </summary>
    private BufferSegment _first;

    /// <summary>
    ///     上一次是等待读标志
    /// </summary>
    private bool _lastReadIsWait;

    /// <summary>
    ///     数据总长度
    /// </summary>
    private int _length;

    /// <summary>
    ///     提供用于实现手动重置 IValueTaskSource 或 IValueTaskSource
    ///     <TResult>
    ///         的核心逻辑。
    ///         设置读异步
    /// </summary>
    private ManualResetValueTaskSourceCore<T> _manualResetValueTaskSource;

    /// <summary>
    ///     等待读标志
    /// </summary>
    private bool _waiting;


    public ObjectPipe()
    {
        throw new NotImplementedException();
    }


    #region Write

    /// <summary>
    ///     写数据
    ///     写入Null返回也是Null
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public int Write(T target)
    {
        throw new NotImplementedException();
    }

    #endregion

    private BufferSegment CreateSegment()
    {
        throw new NotImplementedException();
    }

    private void SetBufferSegment(BufferSegment segment)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     连续的一块内存
    ///     链表组合小内存块变成大内存块
    ///     每块内存 组成链表， 每块内存有两个属性，一个是偏移量，取值的指针，还有一个是结束，存值的指针
    /// </summary>
    private class BufferSegment
    {
        public BufferSegment(T[] array)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     连续的一小块内存
        ///     数据
        /// </summary>
        public T[] Array { get; }

        /// <summary>
        ///     下一个内存块
        ///     链表指针
        /// </summary>
        public BufferSegment Next { get; set; }

        /// <summary>
        ///     偏移量 从0开始
        ///     读数据指针
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        ///     -1为空
        ///     写数据指针
        /// </summary>
        public int End { get; set; } = -1; // -1 means no item in this segment

        /// <summary>
        ///     小内存块是否可写入
        /// </summary>
        public bool IsAvailable => Array.Length > End + 1;

        /// <summary>
        ///     写入数据
        /// </summary>
        /// <param name="value"></param>
        public void Write(T value)
        {
            Array[++End] = value;
        }
    }

    #region Read

    public ValueTask<T> ReadAsync()
    {
        throw new NotImplementedException();
    }

    private bool TryRead(out T value)
    {
        throw new NotImplementedException();
    }

    protected virtual void OnWaitTaskStart()
    {
    }

    #endregion


    #region IValueTaskSource

    T IValueTaskSource<T>.GetResult(short token)
    {
        throw new NotImplementedException();
    }

    ValueTaskSourceStatus IValueTaskSource<T>.GetStatus(short token)
    {
        throw new NotImplementedException();
    }

    void IValueTaskSource<T>.OnCompleted(Action<object?> continuation, object? state, short token,
        ValueTaskSourceOnCompletedFlags flags)
    {
        throw new NotImplementedException();
    }

    #endregion


    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    #endregion
}
#endif