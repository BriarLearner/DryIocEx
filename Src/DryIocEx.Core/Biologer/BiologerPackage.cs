
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.HIS;
using DryIocEx.Core.NetworkPro;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace DryIocEx.Core.Biologer;
public class BiologerPackage
{
    public short Length { get; set; }

    public short ID { get; set; }

    public short Counter { get; set; }

    public short DeviceID { get; set; }

    public int AI { get; set; }

    public int PFC { get; set; }

    public byte[] Value { get; set; }

    public void CalLength()
    {
        Length = (short)(Value.Length + 16);
    }

}


public class BiologerPackageFilter : IPackageFilter<BiologerPackage>

{
    public  BiologerPackage Filter(ref SequenceReader<byte> reader)
    {
        var temreader = new SequenceReader<byte>(reader.Sequence);


        if (temreader.TryReadBigEndian(out short length))
        {
            
            if (length <= 16 || length >= 1000)
            {
                reader.Advance(2);
                return null;
            }
           
            var readlength = temreader.Length;
            if (readlength < length)
            {
                
                return null;
            }
            reader.Advance(length);

            var fullpackage = reader.Sequence.Slice(0, length);
            return Decode(ref fullpackage, null);
        }
        return null;
    }
    public BiologerPackage Decode(ref ReadOnlySequence<byte> fullpackage, object context)
    {
        var package = new BiologerPackage();
        try
        {
            var packagereader = new SequenceReader<byte>(fullpackage);
            packagereader.TryReadBigEndian(out short packagelength);
            package.Length = packagelength;

            packagereader.TryReadBigEndian(out short packageid);
            package.ID = packageid;

            packagereader.TryReadBigEndian(out short packagecount);
            package.Counter = packagecount;

            packagereader.TryReadBigEndian(out short packagedeviceid);
            package.DeviceID = packagedeviceid;

            packagereader.TryReadBigEndian(out int packageai);
            package.AI = packageai;

            packagereader.TryReadBigEndian(out int packagepfc);
            package.PFC = packagepfc;
            package.Value = fullpackage.Slice(16).ToArray();

        }
        catch (Exception e)
        {

        }
        return package;
    }

    public ReadOnlyMemory<byte> Converter(BiologerPackage pack)
    {
        if (pack == null || pack.Length <= 16 || pack.Value == null)
            return Array.Empty<byte>();
        if (pack.Length != (pack.Value.Length + 16))
            return Array.Empty<byte>();
        var list = new List<byte>();
        list.AddRange(pack.Length.GetBigEndian());
        list.AddRange(pack.ID.GetBigEndian());
        list.AddRange(pack.Counter.GetBigEndian());
        list.AddRange(pack.DeviceID.GetBigEndian());
        list.AddRange(pack.AI.GetBigEndian());
        list.AddRange(pack.PFC.GetBigEndian());
        list.AddRange(pack.Value);
        return list.ToArray();
    }

    public void Reset()
    {
       
    }
}

public static class BiologerExtension
{

    public static byte[] GetBigEndian<T>(this T value) where T : struct
    {
       

        try
        {
            var buffer = BitConverter.GetBytes((dynamic)value);
            Array.Reverse(buffer);
            return buffer;
        }
        catch (Exception e)
        {
        }
        return Array.Empty<byte>();
    }
}

public interface IBiologerServer
{
    /// <summary>
    /// 接收包组
    /// </summary>
    event Action<IPEndPoint, BiologerPackage> ReceivePackage;

    /// <summary>
    /// 连接状态
    /// </summary>
    event Action<IPEndPoint, bool> ConnectState;
    /// <summary>
    /// 获得Session
    /// </summary>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    SessionOption GetSessionOption(IPEndPoint endpoint);
    void ReceiveConnect(ISession<BiologerPackage> session);
    void ReceivePackageCore(ISession<BiologerPackage> session, BiologerPackage package);
    void DisConnect(ISession<BiologerPackage> session);

    Task<EnumSendResult> SendPackage(IPEndPoint ipendpoint, BiologerPackage list);
}

public class BiologerServer : IBiologerServer
{
    /// <summary>
    /// 接收包组
    /// </summary>
    public event Action<IPEndPoint, BiologerPackage> ReceivePackage;
    public event Action<IPEndPoint, bool> ConnectState;
    public void ReceiveConnect(ISession<BiologerPackage> session)
    {
        throw new NotImplementedException();
    }
    private ConcurrentDictionary<IPEndPoint, ISession<BiologerPackage>> _sessionDict =
        new ConcurrentDictionary<IPEndPoint, ISession<BiologerPackage>>();
    public void DisConnect(ISession<BiologerPackage> session)
    {
        throw new NotImplementedException();
    }
    public SessionOption GetSessionOption(IPEndPoint endpoint)
    {
        throw new NotImplementedException();
    }
   


