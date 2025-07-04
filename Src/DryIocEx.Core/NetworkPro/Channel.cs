using System;
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
        _host = host;
        var pipe = new Pipe();
        _writer = pipe.Writer;
        _reader = pipe.Reader;
        _receiveSize = receivesize < 0 ? 1024 * 4 : receivesize;
        _maxSize = maxsize < 0 ? 1024 * 1024 : maxsize;
    }

    public async Task ReadAsync(CancellationToken token, HandleReceive handlereceive)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                ReadResult result;
                try
                {
                    result = await _reader.ReadAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException e)
                {
                    break;
                }
                catch (Exception e)
                {
                    if (!_host.IsIgnorableException(e))
                    {
                        _host.OnError("Read ReceivePipe Error", e);
                        break;
                    }

                    continue;
                }

                var buffer = result.Buffer;
                var consumed = buffer.Start;
                var examined = buffer.End;
                if (result.IsCanceled)
                    break;
                var completed = result.IsCompleted;
                try
                {
                    if (buffer.Length > 0)
                        if (!handlereceive(ref buffer, out consumed, out examined))
                        {
                            completed = true;
                            break;
                        }

                    if (completed) break;
                }
                catch (Exception e)
                {
                    _host.OnError("ReadAsyncError", e);
                    if (_host.StopReason == StopReason.Unknown)
                        _host.StopReason = StopReason.ProtocolError;
                    _host.StopTokenSource.Cancel();
                    break;
                }
                finally
                {
                    _reader.AdvanceTo(consumed, examined);
                }
            }

            await _reader.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _host.OnError("Read ReceivePipe Error", e);
        }
    }

    public async Task WriteAsync(CancellationToken token, FillDataAsync filldataasync)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var memory = _writer.GetMemory(_receiveSize);
                    //这边有问题，当主动Cancel的时候 这边不会立刻停止
                    var bytesread = await filldataasync(memory, token).ConfigureAwait(false);
                    if (bytesread == 0)
                    {
                        //远程关闭
                        if (_host.StopReason == StopReason.Unknown)
                            _host.StopReason = StopReason.RemoteClosing;
                        break;
                    }

                    _host.LastActiveTime = DateTime.Now;
                    _writer.Advance(bytesread);
                }
                catch (OperationCanceledException e)
                {
                    if (_host.StopReason == StopReason.Unknown)
                        _host.StopReason = StopReason.LocalClosing;
                    break;
                }
                catch (Exception e)
                {
                    if (_host.IsIgnorableException(e))
                    {
                        if (_host.StopReason == StopReason.Unknown)
                            _host.StopReason = StopReason.RemoteClosing;
                    }
                    else
                    {
                        _host.OnError("WriteAsyncError", e);
                        if (_host.StopReason == StopReason.Unknown)
                            _host.StopReason = StopReason.SocketError;
                    }

                    break;
                }

                var result = await _writer.FlushAsync(token).ConfigureAwait(false);
                if (result.IsCanceled) break;
            }

            await _writer.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _host.OnError("WriteAsyncError", e);
        }
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
        _host = host;
        var pipe = new Pipe();
        _reader = pipe.Reader;
        Writer = pipe.Writer;
    }

    public PipeWriter Writer { get; }

    public async ValueTask ReadAsync(CancellationToken token, SendAsync sendasync)
    {
        while (!token.IsCancellationRequested)
        {
            var result = await _reader.ReadAsync(token).ConfigureAwait(false);
            var completed = result.IsCompleted;
            var buffer = result.Buffer;
            var end = buffer.End;
            if (!buffer.IsEmpty)
                try
                {
                    await sendasync(buffer, token);
                    _host.LastActiveTime = DateTime.Now;
                }
                catch (Exception e)
                {
                    _host.StopTokenSource.Cancel();
                    if (!_host.IsIgnorableException(e)) _host.OnError("SendAsync Error", e);
                    break;
                }

            _reader.AdvanceTo(end);
            if (completed) break;
        }

        await _reader.CompleteAsync().ConfigureAwait(false);
    }


    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data)
    {
        try
        {
            await _sendlock.WaitAsync();
            Writer.Write(data.Span);
            await Writer.FlushAsync();
        }
        finally
        {
            _sendlock.Release();
        }
    }



    public ValueTask WriteCompletedAsync()
    {
        return Writer.CompleteAsync();
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
        Container = container;
        _option = container.Resolve<IChannelOption>().As<ChannelOption>();
        _packageFilter = container.Resolve<IPackageFilter<TPackage>>();
        _channel = Channel.CreateUnbounded<TPackage>();
        _receivePipe = new ReceivePipe(this, _option.ReceiveBufferSize, _option.MaxPackageLength);
        _sendPipe = new SendPipe(this);
        _logManager = Container.Resolve<ILogManager>();
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
        StartTime = DateTime.Now;
        _receivetask = ProcessReceiveAsync();
        _sendtask = ProcessSendAsync();
        HandleStopping().ConfigureAwait(false);
    }

    public void Stop(StopReason reason = StopReason.LocalClosing)
    {
        if (IsStop) return;
        StopReason = reason;
        _stopTokenSource.Cancel();
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> data)
    {
        if (IsStop)
#if NET
            return ValueTask.CompletedTask;
#else
                return new ValueTask();
#endif
        return _sendPipe.WriteAsync(data);
    }

    public ValueTask SendAsync(TPackage package)
    {
        if (IsStop)
#if NET
            return ValueTask.CompletedTask;
#else
                return new ValueTask();
#endif
        var data = _packageFilter.Converter(package);
        if (data.IsEmpty) return new ValueTask();
        return _sendPipe.WriteAsync(data);
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
        if (IsStop) return default;
        var package = await _channel.Reader.ReadAsync().ConfigureAwait(false);
        if (package == null)
        {
            _stopTokenSource.Cancel();
            return default;
        }

        return package;
    }

    public CancellationTokenSource StopTokenSource => _stopTokenSource;

    public void OnError(string message, Exception exception = null)
    {
        if (exception != null)
            _logManager.BroadcastLog(exception.ToLogInfo(message));
        else
            _logManager.BroadcastLog(message.ToLogInfo(EnumLogDegree.Info, "Network"));
    }

    public bool IsIgnorableException(Exception e)
    {
        if (e is ObjectDisposedException || e is NullReferenceException)
            return true;

        if (e.InnerException != null)
            return IsIgnorableException(e.InnerException);

        return false;
    }

    public async ValueTask HandleStopping()
    {
        try
        {
            if (_receivetask != null && _sendtask != null)
                await Task.WhenAll(_receivetask, _sendtask).ConfigureAwait(false);
            _receivetask = null;
            _sendtask = null;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            OnError("HandleStopping", e);
        }
        finally
        {
            if (!IsStop)
                try
                {
                    OnInnerStop();
                    OnStopped();
                }
                catch (Exception e)
                {
                    if (!IsIgnorableException(e)) OnError("Unhadled Exception", e);
                }
        }
    }

    /// <summary>
    ///     发布Stop
    /// </summary>
    public async void OnStopped()
    {
        if (IsStop) return;
        IsStop = true;
        IChannel<TPackage>.ChannelStopHandle<TPackage> channelstop = null;
        //lock (_lockEvent)
        {
            channelstop = ChannelStop;
            ChannelStop = null;
        }
        if (channelstop != null)
            try
            {
                channelstop.Invoke(this, StopReason).DoNotAwait();
            }
            catch (Exception e)
            {
            }
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
        return _receivePipe.WriteAsync(token, ReceiveIOAsync);
    }

    private async Task ProcessReceiveAsync()
    {
        var writertask = HandleReceiveWritePipe(_stopTokenSource.Token);
        var readertask = _receivePipe.ReadAsync(_stopTokenSource.Token, ResolveBuffer);
        await Task.WhenAll(writertask, readertask).ConfigureAwait(false);
        await _sendPipe.WriteCompletedAsync().ConfigureAwait(false); //不允许发送
        WriteEOFPackage(); //通知读取包迭代结束
    }

    protected abstract ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token);

    protected abstract ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token);

    public void WriteEOFPackage()
    {
        _channel.Writer.TryComplete();
    }

    private async Task ProcessSendAsync()
    {
        await _sendPipe.ReadAsync(_stopTokenSource.Token, SendIOAsync);
    }

    private bool ResolveBuffer(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
        out SequencePosition examined)
    {
        consumed = buffer.Start;
        examined = buffer.End;
        var bytesConsumedTotal = 0L;
        var maxPackageLength = _option.MaxPackageLength;
        var reader = new SequenceReader<byte>(buffer);
        while (true)
        {
            var packagefilter = _packageFilter;
            var packageinfo = packagefilter.Filter(ref reader);

            var bytescomsumed = reader.Consumed;
            bytesConsumedTotal += bytescomsumed;

            var len = bytescomsumed;
            // nothing has been consumed, need more data
            if (len == 0) len = buffer.Length;

            if (maxPackageLength > 0 && len > maxPackageLength)
            {
                OnError($"Package larger than {maxPackageLength}");
                StopReason = StopReason.ProtocolError;
                //直接关闭连接
                _stopTokenSource.Cancel();
                return false;
            }

            if (packageinfo == null)
            {
                // the current pipeline filter needs more data to process
                //if (!filterSwitched)
                //{
                // set consumed position and then continue to receive...
                // 分包了 实际消耗的位置 剩余分包的保存 继续接收
                consumed = buffer.GetPosition(bytesConsumedTotal);
                return true;
                //}

                // we should reset the previous pipeline filter after switch
                // 第一个过滤器已经处理了 ，继续循环让第二个过滤器处理
                //packageramp.Reset();
            }

            // reset the pipeline filter after we parse one full package
            packagefilter.Reset();
            _channel.Writer.TryWrite(packageinfo);

            if (reader.End) // no more data
            {
                examined = consumed = buffer.End;
                return true;
            }

            if (bytescomsumed > 0)
                reader = new SequenceReader<byte>(reader.Sequence.Slice(bytescomsumed)); //slice(startindex)
        }
    }
}

