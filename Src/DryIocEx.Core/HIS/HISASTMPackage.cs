using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.NetworkPro;

namespace DryIocEx.Core.HIS;

/// <summary>
///     ASTM 协议特殊字符
/// </summary>
public class ASTMConst
{
    #region 定义常量

    /// <summary>
    ///     【ENQ】
    /// </summary>
    public const byte ENQ = 0x05;

    /// <summary>
    ///     【EOT】
    /// </summary>
    public const byte EOT = 0x04;

    /// <summary>
    ///     【ACK】
    /// </summary>
    public const byte ACK = 0x06;

    /// <summary>
    ///     【NAK】
    /// </summary>
    public const byte NAK = 0x15;

    /// <summary>
    ///     心跳
    /// </summary>
    public const byte HT = 0x09;

    /// <summary>
    ///     【ETB】
    /// </summary>
    public const byte ETB = 0x17;

    /// <summary>
    ///     【ETX】
    /// </summary>
    public const byte ETX = 0x03;

    /// <summary>
    ///     【STX】
    /// </summary>
    public const byte STX = 0x02;

    /// <summary>
    ///     【CR】
    /// </summary>
    public const byte CR = 0x0D;

    /// <summary>
    ///     【LF】
    /// </summary>
    public const byte LF = 0x0A;

    public static readonly byte[] CRLF = [0x0D, 0xA];

    /// <summary>
    ///     【|】
    /// </summary>
    public const byte charField = (byte)'|';

    /// <summary>
    ///     【^】
    /// </summary>
    public const byte charComponent = (byte)'^';

    /// <summary>
    ///     【\】
    /// </summary>
    public const byte charRepeating = (byte)'\\';

    /// <summary>
    ///     【&】
    /// </summary>
    public const byte charEscape = (byte)'&';

    #endregion
}

public class HISASTMPackagePool : Pool<HISASTMPackage>
{
    public override void Return(HISASTMPackage sub)
    {
        throw new NotImplementedException();
    }

    public override HISASTMPackage Rent()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     ASTM包
/// </summary>
public class HISASTMPackage : IDisposable
{
    private int disflag;

    public HISASTMPackage()
    {
        throw new NotImplementedException();
    }

    public static IPool<HISASTMPackage> Pool { get; } = new HISASTMPackagePool();


    public DateTime CreateTime { get; private set; }

    /// <summary>
    ///     帧序号
    /// </summary>
    public int FrameIndex { set; get; }

    /// <summary>
    ///     包类型
    /// </summary>
    public EnumASTMPackageType Type { get; set; }

    /// <summary>
    ///     消息类型 消息结构中
    /// </summary>
    public string Identify { set; get; }

    /// <summary>
    ///     包序号
    /// </summary>
    public int PackageIndex { set; get; }

    /// <summary>
    ///     数据域
    /// </summary>
    public List<string> Fields { get; } = new();

    public bool Disposable { get; private set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    internal void Initial()
    {
        throw new NotImplementedException();
    }

    public HISASTMPackage Clone()
    {
        throw new NotImplementedException();
    }
}

public enum EnumASTMPackageType
{
    Unknown = 0,

    /// <summary>
    ///     心跳
    /// </summary>
    HT,

    /// <summary>
    ///     开始传送
    /// </summary>
    ENQ,

    /// <summary>
    ///     反馈
    /// </summary>
    ACK,

    /// <summary>
    ///     拒绝
    /// </summary>
    NAK,

    /// <summary>
    ///     结束传送
    /// </summary>
    EOT,

    /// <summary>
    ///     完整包
    /// </summary>
    STX
}

public class HISASTMPackageFilter : IPackageFilter<HISASTMPackage>
{
    public HISASTMPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte> Converter(HISASTMPackage pack)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
    }

    private long TryReadTo(ReadOnlySequence<byte> sequence)
    {
        throw new NotImplementedException();
    }

    private HISASTMPackage Decoder(ref ReadOnlySequence<byte> buffer, object context)
    {
        throw new NotImplementedException();
    }

    private EnumASTMPackageType GetType(SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }
}

public interface IHISASTMMiddleware : IMiddleware<HISASTMPackage>
{
}

public class HISASTMMiddleware : IHISASTMMiddleware
{
    private readonly IASTMServer ASTMServer;

    //注入的时候要额外注入Server
    public HISASTMMiddleware(IASTMServer server)
    {
        throw new NotImplementedException();
    }

    public int Order { get; }

    public ValueTask Handle(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        throw new NotImplementedException();
    }

    public ValueTask UnRegister(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    ValueTask IMiddleware<HISASTMPackage>.Register(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }
}

public interface IHISASTMHeartMiddleware : IMiddleware<HISASTMPackage>
{
}

public class HISASTMHeartMiddleware : IHISASTMHeartMiddleware
{
    public HISASTMHeartMiddleware()
    {
        throw new NotImplementedException();
    }

    private HISASTMPackage HeartPackage { get; }
    public int Order { get; }