    public void ReceivePackageCore(ISession<BiologerPackage> session, BiologerPackage package)
    {
        throw new NotImplementedException();
    }

    public async Task<EnumSendResult> SendPackage(IPEndPoint endpoint, BiologerPackage package)
    {
        throw new NotImplementedException();
    }

    public SGSpinLock _sendQueueLock = new SGSpinLock();
    private readonly ConcurrentDictionary<IPEndPoint, ConcurrentQueue<SendBiologerInfo>> SendQueue =
        new ConcurrentDictionary<IPEndPoint, ConcurrentQueue<SendBiologerInfo>>();
    private readonly ConcurrentDictionary<IPEndPoint, int> SendFlag = new ConcurrentDictionary<IPEndPoint, int>();

    private void InnerStartSend(IPEndPoint ipendpoint)
    {
        
    }

    private void InnerSend(IPEndPoint ipendpoint)
    {

        
    }

    private readonly ConcurrentDictionary<IPEndPoint, ConcurrentDictionary<(int, int), TaskCompletionSource<int>>> WaitACKDict = new();//ID，PFC
    private async Task<EnumSendResult> InnerSendPackage(IPEndPoint ipendpoint, BiologerPackage package)
    {
        throw new NotImplementedException();
    }


}

public class SendBiologerInfo
{
    public BiologerPackage Package { get; }

    public SendBiologerInfo(BiologerPackage package)
    {
        Package = package;
    }

    public EnumSendResult Result { set; get; } = EnumSendResult.None;
    public TaskCompletionSource<bool> WaitTask { get; } = new TaskCompletionSource<bool>();
}

public static class BiologerPackageStore
{
    public static BiologerPackage CreateAck(int id, int pfc)
    {
        throw new NotImplementedException();
    }

    public static byte[] CreateValue(params dynamic[] items)
    {
        throw new NotImplementedException();
    }
    public static byte[] GetBigEndian(dynamic value)
    {
        throw new NotImplementedException();
    }

    public static BiologerPackage CreateHeart()
    {
        throw new NotImplementedException();
    }
}


public interface IBiologerHeartClientMiddleware : IMiddleware<BiologerPackage>
{

}
public class BiologerHeartClientMiddleware : IBiologerHeartClientMiddleware
{
    public int Order { get; }


    public BiologerHeartClientMiddleware()
    {
        HeartPackage = BiologerPackageStore.CreateHeart();
    }


    public BiologerPackage HeartPackage { get; }
    public ValueTask Register(ISession<BiologerPackage> session)
    {
        throw new NotImplementedException();
    }

    public ValueTask UnRegister(ISession<BiologerPackage> session)
    {
        return new ValueTask();
    }
    public ValueTask Handle(ISession<BiologerPackage> session, BiologerPackage package)
    {
        return new ValueTask();
    }
}

public interface IBiologerMiddleware : IMiddleware<BiologerPackage>
{

}

public class BiologerMiddleware : IBiologerMiddleware
{

    private readonly IBiologerServer BiologerServer;
    public int Order { get; }
    public BiologerMiddleware(IBiologerServer server)
    {
        BiologerServer = server;
    }
    public ValueTask Register(ISession<BiologerPackage> channel)
    {
       
            BiologerServer.ReceiveConnect(channel);
       
        
        return new ValueTask();
    }

    public ValueTask UnRegister(ISession<BiologerPackage> channel)
    {
        if (channel != null)
            BiologerServer.DisConnect(channel);
        return new ValueTask();
    }

    public  ValueTask Handle(ISession<BiologerPackage> session, BiologerPackage package)
    {
        try
        {
            BiologerServer.ReceivePackageCore(session, package);
        }
        catch (Exception e)
        {
           
        }
        
        return new ValueTask();
    }
}


public class BiologerClientBuilder : TcpClientBuilder<BiologerPackage, BiologerClientBuilder>
{
    public BiologerClientBuilder(IBiologerServer server)
    {
        UseMiddleware(new BiologerMiddleware(server));
        UseReconnect();
        UseMiddleware<BiologerHeartClientMiddleware>();
        UseFilter<BiologerPackageFilter>();
    }
}

public class BiologerServerBuilder : TcpServerBuilder<BiologerPackage, BiologerServerBuilder>
{
    public BiologerServerBuilder(IBiologerServer server)
    {
        UseMiddleware(new BiologerMiddleware(server));
        UseFilter<BiologerPackageFilter>();
    }
}
