using SuddenGale.Core.IOC;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using SuddenGale.Core.Event;
using SuddenGale.Core.Extensions;
using SuddenGale.Core.Log;
using IContainer = SuddenGale.Core.IOC.IContainer;
using System.Runtime.InteropServices;

namespace SuddenGale.Core.IOCPNetwork
{
    public interface IChannelOption
    {
        // 1M by default
        int MaxPackageLength { get; set; }

        // 4k by default
        int ReceiveBufferSize { get; set; } 

        // 4k by default
        int SendBufferSize { get; set; } 

        /// <summary>
        /// Nagle算法是一种用于减少小数据包发送的网络优化算法。它通过将小数据包缓冲在本地，组合成更大的数据包进行发送，从而减少网络传输中的开销。这种方式可以提高网络的利用率，特别是在需要频繁发送小数据包的场景下
        /// </summary>
        bool NoDelay { set; get; } 
    }

    public class ChannelOption:IChannelOption
    {
        // 1M by default
        public int MaxPackageLength { get; set; } = 1024 * 1024;

        // 4k by default
        public int ReceiveBufferSize { get; set; } = 1024 * 4;

        // 4k by default
        public int SendBufferSize { get; set; } = 1024 * 4;

        /// <summary>
        /// Nagle算法是一种用于减少小数据包发送的网络优化算法。它通过将小数据包缓冲在本地，组合成更大的数据包进行发送，从而减少网络传输中的开销。这种方式可以提高网络的利用率，特别是在需要频繁发送小数据包的场景下
        /// </summary>
        public bool NoDelay { set; get; } = true;
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

    internal interface IPipeHost
    {
        DateTime LastActiveTime { get; set; }

        StopReason StopReason { get; set; }
        CancellationTokenSource StopTokenSource { get; }
        void OnError(string message, Exception exception = null);

        bool IsIgnorableException(Exception e);
    }



    public interface IChannel<TPackage>
    {

        IContainer Container { get;  }

        bool IsStop { get; }

        StopReason StopReason { get; }

        DateTime StartTime { get; }

        DateTime LastActiveTime { get; }

        ChannelEventStop<TPackage> Stopped { get; }

        void Start();

        void Stop(StopReason reason = StopReason.LocalClosing);

        ValueTask SendAsync(ReadOnlyMemory<byte> data);

        ValueTask SendAsync(TPackage package);

        IAsyncEnumerable<TPackage> RunAsync();

        ValueTask<TPackage> ReceiveAsync();

    }
    delegate bool HandleReceive(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
        out SequencePosition examined);

    delegate ValueTask<int> FillDataAsync(Memory<byte> buffer, CancellationToken token);

    internal class ReceivePipe
    {
        private int _receiveSize;
        private int _maxSize;
        private PipeWriter _writer;
        private PipeReader _reader;
        private IPipeHost _pipeHost;
        public ReceivePipe(IPipeHost host, int receivesize,int maxsize)
        {
            _pipeHost = host;
            var pipe = new Pipe();
            _writer = pipe.Writer;
            _reader= pipe.Reader;
            _receiveSize=receivesize<0?1024*4:receivesize;
            _maxSize=maxsize<0?1024*1024:maxsize;
        }

        public async Task ReadAsync(CancellationToken token, HandleReceive handlereceive)
        {
            while (!token.IsCancellationRequested)
            {
                ReadResult result;
                try
                {
                    result = await _reader.ReadAsync(token).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (!_pipeHost.IsIgnorableException(e))
                    {
                        _pipeHost.OnError("Read ReceivePipe Error",e);
                        break;
                    }
                    continue;
                }
                var buffer = result.Buffer;
                var consumed = buffer.Start;
                var examined = buffer.End; //核验位置
                if (result.IsCanceled)
                    break;
                var completed = result.IsCompleted;
                try
                {
                    if (buffer.Length > 0)
                    {
                        if (!handlereceive(ref buffer, out consumed, out examined))
                        {
                            completed = true;
                            break;
                        }
                    }
                    if (completed) break;
                }
                catch (Exception e)
                {
                    _pipeHost.OnError("RampError", e);
                    if (_pipeHost.StopReason == StopReason.Unknown)
                        _pipeHost.StopReason = StopReason.ProtocolError;
                    _pipeHost.StopTokenSource.Cancel();
                    break;
                }
                finally
                {
                    _reader.AdvanceTo(consumed,examined);
                }
            }
            await _reader.CompleteAsync().ConfigureAwait(false);
        }


