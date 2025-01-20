#if NET
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Common;
using DryIocEx.Core.Event;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Network;

public interface IChannelOption
{
}

public class PipeChannelOption : IChannelOption
{
    // 1M by default
    public int MaxPackageLength { get; set; } = 1024 * 1024;

    // 4k by default
    public int ReceiveBufferSize { get; set; } = 1024 * 4;

    // 4k by default
    public int SendBufferSize { get; set; } = 1024 * 4;

    // trigger the read only when the stream is being consumed
    public bool ReadAsDemand { get; set; }

    /// <summary>
    ///     in milliseconds
    /// </summary>
    /// <value></value>
    public int ReceiveTimeout { get; set; }

    /// <summary>
    ///     in milliseconds
    /// </summary>
    /// <value></value>
    public int SendTimeout { get; set; }

    public bool NoDelay { set; get; } = true;
}

public class UdpChannelOption : PipeChannelOption
{
    public bool UseSendingPipe { set; get; } = true;
}

public enum StopReason
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

public class StopEventArgs : EventArgs
{
    public StopEventArgs(StopReason reason)
    {
        throw new NotImplementedException();
    }

    public StopReason Reason { get; }
}

public class EventStopped : PubSubEvent<object, StopEventArgs>
{
}

public interface IChannel
{
    INetworkContainer Container { get; }

    DateTime LastActiveTime { get; }

    /// <summary>
    ///     停止事件
    /// </summary>
    EventStopped Stopped { get; }

    StopReason? StopReason { get; }

    bool IsStopped { get; }

    void Start();

    ValueTask StopAsync(StopReason reason);

    ValueTask SendAsync(ReadOnlyMemory<byte> data);
}

public interface IChannel<TPackage> : IChannel
{
    IPackageEncoder<TPackage> Encoder { get; }
    IPackageFilter<TPackage> PackageFilter { get; }
    ValueTask<TPackage> ReceiveAsync();
    IAsyncEnumerable<TPackage> RunAsync();
    ValueTask SendAsync(TPackage package);
}

public abstract class PipeChannel<TPackage> : IChannel<TPackage>
{
    public PipeChannel(INetworkContainer container)
    {
        throw new NotImplementedException();
    }

    protected CancellationTokenSource PipeCTS { get; set; } = new();

    protected IChannelOption Option { get; }

    /// <summary>
    ///     会自动取消订阅
    /// </summary>
    public EventStopped Stopped { get; } = new();

    public INetworkContainer Container { get; }

    /// <summary>
    ///     发送和接收都会更新
    /// </summary>
    public DateTime LastActiveTime { get; protected set; }

    public StopReason? StopReason { get; protected set; }

    public IPackageEncoder<TPackage> Encoder { get; set; }
    public bool IsStopped { get; protected set; }

    public void Start()
    {
        throw new NotImplementedException();
    }

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        throw new NotImplementedException();
    }

    #region Receive

    public async IAsyncEnumerable<TPackage> RunAsync()
    {
        if (_receivesTask == null || _sendsTask == null) throw new Exception("This Channel has not been started yet.");

        while (true)
        {
            var package = await PackagePipe.ReadAsync().ConfigureAwait(false);
            if (package == null)
            {
                await HandleStopping(); //等待断开连接,不再接收数据
                yield break;
            }

            yield return package;
        }
    }

    public IPackageFilter<TPackage> PackageFilter { get; set; }

    protected Pipe Receive { get; } = new();
    protected IObjectPipe<TPackage> PackagePipe { get; } = new ObjectPipe<TPackage>();
    private Task _receivesTask;

    public async ValueTask<TPackage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }

    protected async Task ProcessReceives()
    {
        throw new NotImplementedException();
    }

    protected async void WaitReceiveSendEnd()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     处理接收
    ///     读管道，从管道中读数据
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <returns></returns>
    protected async Task ReadReceivePipeAsync(PipeReader reader)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     过滤数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="consumed"></param>
    /// <param name="examined"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected bool ResolveReceiveBuffer(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
        out SequencePosition examined)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task WriteReceivePipeAsync(PipeWriter writer)
    {
        throw new NotImplementedException();
    }

    protected abstract ValueTask<int> ReceiveIOAsync(Memory<byte> memory,
        CancellationToken cancellationtoken);

    protected void WriteEOFPackage()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Send

    private Task _sendsTask;

    protected Pipe Send { get; } = new();
    protected SemaphoreSlim SendLock { get; } = new(1, 1);

    protected async ValueTask<bool> ProcessSendRead(PipeReader reader)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     无法检测发送失败
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public virtual async ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task ProcessSends()
    {
        throw new NotImplementedException();
    }

    protected abstract ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token);


    public virtual async ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }

    protected void WriteBuffer(PipeWriter writer, ReadOnlyMemory<byte> buffer)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Stop

    protected virtual bool IsIgnorableException(Exception e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     等待收发结束后调用 Close和OnClose
    /// </summary>
    /// <returns></returns>
    private async ValueTask HandleStopping()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     修改状态触发事件
    /// </summary>
    protected void OnStopped()
    {
        throw new NotImplementedException();
    }

    protected virtual void OnError(string message, Exception exception = null)
    {
        throw new NotImplementedException();
    }

    protected abstract void Stop();


    public virtual async ValueTask StopAsync(StopReason reason)
    {
        throw new NotImplementedException();
    }

    #endregion
}