public class TcpChannel<TPackage> : Channel<TPackage>
{
    private Socket _socket;

    private List<ArraySegment<byte>> segmentsForSend;

    public TcpChannel(IContainer container, Socket socket) : base(container)
    {
        _socket = socket;
    }

    protected override void OnInnerStop()
    {
        var socket = _socket;
        if (socket == null) return;
        if (Interlocked.CompareExchange(ref _socket, null, socket) == socket) socket.Dispose();
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        if (buffer.IsSingleSegment)
            return await _socket.SendAsync(GetArrayByMemory(buffer.First), SocketFlags.None)
                .ConfigureAwait(false);

        if (segmentsForSend == null)
            segmentsForSend = new List<ArraySegment<byte>>();
        else
            segmentsForSend.Clear();
        var segments = segmentsForSend;
        foreach (var piece in buffer)
        {
            token.ThrowIfCancellationRequested();
            segmentsForSend.Add(GetArrayByMemory(piece));
        }

        token.ThrowIfCancellationRequested();
        return await _socket.SendAsync(segmentsForSend, SocketFlags.None).ConfigureAwait(false);
    }

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        if (!MemoryMarshal.TryGetArray(memory, out var result))
            throw new InvalidOperationException("Buffer backed by array was expected");
        return result;
    }

    protected override async ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token)
    {
        //这边必须要带入Token否则 close的时候关不掉。这边还在Await
#if NET
        return await _socket.ReceiveAsync(GetArrayByMemory((ReadOnlyMemory<byte>)memory), SocketFlags.None, token);
#else
            var canceltasksource = new TaskCompletionSource<int>();
            _socket.ReceiveAsync(GetArrayByMemory((ReadOnlyMemory<byte>)memory), SocketFlags.None)
                .ContinueWith(t => canceltasksource.TrySetResult(t.Result)).DoNotAwait();
            token.Register((cancel) => ((TaskCompletionSource<int>)cancel)?.TrySetResult(0),canceltasksource); //存在问题 每次都会注入，什么时候释放
            return await canceltasksource.Task;
#endif
    }
}

