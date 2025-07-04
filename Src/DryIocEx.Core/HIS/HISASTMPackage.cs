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
        if (_queue.Contains(sub)) return;
        base.Return(sub);
    }

    public override HISASTMPackage Rent()
    {
        var operate = base.Rent();
        operate.Initial();
        return operate;
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

        Initial();
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
        if (Disposable) return;
        if (Interlocked.CompareExchange(ref disflag, 1, 0) != 0) return;
        Disposable = true;
        Pool.Return(this);
    }

    internal void Initial()
    {
        Interlocked.Exchange(ref disflag, 0);
        Disposable = false;

        FrameIndex = default;
        Type = default;
        Identify = default;
        PackageIndex = default;
        CreateTime = DateTime.Now;
        Fields.Clear();
    }

    public HISASTMPackage Clone()
    {
        var package = new HISASTMPackage
        {
            FrameIndex = FrameIndex,
            Type = Type,
            Identify = Identify,
            PackageIndex = PackageIndex
        };
        package.Fields.AddRange(Fields);
        return package;
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
        //1.检测第一个字节是否是 识别的字节
        var type = GetType(reader);
        //2.不是过滤掉
        if (type == EnumASTMPackageType.Unknown) //过滤掉未知数据
        {
            reader.Advance(1);
            return null;
        }

        //3.第一个字节是STX,解析STX
        if (type == EnumASTMPackageType.STX)
        {
#if NET
            var temsequence = reader.UnreadSequence;
#else
            var temsequence = reader.Sequence.Slice(reader.Consumed);
#endif

            var readlean = TryReadTo(temsequence);
            if (readlean < 0)
            {
                reader.Advance(reader.Remaining);
                return null;
            }

            if (readlean == 0) return null;
            reader.Advance(readlean);
            var spack = temsequence.Slice(0, readlean);

            
            if (spack.Length < 10) return null; 
            //3.3验证crc
#if NET
            var crc = Encoding.ASCII.GetString(spack.Slice(spack.Length - 4, 2));
#else
            var crc = Encoding.UTF8.GetString(spack.Slice(spack.Length - 4, 2).ToArray()); 
#endif

            var keyvaluereader = new SequenceReader<byte>(spack.Slice(0, spack.Length - 4)); 
            var sum = 0;
            byte temvalue = 0;
            while (keyvaluereader.TryRead(out temvalue)) sum += temvalue;
            var calcrc = (sum % 0x100).ToString("X2"); 
            if (!calcrc.Equals(crc, StringComparison.OrdinalIgnoreCase)) return null; 
            return Decoder(ref spack, null);
        }

        var package = HISASTMPackage.Pool.Rent();
        package.Type = type;
        reader.Advance(1);
        return package;
    }

    public ReadOnlyMemory<byte> Converter(HISASTMPackage pack)
    {
        if (pack == null) return Array.Empty<byte>();
        switch (pack.Type)
        {
            case EnumASTMPackageType.ENQ:
                return new[] { ASTMConst.ENQ };
            case EnumASTMPackageType.ACK:
                return new[] { ASTMConst.ACK };
            case EnumASTMPackageType.NAK:
                return new[] { ASTMConst.NAK };
            case EnumASTMPackageType.EOT:
                return new[] { ASTMConst.EOT };
            case EnumASTMPackageType.HT:
                return new[] { ASTMConst.HT };
            case EnumASTMPackageType.STX:
                var list = new List<byte>();
                list.Add(ASTMConst.STX);
                list.Add((byte)(pack.FrameIndex % 8).ToString("D1").First());
                list.Add((byte)pack.Identify.ToUpper().First());
                list.Add(ASTMConst.charField);
                list.Add((byte)pack.PackageIndex.ToString("D1").First());

                if (pack.Fields.Any())
                {
                    list.Add(ASTMConst.charField);
                    list.AddRange(Encoding.UTF8.GetBytes(string.Join("|", pack.Fields)));
                }

                list.Add(ASTMConst.CR);
                list.Add(ASTMConst.ETX);
                list.AddRange(Encoding.UTF8.GetBytes((list.Sum(s => s) % 0x100).ToString("X2")));
                list.Add(ASTMConst.CR);
                list.Add(ASTMConst.LF);
                return list.ToArray();
            default:
                return Array.Empty<byte>();
        }
    }

    public void Reset()
    {
    }

    private long TryReadTo(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            var exit = false;
            do
            {
                var find = reader.TryReadTo(out ReadOnlySequence<byte> spack, ASTMConst.CRLF);
                if (!find)
                {
                    if (reader.Length < 4 * 1024) //一个包的长度不能超过4K
                    {
                        exit = true;
                        return 0; //有可能分包了
                    }

                    exit = true;
                    return -1; //异常包 全部丢弃
                }

                if (spack.Length < 4)
                {
                }
                else
                {
                    var slice = spack.Slice(spack.Length - 4, 2);
                    var temre = new SequenceReader<byte>(slice);
                    temre.TryRead(out var temcr);
                    temre.TryRead(out var temetx);
                    if (temcr == ASTMConst.CR && (temetx == ASTMConst.ETX || temetx == ASTMConst.ETB))
                    {
                        exit = true;
                        return reader.Consumed;
                    }
                }
            } while (!exit);
        }
        catch (Exception e)
        {
        }

        return -1; //异常结束全部丢弃
    }

    private HISASTMPackage Decoder(ref ReadOnlySequence<byte> buffer, object context)
    {
        try
        {
            var content = buffer.Slice(1, buffer.Length - 7);
#if NET
            var parameters = Encoding.UTF8.GetString(content).Split('|');
#else
            var parameters = Encoding.UTF8.GetString(content.ToArray()).Split('|');
#endif

            if (parameters.Length < 2 || parameters[0].Length < 2) return null; //无效包 
            var package = HISASTMPackage.Pool.Rent();
            package.Type = EnumASTMPackageType.STX;
            package.FrameIndex = int.Parse(parameters[0][0].ToString());
            package.Identify = parameters[0][1].ToString();
            package.PackageIndex = int.Parse(parameters[1]);
            if (parameters.Length > 2) package.Fields.AddRange(parameters.Skip(2).ToArray());
            return package;
        }
        catch (Exception e)
        {
        }

        return null;
    }

    private EnumASTMPackageType GetType(SequenceReader<byte> reader)
    {
        if (reader.IsNext(ASTMConst.ENQ)) return EnumASTMPackageType.ENQ;
        if (reader.IsNext(ASTMConst.ACK)) return EnumASTMPackageType.ACK;
        if (reader.IsNext(ASTMConst.NAK)) return EnumASTMPackageType.NAK;
        if (reader.IsNext(ASTMConst.EOT)) return EnumASTMPackageType.EOT;
        if (reader.IsNext(ASTMConst.STX)) return EnumASTMPackageType.STX;
        if (reader.IsNext(ASTMConst.HT)) return EnumASTMPackageType.HT;
        return EnumASTMPackageType.Unknown;
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
        ASTMServer = server;
    }

    public int Order { get; }

    public ValueTask Handle(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        try
        {
            ASTMServer.ReceivePackage(session, package);
        }
        catch (Exception e)
        {
        }

        return new ValueTask();
    }

    public ValueTask UnRegister(ISession<HISASTMPackage> session)
    {
        if (session != null) ASTMServer.DisConnect(session);
        return new ValueTask();
    }

    ValueTask IMiddleware<HISASTMPackage>.Register(ISession<HISASTMPackage> session)
    {
        ASTMServer.ReceiveConnect(session);
        return new ValueTask();
    }
}