public class SocketPipeChannel<TPackage> : PipeChannel<TPackage>, IEndPoint
{
    private List<ArraySegment<byte>> segmentsForSend;
    protected Socket Socket;

    public SocketPipeChannel(INetworkContainer container, Socket socket, EndPoint endpoint) : base(container)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     此处Endpoint只是作为一个Key，当客户端的时候是Local，服务端的时候是Server
    /// </summary>
    public EndPoint EndPoint { get; }

    protected virtual void SetSocketOption(Socket socket)
    {
        throw new NotImplementedException();
    }


    protected override async ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken cancellationtoken)
    {
        throw new NotImplementedException();
    }

    protected virtual async ValueTask<int> ReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer,
        CancellationToken cancellationtoken)
    {
        throw new NotImplementedException();
    }

    protected override void Stop()
    {
        throw new NotImplementedException();
    }


    protected override bool IsIgnorableException(Exception e)
    {
        throw new NotImplementedException();
    }

    private bool IsIgnorableSocketException(SocketException se)
    {
        throw new NotImplementedException();
    }
}

public interface IWriteReceiveProxyChannel
{
    ValueTask<FlushResult> WriteReceiveDataAsync(Memory<byte> memory, CancellationToken cancellationtoken);
}

public abstract class WriteReceiveProxyChannel<TPackage> : PipeChannel<TPackage>, IWriteReceiveProxyChannel
{
    public WriteReceiveProxyChannel(INetworkContainer container) : base(container)
    {
    }

    public async ValueTask<FlushResult> WriteReceiveDataAsync(Memory<byte> memory, CancellationToken cancellationtoken)
    {
        throw new NotImplementedException();
    }

    protected override Task WriteReceivePipeAsync(PipeWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class UdpClientPipeChannel<TPackage> : SocketPipeChannel<TPackage>
{
    public UdpClientPipeChannel(INetworkContainer container, Socket socket, IPEndPoint endpoint) : base(container,
        socket, socket.LocalEndPoint)
    {
        throw new NotImplementedException();
    }

    private IPEndPoint RemoteEndPoint { get; }

    protected override void SetSocketOption(Socket socket)
    {
        throw new NotImplementedException();
    }

    private void MergeBuffer(ref ReadOnlySequence<byte> buffer, byte[] destBuffer)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public override ValueTask StopAsync(StopReason reason)
    {
        throw new NotImplementedException();
    }


    protected override void Stop()
    {
        throw new NotImplementedException();
    }


    protected override async ValueTask<int> ReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public interface IEndPoint
{
    EndPoint EndPoint { get; }
}

public class UdpServerPipeChannel<TPackage> : WriteReceiveProxyChannel<TPackage>, IEndPoint
{
    public UdpServerPipeChannel(INetworkContainer container, Socket socket, IPEndPoint remoteendpoint,
        string sessionkey) : base(container)
    {
        throw new NotImplementedException();
    }

    public Socket Socket { get; }
    public string SessionKey { get; }
    public bool UseSendingPipe => ((UdpChannelOption)Option).UseSendingPipe;

    public EndPoint EndPoint { get; }

    protected override ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken cancellationtoken)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private void MergeBuffer(ref ReadOnlySequence<byte> buffer, byte[] destBuffer)
    {
        throw new NotImplementedException();
    }

    protected override Task ProcessSends()
    {
        throw new NotImplementedException();
    }

    protected override void Stop()
    {
        throw new NotImplementedException();
    }

    public override async ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }

    public override async ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }
}

#endif