public class UdpServerChannel<TPackage> : Channel<TPackage>
{
    private readonly IPEndPoint _remoteEndPoint;
    private Socket _socket;

    public UdpServerChannel(IContainer container, Socket socket, IPEndPoint ipendpoint) : base(container)
    {
        _socket = socket;
        _remoteEndPoint = ipendpoint;
    }

    protected override void OnInnerStop()
    {
        var socket = _socket;
        if (socket == null) return;
        Interlocked.CompareExchange(ref _socket, null, socket);
    }

    private void MergeBuffer(ref ReadOnlySequence<byte> buffer, byte[] destBuffer)
    {
        Span<byte> destSpan = destBuffer;

        var total = 0;

        foreach (var piece in buffer)
        {
            piece.Span.CopyTo(destSpan);
            total += piece.Length;
            destSpan = destSpan.Slice(piece.Length);
        }
    }

    protected ArraySegment<T> GetArrayByMemory<T>(ReadOnlyMemory<T> memory)
    {
        if (!MemoryMarshal.TryGetArray(memory, out var result))
            throw new InvalidOperationException("Buffer backed by array was expected");
        return result;
    }

    protected override async ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        if (buffer.IsSingleSegment)
        {
            var total = 0;
            foreach (var piece in buffer)
                total += await _socket.SendAsync(GetArrayByMemory(buffer.First), SocketFlags.None);
            return total;
        }

        var pool = ArrayPool<byte>.Shared;
        var destBuffer = pool.Rent((int)buffer.Length);
        try
        {
            MergeBuffer(ref buffer, destBuffer);
#if NET
            return await _socket.SendToAsync(new ArraySegment<byte>(destBuffer, 0, (int)buffer.Length),
                SocketFlags.None, _remoteEndPoint, token);
#else
                var canceltasksource = new TaskCompletionSource<int>();
                _socket.SendToAsync(new ArraySegment<byte>(destBuffer, 0, (int)buffer.Length),
                    SocketFlags.None, _remoteEndPoint).ContinueWith(t => canceltasksource.TrySetResult(t.Result)).DoNotAwait();
                token.Register((cancel) => ((TaskCompletionSource<int>)cancel)?.TrySetResult(0), canceltasksource); //存在问题 每次都会注入，什么时候释放
                return await canceltasksource.Task;
#endif
        }
        finally
        {
            pool.Return(destBuffer);
        }
    }

    public async ValueTask<FlushResult> WriteReceiveDataAsync(Memory<byte> memory, CancellationToken cancellationtoken)
    {
        return await _receivePipe._writer.WriteAsync(memory, cancellationtoken).ConfigureAwait(false);
    }


    protected override Task HandleReceiveWritePipe(CancellationToken token)
    {
        return Task.CompletedTask;
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