public interface IHISASTMHeartMiddleware : IMiddleware<HISASTMPackage>
{
}

public class HISASTMHeartMiddleware : IHISASTMHeartMiddleware
{
    public HISASTMHeartMiddleware()
    {
        var package = new HISASTMPackage();
        package.Type = EnumASTMPackageType.HT;
        HeartPackage = package;
    }

    private HISASTMPackage HeartPackage { get; }
    public int Order { get; }

    public ValueTask Register(ISession<HISASTMPackage> session)
    {
        Task.Run(async () =>
        {
            while (!session.IsStop)
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    await session.SendAsync(HeartPackage);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
                catch (Exception e)
                {
                }
        }).DoNotAwait();
        return new ValueTask();
    }

    public ValueTask UnRegister(ISession<HISASTMPackage> session)
    {
        return new ValueTask();
    }

    public ValueTask Handle(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        return new ValueTask();
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
        IPEndpoint = ipendpoint;
        action = autohandlegroup;
        aliveTime = alivetime;
    }


    public EnumGroupReason GroupReason { get; private set; } = EnumGroupReason.None;

    public DateTime LastReceiveTime { get; private set; }

    public void SetGroupPackage(EnumGroupReason reason)
    {
        GroupReason = reason;
        IsGroup = true;
        action?.Invoke(IPEndpoint, Group, GroupReason);
    }