        public async Task WriteAsync(CancellationToken token, FillDataAsync filldataasync)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var memory = _writer.GetMemory(_receiveSize);
                    var bytesread = await filldataasync(memory, token).ConfigureAwait(false);
                    if (bytesread == 0)
                    {
                        //远程关闭
                        if (_pipeHost.StopReason == StopReason.Unknown)
                            _pipeHost.StopReason = StopReason.RemoteClosing;
                        break;
                    }
                    _pipeHost.LastActiveTime= DateTime.Now;
                    _writer.Advance(bytesread);

                }
                catch (Exception e)
                {
                    if (_pipeHost.IsIgnorableException(e))
                    {
                        if (_pipeHost.StopReason == StopReason.Unknown)
                            _pipeHost.StopReason = StopReason.RemoteClosing;
                    }
                    else
                    {
                        _pipeHost.OnError("ReceiveAsyncError",e);
                        if (_pipeHost.StopReason == StopReason.Unknown)
                            _pipeHost.StopReason = token.IsCancellationRequested
                                ? StopReason.LocalClosing
                                : StopReason.SocketError;
                    }
                    break;
                }
                var result = await _writer.FlushAsync(token).ConfigureAwait(false);
                if(result.IsCanceled) break;
            }
            await _writer.CompleteAsync().ConfigureAwait(false);
        }
    }

    delegate  ValueTask<int> SendAsync(ReadOnlySequence<byte> buffer, CancellationToken token);

    internal class SendPipe
    {
        private IPipeHost _pipeHost;
        private PipeReader _reader;

        private PipeWriter _writer;

        public PipeWriter Writer=>_writer;
        private SemaphoreSlim _sendlock;

        public SendPipe(IPipeHost host)
        {
            _pipeHost = host;
            var pipe = new Pipe();
            _reader = pipe.Reader;
            _writer = pipe.Writer;
            _sendlock = new(1, 1);
        }


        public async ValueTask ReadAsync(CancellationToken token, SendAsync sendasync)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _reader.ReadAsync(CancellationToken.None).ConfigureAwait(false);
                var completed = result.IsCompleted;
                var buffer = result.Buffer;
                var end = buffer.End;
                if (!buffer.IsEmpty)
                { 
                    try
                    {
                        await sendasync(buffer, token);
                        _pipeHost.LastActiveTime = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        _pipeHost.StopTokenSource.Cancel();
                        if (!_pipeHost.IsIgnorableException(e)) _pipeHost.OnError("SendAsync Error", e);
                        break;
                    }
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
                _writer.Write(data.Span);
                await _writer.FlushAsync();
            }
            finally
            {
                _sendlock.Release();
            }
        }

        public ValueTask WriteCompletedAsync()
        {
            return _writer.CompleteAsync();
        }

    }


    public class ChannelEventStop<TPackage> : PubSubEvent<IChannel<TPackage>, StopReason>
    {

    }



    public abstract class Channel<TPackage>:IChannel<TPackage>,IPipeHost
    {
        public IContainer Container { get; }
        public bool IsStop { get; private set; }
        public StopReason StopReason { get; set; }
        public DateTime StartTime { get; private set; }
        public DateTime LastActiveTime { get; set; }

        

        public ChannelEventStop<TPackage> Stopped { get; }=new ChannelEventStop<TPackage>();

        private IPackageRamp<TPackage> _packageRamp { get; }
        private CancellationTokenSource _stopTokenSource;


        public CancellationTokenSource StopTokenSource=> _stopTokenSource;
        private System.Threading.Channels.Channel<TPackage> _channel;

        private ILogManager _logManager;
        public Channel(IContainer container)
        {
            Container=container;
            Option = container.Resolve<IChannelOption>().As<ChannelOption>();
            _packageRamp = container.Resolve<IPackageRamp<TPackage>>();
            _stopTokenSource = new CancellationTokenSource();
            _channel = Channel.CreateUnbounded<TPackage>();
            _receivePipe = new ReceivePipe(this, Option.ReceiveBufferSize, Option.MaxPackageLength);
            _sendPipe = new SendPipe(this);
            _logManager = Container.Resolve<ILogManager>();
        }

        public ChannelOption Option { get; }

        public void Start()
        {
            StartTime=DateTime.Now;
            _receivetask = ProcessReceiveAsync();
            _sendtask = ProcessSendAsync();
            HandleStopping().ConfigureAwait(false);
        }

        private Task _receivetask;
        private Task _sendtask;


        public bool IsIgnorableException(Exception e)
        {
            if (e is ObjectDisposedException || e is NullReferenceException)
                return true;

            if (e.InnerException != null)
                return IsIgnorableException(e.InnerException);

            return false;
        }

        private ReceivePipe _receivePipe;
        private SendPipe _sendPipe;

        private async Task ProcessReceiveAsync()
        {
            var writertask = _receivePipe.WriteAsync(_stopTokenSource.Token, ReceiveIOAsync);
            var readertask = _receivePipe.ReadAsync(_stopTokenSource.Token, ResolveBuffer);
            await Task.WhenAll(writertask, readertask).ConfigureAwait(false);
            await _sendPipe.WriteCompletedAsync().ConfigureAwait(false); //不允许发送
            WriteEOFPackage(); //通知读取包迭代结束
        }


        private async Task ProcessSendAsync()
        {
           
            await _sendPipe.ReadAsync(_stopTokenSource.Token, SendIOAsync);
        }

        public void WriteEOFPackage()
        {
            _channel.Writer.TryComplete();
        }
        protected abstract ValueTask<int> SendIOAsync(ReadOnlySequence<byte> buffer, CancellationToken token);
        protected abstract ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token);

        private bool ResolveBuffer(ref ReadOnlySequence<byte> buffer, out SequencePosition consumed,
            out SequencePosition examined)
        {
            consumed = buffer.Start;
            examined = buffer.End;
            var bytesConsumedTotal = 0L;
            var maxPackageLength = Option.MaxPackageLength;
            var tembuffer = buffer;
            while (true)
            {
                var packageramp = _packageRamp;
                var packageinfo = packageramp.Filter(tembuffer, out var singleconsumed);
                
                bytesConsumedTotal += singleconsumed;

                long len = singleconsumed;
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
                else
                {
                    // reset the pipeline filter after we parse one full package
                    //packageramp.Reset();
                    _channel.Writer.TryWrite(packageinfo);
                }

                if (bytesConsumedTotal>=buffer.Length) // no more data
                {
                    examined = consumed = buffer.End;
                    return true;
                }

                if (singleconsumed > 0)
                    tembuffer=tembuffer.Slice(singleconsumed); //slice(startindex)
            }
        }

        #region Stop

        public async ValueTask HandleStopping()
        {
            try
            {
                if(_receivetask!=null&&_sendtask!=null)
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
                {
                    try
                    {
                        OnInnerStop();
                        OnStopped();
                    }
                    catch (Exception e)
                    {
                        if(!IsIgnorableException(e)) OnError("Unhadled Exception",e);
                    }
                }
            }
        }

        public void OnError(string message, Exception exception = null)
        {
            if (exception != null)
            {
                _logManager.BroadcastLog(exception.ToLogInfo(message));
            }
            else
            {
                _logManager.BroadcastLog(message.ToLogInfo(EnumLogDegree.Info,"Network"));
            }
            
        }

        /// <summary>
        /// 发布Stop
        /// </summary>
        public void OnStopped()
        {
            if (IsStop) return;
            IsStop = true;
            Stopped.Publish(this,StopReason);
            Stopped.Clear();
        }

        /// <summary>
        /// 子类Stop 
        /// </summary>
        protected abstract void OnInnerStop();

        public void Stop(StopReason reason = StopReason.LocalClosing)
        {
            if (IsStop)
            {
                return;
            }
            StopReason = reason;
            _stopTokenSource.Cancel();
        }

        #endregion
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
            return _packageRamp.WriteAsync(_sendPipe.Writer, package);
        }

        public async IAsyncEnumerable<TPackage> RunAsync()
        {
            while (true)
            {
                if(IsStop) yield break;
                TPackage package = default;
                try
                {
                    package = await _channel.Reader.ReadAsync().ConfigureAwait(false);
                    if (package==null)
                    {
                        throw new ArgumentException("null package");
                    }

                }
                catch (Exception e)
                {

                    if (!(e is ChannelClosedException))
                    {
                        OnError("channel ReadAsync", e);
                    }
                    _stopTokenSource.Cancel();
                    yield break;
                }
                yield return package;
            }
        }

        public async ValueTask<TPackage> ReceiveAsync()
        {
            if (IsStop) return default;
            var package=await _channel.Reader.ReadAsync().ConfigureAwait(false);
            if (package == null)
            {
                _stopTokenSource.Cancel();
                return default;
            }
            return package;
        }
    }

    public class TcpChannel<TPackage> : Channel<TPackage>
    {
        private Socket _socket;

        public TcpChannel(IContainer container, Socket socket):base(container)
        {
            _socket=socket;
        }
        private List<ArraySegment<byte>> segmentsForSend;
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
        protected override async  ValueTask<int> ReceiveIOAsync(Memory<byte> memory, CancellationToken token)
        {
            return await _socket
                .ReceiveAsync(GetArrayByMemory((ReadOnlyMemory<byte>)memory), SocketFlags.None)
                .ConfigureAwait(false);
        }

        protected override void OnInnerStop()
        {
            var socket=_socket;
            if (socket == null) return;
            if (Interlocked.CompareExchange(ref _socket, null, socket) == socket)
            {
                socket.Dispose();
            }
        }
    }
}