    public ValueTask Register(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask UnRegister(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask Handle(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        throw new NotImplementedException();
    }
}

public interface IASTMServer
{
    /// <summary>
    ///     接收未知包
    /// </summary>
    event Action<IPEndPoint, HISASTMPackage> ReceiveUnknownPackage;

    /// <summary>
    ///     接收包组
    /// </summary>
    event Action<IPEndPoint, List<HISASTMPackage>, EnumGroupReason> ReceiveGroupPackage;

    /// <summary>
    ///     接收单个包
    /// </summary>
    event Action<IPEndPoint, HISASTMPackage> ReceiveSinglePackage;

    /// <summary>
    ///     连接状态
    /// </summary>
    event Action<IPEndPoint, bool> ConnectState;

    void SetSend(int timeout, int sendinternal);

    void ReceiveConnect(ISession<HISASTMPackage> session);
    void ReceivePackage(ISession<HISASTMPackage> session, HISASTMPackage package);
    void DisConnect(ISession<HISASTMPackage> session);

    Task<EnumSendResult> SendGroupASTMPackage(IPEndPoint ipendpoint, List<HISASTMPackage> list);
    void StopAll();
}

public enum EnumGroupReason
{
    None,
    Success,
    BreakUp,
    OutTime
}

internal class GroupASTMPackage
{
    private readonly Action<IPEndPoint, List<HISASTMPackage>, EnumGroupReason> action;
    private readonly int aliveTime = 30;
    private readonly List<HISASTMPackage> Group = new();

    private readonly IPEndPoint IPEndpoint;

    private bool IsGroup;

    public GroupASTMPackage(IPEndPoint ipendpoint,
        Action<IPEndPoint, List<HISASTMPackage>, EnumGroupReason> autohandlegroup, int alivetime = 30)
    {
        throw new NotImplementedException();
    }


    public EnumGroupReason GroupReason { get; private set; } = EnumGroupReason.None;

    public DateTime LastReceiveTime { get; private set; }

    public void SetGroupPackage(EnumGroupReason reason)
    {
        throw new NotImplementedException();
    }

    public void AddPackage(HISASTMPackage package)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     存到数据库的时候归还
/// </summary>
public class ASTMServer : IASTMServer
{
    /// <summary>
    ///     打包缓存
    /// </summary>
    private readonly ConcurrentDictionary<IPEndPoint, GroupASTMPackage> CacheGroupPackage = new();

    /// <summary>
    ///     连接缓存
    /// </summary>
    private readonly ConcurrentDictionary<IPEndPoint, ISession<HISASTMPackage>> Clients = new();

    /// <summary>
    ///     发送标识 1标识正在发送 0标识停止发送
    /// </summary>
    private readonly ConcurrentDictionary<IPEndPoint, SendFlagModel> SendFlag = new();

    /// <summary>
    ///     发送队列
    /// </summary>
    private readonly ConcurrentDictionary<IPEndPoint, ConcurrentQueue<SendASTMInfo>> SendQueue = new();

    /// <summary>
    ///     发送等待ACK缓存
    /// </summary>
    private readonly ConcurrentDictionary<IPEndPoint, TaskCompletionSource<int>> WaitACK = new();

    private int _sendInternal;

    private int _sendOutTime = 5;


    private readonly SGSpinLock LockSendQueue = new();

    /// <summary>
    ///     接收包
    /// </summary>
    public event Action<IPEndPoint, List<HISASTMPackage>, EnumGroupReason> ReceiveGroupPackage;

    /// <summary>
    ///     连接状态
    /// </summary>
    public event Action<IPEndPoint, bool> ConnectState;

    /// <summary>
    ///     接收单个包
    /// </summary>
    public event Action<IPEndPoint, HISASTMPackage> ReceiveSinglePackage;

    /// <summary>
    ///     设置发送参数
    /// </summary>
    /// <param name="timeout">发送超时时间/s</param>
    /// <param name="sendinternal">每次发包间隔/ms</param>
    public void SetSend(int timeout, int sendinternal)
    {
        throw new NotImplementedException();
    }

    //服务端是远程IP，客户端是本地IP，对于
    public void ReceiveConnect(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    public void DisConnect(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }


    public async void StopAll()
    {
        throw new NotImplementedException();
    }


    public event Action<IPEndPoint, HISASTMPackage> ReceiveUnknownPackage;

    public void ReceivePackage(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        throw new NotImplementedException();
    }

    public async Task<EnumSendResult> SendGroupASTMPackage(IPEndPoint ipendpoint, List<HISASTMPackage> list)
    {
        throw new NotImplementedException();
    }

    public SessionOption GetSessionOption(IPEndPoint endpoint)
    {
        throw new NotImplementedException();
    }

    private bool CanAccept(IPEndPoint endpoint)
    {
        throw new NotImplementedException();
    }


    private void HandleUnknownPackage(IPEndPoint ipendpoint, HISASTMPackage package)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     自动打包
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="group"></param>
    private void HandleAutoGroup(IPEndPoint endpoint, List<HISASTMPackage> group, EnumGroupReason reason)
    {
        throw new NotImplementedException();
    }

    private void SendACK(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    private void InnerStartSend(IPEndPoint ipendpoint)
    {
        throw new NotImplementedException();
    }

    private void InnerSend(IPEndPoint ipendpoint)
    {
        throw new NotImplementedException();
    }

    private async Task<EnumSendResult> InnerSendPackage(IPEndPoint ipendpoint, HISASTMPackage package)
    {
        throw new NotImplementedException();
    }

    private class SendFlagModel
    {
        public int IsSending;
    }
}

public enum EnumSendResult
{
    None, //NotHandle,
    NotFind,
    Success,
    TimeOut,
    Reject,
    Fail,
    PackageError
}

internal class SendASTMInfo
{
    public SendASTMInfo(List<HISASTMPackage> packages)
    {
        throw new NotImplementedException();
    }

    public List<HISASTMPackage> Packages { get; set; }

    public EnumSendResult Result { set; get; }

    public TaskCompletionSource<bool> WaitTask { get; } = new();
}

public class HISASTMClientBuilder : TcpClientBuilder<HISASTMPackage, HISASTMClientBuilder>
{
    public HISASTMClientBuilder(IASTMServer server)
    {
        throw new NotImplementedException();
    }
}

public class HISASTMServerBuilder : TcpServerBuilder<HISASTMPackage, HISASTMServerBuilder>
{
    public HISASTMServerBuilder(IASTMServer server)
    {
        throw new NotImplementedException();
    }
}