    public void AddPackage(HISASTMPackage package)
    {
        Group.Add(package);
        LastReceiveTime = DateTime.Now;
        Task.Run(async () =>
        {
            await Task.Delay(aliveTime * 1000);
            if (DateTime.Now - LastReceiveTime > TimeSpan.FromSeconds(aliveTime) && !IsGroup)
            {
                //超时自动打包
                SetGroupPackage(EnumGroupReason.OutTime);
                action.Invoke(IPEndpoint, Group, GroupReason);
            }
        });
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
        _sendOutTime = timeout;
        _sendInternal = sendinternal;
    }

    //服务端是远程IP，客户端是本地IP，对于
    public void ReceiveConnect(ISession<HISASTMPackage> session)
    {
        var endpoint = session.GetKey<IPEndPoint>();
        if (endpoint != null && CanAccept(endpoint))
        {
            Clients[endpoint] = session;
            ConnectState?.Invoke(endpoint, true);
        }
        else
        {
            session.Stop();
        }
    }

    public void DisConnect(ISession<HISASTMPackage> session)
    {
        var endpoint = session.GetKey<IPEndPoint>();
        if (endpoint != null)
        {
            if (Clients.ContainsKey(endpoint))
            {
                ConnectState?.Invoke(endpoint, false);
                Clients.TryRemove(endpoint, out var temvalue);
            }
        }
    }


    public async void StopAll()
    {
        if (Clients.Any())
        {
            var clients = Clients.Values.ToArray();

            foreach (var client in clients) client.Stop(StopReason.RemoteClosing);

            Clients.Clear();
        }
    }


    public event Action<IPEndPoint, HISASTMPackage> ReceiveUnknownPackage;

    public void ReceivePackage(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        try
        {
            var ipendpoint = session.GetKey<IPEndPoint>();
            if (ipendpoint == null) return;

            try
            {
                ReceiveSinglePackage?.Invoke(ipendpoint, package);
            }
            catch (Exception e)
            {
            }

            if (package.Type == EnumASTMPackageType.ENQ)
            {
                SendACK(session);
                //如果包含旧数据
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    if (CacheGroupPackage.TryRemove(ipendpoint, out var group))
                    {
                        group.SetGroupPackage(EnumGroupReason.BreakUp);
                    }

                }

                //接收到新的数据开始
                var newgroup = new GroupASTMPackage(ipendpoint, HandleAutoGroup);
                newgroup.AddPackage(package);
                CacheGroupPackage[ipendpoint] = newgroup;
                return;
            }

            if (package.Type == EnumASTMPackageType.EOT)
            {
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    //正常结束
                    if (CacheGroupPackage.TryRemove(ipendpoint, out var temgroup))
                    {
                        temgroup.AddPackage(package);
                        temgroup.SetGroupPackage(EnumGroupReason.Success);
                    }
                }

                return;
            }

            //心跳
            if (package.Type == EnumASTMPackageType.HT)
            {
                package.Dispose();
                return;
            }

            if (package.Type == EnumASTMPackageType.ACK)
            {
                //发送完等待ACK,通知发送成功
                //没有发送，收到到ACK，丢弃
                if (WaitACK.ContainsKey(ipendpoint)) WaitACK[ipendpoint].TrySetResult(1);
                return;
            }

            if (package.Type == EnumASTMPackageType.NAK)
            {
                if (WaitACK.ContainsKey(ipendpoint)) WaitACK[ipendpoint].TrySetResult(3);
                return;
            }

