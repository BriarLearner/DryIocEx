﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.NetworkPro;

internal interface IPipeHost
{
    DateTime LastActiveTime { set; get; }

    StopReason StopReason { set; get; }

    CancellationTokenSource StopTokenSource { get; }
    void OnError(string message, Exception exception = null);

    bool IsIgnorableException(Exception e);
}

internal delegate bool HandleReceive(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
    out SequencePosition examined);

internal delegate ValueTask<int> FillDataAsync(Memory<byte> buffer, CancellationToken token);

internal class ReceivePipe
{
    private readonly IPipeHost _host;
    private int _maxSize;
    private readonly PipeReader _reader;
    private readonly int _receiveSize;
    internal PipeWriter _writer;

    public ReceivePipe(IPipeHost host, int receivesize, int maxsize)
    {
        throw new NotImplementedException();
    }

    public async Task ReadAsync(CancellationToken token, HandleReceive handlereceive)
    {
        throw new NotImplementedException();
    }

    public async Task WriteAsync(CancellationToken token, FillDataAsync filldataasync)
    {
        throw new NotImplementedException();
    }
}

internal delegate ValueTask<int> SendAsync(ReadOnlySequence<byte> buffer, CancellationToken token);

internal class SendPipe
{
    private readonly IPipeHost _host;
    private readonly PipeReader _reader;
    private readonly SemaphoreSlim _sendlock = new(1, 1);

    public SendPipe(IPipeHost host)
    {
        throw new NotImplementedException();
    }

    public PipeWriter Writer { get; }

    public async ValueTask ReadAsync(CancellationToken token, SendAsync sendasync)
    {
        throw new NotImplementedException();
    }


    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }



    public ValueTask WriteCompletedAsync()
    {
        throw new NotImplementedException();
    }
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

public interface IChannelOption : IOption
{
}

public class ChannelOption : IChannelOption
{
    // 1M by default
    public int MaxPackageLength { get; set; } = 1024 * 1024;

    // 4k by default
    public int ReceiveBufferSize { get; set; } = 1024 * 4;

    // 4k by default
    public int SendBufferSize { get; set; } = 1024 * 4;

    /// <summary>
    ///     Nagle算法是一种用于减少小数据包发送的网络优化算法。它通过将小数据包缓冲在本地，组合成更大的数据包进行发送，从而减少网络传输中的开销。这种方式可以提高网络的利用率，特别是在需要频繁发送小数据包的场景下
    /// </summary>
    public bool NoDelay { set; get; } = true;
}

public interface IChannel<TPackage>
{
    public delegate ValueTask ChannelStopHandle<TPackage>(IChannel<TPackage> channel, StopReason reason);

    IContainer Container { get; }

    bool IsStop { get; }

    StopReason StopReason { get; }

    DateTime StartTime { get; }

    DateTime LastActiveTime { get; }
    public event ChannelStopHandle<TPackage> ChannelStop;

    void Start();

    void Stop(StopReason reason = StopReason.LocalClosing);

    ValueTask SendAsync(ReadOnlyMemory<byte> data);

    ValueTask SendAsync(TPackage package);

    IAsyncEnumerable<TPackage> RunAsync();

    ValueTask<TPackage> ReceiveAsync();
}

public abstract class Channel<TPackage> : IChannel<TPackage>, IPipeHost
{
    private readonly System.Threading.Channels.Channel<TPackage> _channel;
    private readonly ILogManager _logManager;

    private readonly IPackageFilter<TPackage> _packageFilter;

    internal ReceivePipe _receivePipe;

    private Task _receivetask;
    private readonly SendPipe _sendPipe;
    private Task _sendtask;

    protected CancellationTokenSource _stopTokenSource = new();

    public Channel(IContainer container)
    {
        throw new NotImplementedException();
    }

    private ChannelOption _option { get; }
    public IContainer Container { get; }
    public bool IsStop { get; private set; }
    public StopReason StopReason { get; set; }
    public DateTime StartTime { get; private set; }
    public DateTime LastActiveTime { get; set; }

    public event IChannel<TPackage>.ChannelStopHandle<TPackage> ChannelStop;

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop(StopReason reason = StopReason.LocalClosing)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync(TPackage package)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<TPackage> RunAsync()
    {
        while (true)
        {
            if (IsStop) yield break;
            TPackage package = default;
            try
            {
                package = await _channel.Reader.ReadAsync().ConfigureAwait(false);
                if (package == null) throw new ArgumentException("null package");
            }
            catch (Exception e)
            {
                if (!(e is ChannelClosedException)) OnError("channel ReadAsync", e);
                _stopTokenSource.Cancel();
                yield break;
            }

            yield return package;
        }
    }

    public async ValueTask<TPackage> ReceiveAsync()
    {
        throw new NotImplementedException();
    }

    public CancellationTokenSource StopTokenSource => _stopTokenSource;

    public void OnError(string message, Exception exception = null)
    {
        throw new NotImplementedException();
    }

    public bool IsIgnorableException(Exception e)
    {
        throw new NotImplementedException();
    }

    public async ValueTask HandleStopping()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     发布Stop
    /// </summary>
    public async void OnStopped()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     子类Stop
    /// </summary>
    protected abstract void OnInnerStop();

    /// <summary>
    ///     抽象出来 因为UdpServer从Listen的socket中接收数据放到ReceivePipe中
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual Task HandleReceiveWritePipe(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private async Task ProcessReceiveAsync()
    {
        throw new NotImplementedException();
    }

    protected abstract ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token);

    protected abstract ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token);

    public void WriteEOFPackage()
    {
        throw new NotImplementedException();
    }

    private async Task ProcessSendAsync()
    {
        throw new NotImplementedException();
    }

    private bool ResolveBuffer(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
        out SequencePosition examined)
    {
        throw new NotImplementedException();
    }
}

public class TcpChannel<TPackage> : Channel<TPackage>
{
    private Socket _socket;

    private List<ArraySegment<byte>> segmentsForSend;

    public TcpChannel(IContainer container, Socket socket) : base(container)
    {
        throw new NotImplementedException();
    }

    protected override void OnInnerStop()
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

public class UdpServerChannel<TPackage> : Channel<TPackage>
{
    private readonly IPEndPoint _remoteEndPoint;
    private Socket _socket;

    public UdpServerChannel(IContainer container, Socket socket, IPEndPoint ipendpoint) : base(container)
    {
        throw new NotImplementedException();
    }

    protected override void OnInnerStop()
    {
        throw new NotImplementedException();
    }

    private void MergeBuffer(ref ReadOnlySequence<byte> buffer, byte[] destBuffer)
    {
        throw new NotImplementedException();
    }

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<FlushResult> WriteReceiveDataAsync(Memory<byte> memory, CancellationToken cancellationtoken)
    {
        throw new NotImplementedException();
    }


    protected override Task HandleReceiveWritePipe(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

public class UdpClientChannel<TPackage> : Channel<TPackage>
{
    private readonly EndPoint _remoteEndPoint;

    private Socket _socket;

    public UdpClientChannel(IContainer container, Socket socket, EndPoint remoteendpoint) : base(container)
    {
        throw new NotImplementedException();
    }

    protected override void OnInnerStop()
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

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}