            if (package.Type == EnumASTMPackageType.STX)
            {
                SendACK(session);
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    var group = CacheGroupPackage[ipendpoint];
                    group.AddPackage(package);
                }
                return;
            }

            HandleUnknownPackage(ipendpoint, package);
        }
        catch (Exception e)
        {
        }
    }

    public async Task<EnumSendResult> SendGroupASTMPackage(IPEndPoint ipendpoint, List<HISASTMPackage> list)
    {
        if (!Clients.ContainsKey(ipendpoint)) return EnumSendResult.NotFind;
        if (list != null && list.Any())
        {
            if (!SendQueue.ContainsKey(ipendpoint))
                try
                {
                    //这边用双检，发现外部大量 taskrun这个方法的时候，这边有问题
                    LockSendQueue.Enter();
                    if (!SendQueue.ContainsKey(ipendpoint))
                    {
                        SendQueue[ipendpoint] = new ConcurrentQueue<SendASTMInfo>();
                        SendFlag[ipendpoint] = new SendFlagModel();
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    LockSendQueue.Leave();
                }

            var sendinfo = new SendASTMInfo(list);
            var queue = SendQueue[ipendpoint];
            queue.Enqueue(sendinfo);

            InnerStartSend(ipendpoint);
            await sendinfo.WaitTask.Task;
            return sendinfo.Result;
        }

        return EnumSendResult.None;
    }

    public SessionOption GetSessionOption(IPEndPoint endpoint)
    {
        if (Clients.ContainsKey(endpoint)) return Clients[endpoint].GetOption<SessionOption>();

        return null;
    }

    private bool CanAccept(IPEndPoint endpoint)
    {
        return true;
    }


    private void HandleUnknownPackage(IPEndPoint ipendpoint, HISASTMPackage package)
    {
        try
        {
            ReceiveUnknownPackage?.Invoke(ipendpoint, package);
        }
        catch (Exception e)
        {
        }
    }

    /// <summary>
    ///     自动打包
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="group"></param>
    private void HandleAutoGroup(IPEndPoint endpoint, List<HISASTMPackage> group, EnumGroupReason reason)
    {
        try
        {
            CacheGroupPackage.TryRemove(endpoint, out var temgroup);
            ReceiveGroupPackage?.Invoke(endpoint, group, reason);
        }
        catch (Exception e)
        {
        }
    }

    private void SendACK(ISession<HISASTMPackage> session)
    {
        throw new NotImplementedException();
    }

    private void InnerStartSend(IPEndPoint ipendpoint)
    {
        if (Interlocked.CompareExchange(ref SendFlag[ipendpoint].IsSending, 1, 0) == 0)
            Task.Run(() =>
            {
                try
                {
                    InnerSend(ipendpoint);
                }
                finally
                {
                    Interlocked.Exchange(ref SendFlag[ipendpoint].IsSending, 0); //正在发送
                }
            });
    }

    private void InnerSend(IPEndPoint ipendpoint)
    {
        if (SendQueue.TryGetValue(ipendpoint, out var queue))
            try
            {
                
                while (queue.TryDequeue(out var sendinfo) && sendinfo != null)
                   
                {
                    try
                    {
                        var success = true;
                        if (sendinfo.Packages != null && sendinfo.Packages.Any())
                            foreach (var package in sendinfo.Packages)
                            {
                                var result = InnerSendPackage(ipendpoint, package).Result;
                                if (result != EnumSendResult.Success)
                                {
                                    sendinfo.Result = result;
                                    success = false;
                                    break;
                                }
                            }

                        if (success) sendinfo.Result = EnumSendResult.Success;
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        sendinfo.WaitTask.TrySetResult(true);
                    }

                    //上位机解析每次接收延迟10ms 用我自己的解析器没有问题，真不知道当时是怎么想的
                    if (_sendInternal > 0)
                        Task.Delay(_sendInternal).Wait();
                }
            }
            catch (Exception e)
            {
            }
    }

    private async Task<EnumSendResult> InnerSendPackage(IPEndPoint ipendpoint, HISASTMPackage package)
    {
        if (Clients.ContainsKey(ipendpoint))
        {
            var session = Clients[ipendpoint];
            try
            {
                if (package.Type == EnumASTMPackageType.EOT ||
                    package.Type == EnumASTMPackageType.ACK ||
                    package.Type == EnumASTMPackageType.NAK ||
                    package.Type == EnumASTMPackageType.HT
                   )
                {
                    await session.SendAsync(package);
                    return EnumSendResult.Success;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                WaitACK.TryRemove(ipendpoint, out var obcts);
            }

            return EnumSendResult.Fail;
        }

        return EnumSendResult.NotFind;
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
        UseMiddleware(new HISASTMMiddleware(server));
        UseReconnect();
        UseFilter<HISASTMPackageFilter>();
    }
}

public class HISASTMServerBuilder : TcpServerBuilder<HISASTMPackage, HISASTMServerBuilder>
{
    public HISASTMServerBuilder(IASTMServer server)
    {
        UseMiddleware(new HISASTMMiddleware(server));
        UseFilter<HISASTMPackageFilter>();
    }
}