#if NET


using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DryIocEx.Core.Common;
using DryIocEx.Core.Network;

namespace DryIocEx.Core.HIS;

public class HISPackageBuilder<Self>
    where Self : HISPackageBuilder<Self>
{
    protected HISPackage _collection;

    public HISPackageBuilder()
    {
    }

    public HISPackageBuilder(HISPackage collection)
    {
        throw new NotImplementedException();
    }

    public virtual Self Initial(HISPackage package)
    {
        throw new NotImplementedException();
    }

    public Self Set<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public Self SetCmd(string str)
    {
        throw new NotImplementedException();
    }

    public HISPackage Build()
    {
        throw new NotImplementedException();
    }
}

public class DefaultPackageBuilder : HISPackageBuilder<DefaultPackageBuilder>
{
    public DefaultPackageBuilder(HISPackage collection) : base(collection)
    {
    }
}

public struct HISPackageBuilder
{
    private readonly HISPackage _collection;

    public HISPackageBuilder(HISPackage package)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder Set<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder Set(Action<HISPackage> action)
    {
        throw new NotImplementedException();
    }

    public HISPackageBuilder SetCmd(string str)
    {
        throw new NotImplementedException();
    }

    public HISPackage Build()
    {
        throw new NotImplementedException();
    }
}

public class HISPackagePool : Pool<HISPackage>
{
    public override void Return(HISPackage sub)
    {
        throw new NotImplementedException();
    }

    public override HISPackage Rent()
    {
        throw new NotImplementedException();
    }
}

public class HISPackage : IDisposable
{
    private static uint staticid;
    public readonly KeyValueCollection Data;


    private int disflag;

    /// <summary>
    ///     不需要用户调用
    /// </summary>
    public HISPackage()
    {
        throw new NotImplementedException();
    }

    internal HISPackage(KeyValueCollection collection)
    {
        throw new NotImplementedException();
    }


    internal static IPool<HISPackage> Pool { get; } = new HISPackagePool();
    public bool Disposable { get; private set; }

    /// <summary>
    ///     内部回收机制
    ///     不调用Dispose不会复用，由GC回收
    /// </summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public HISPackage Clone()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     不需要用户调用
    /// </summary>
    public void Initial()
    {
        throw new NotImplementedException();
    }

    public static HISPackageBuilder Create(string cmd = "")
    {
        throw new NotImplementedException();
    }

    

    #region 属性

    /// <summary>
    ///     发送次数
    /// </summary>
    public int SendCount
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public DateTime NotifyTime
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     版本
    /// </summary>
    public int Version
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     id
    /// </summary>
    public uint Id
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     uid
    /// </summary>
    public string Uid
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public uint Sid
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int SendId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int WorkId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public int BufferId
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     优先级
    /// </summary>
    public CommandPriority Priority
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    /// <summary>
    ///     获取该指令的名称。
    /// </summary>
    public string Cmd
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public DateTime CreateTime
    {
        set => throw new NotImplementedException();
        get => throw new NotImplementedException();
    }

    public bool IsAck => throw new NotImplementedException();

    public bool IsNak => throw new NotImplementedException();

    public bool IsHeartBeat => throw new NotImplementedException();

    public bool IsWorkCmd => throw new NotImplementedException();

    #endregion
}

public static class HISPackageExtension
{
    public static IEnumerable<KeyValuePair<string, string>> GetAll(this HISPackage package)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Remove(this HISPackage package, string key)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Add<T>(this HISPackage package, string key, T value)
    {
        throw new NotImplementedException();
    }

    public static bool TryGet<T>(this HISPackage package, string key, out T value)
    {
        throw new NotImplementedException();
    }

    public static T Get<T>(this HISPackage package, string key)
    {
        throw new NotImplementedException();
    }

    public static HISPackage Clone(this HISPackage packge)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     指令的优先级。
/// </summary>
public enum CommandPriority : byte
{
    /// <summary>
    ///     高优先级。
    /// </summary>
    High = 10,

    /// <summary>
    ///     普通优先级。
    /// </summary>
    Normal = 20,

    /// <summary>
    ///     低优先级。
    /// </summary>
    Low = 30
}

public class HISPackageFilter : PackageFilter<HISPackage>
{
    public const byte STX = 0x02;

    public const byte ETX = 0x03;

    public const byte GS = 0x1D;

    public const byte RS = 0x1E;


    public override HISPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }
}

public class HISPackageDecoder : IPackageDecoder<HISPackage>
{
    public readonly byte RS = 0x1E;

    public HISPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        throw new NotImplementedException();
    }

    private void ParseKeyValue(ref ReadOnlySequence<byte> bytes, ref HISPackageBuilder builder)
    {
        throw new NotImplementedException();
    }
}

public class HISPackageEncoder : IPackageEncoder<HISPackage>
{
    public const byte STX = 0x02;

    public const byte ETX = 0x03;

    public const byte GS = 0x1D;

    public const byte RS = 0x1E;

    public int Encode(IBufferWriter<byte> writer, HISPackage pack)
    {
        throw new NotImplementedException();
    }
}

public class
    HISServerBuilder : SimpleServerBuilder<HISPackage, HISPackageFilter, HISPackageEncoder, HISPackageDecoder>
{
    public HISServerBuilder()
    {
        throw new NotImplementedException();
    }
}

public class HISClientBuilder : SimpleClientBuilder<HISPackage, HISPackageFilter, HISPackageEncoder, HISPackageDecoder>
{
    public HISClientBuilder()
    {
        throw new NotImplementedException();
    }
}

public class HISUDPServerBuilder : UdpServerBuilder<HISPackage, HISPackageFilter, HISPackageEncoder, HISPackageDecoder>
{
    public HISUDPServerBuilder()
    {
        throw new NotImplementedException();
    }
}

public class HISUDPClientBuilder : UdpClientBuilder<HISPackage, HISPackageFilter, HISPackageEncoder, HISPackageDecoder>
{
    public HISUDPClientBuilder()
    {
        throw new NotImplementedException();
    }
}

#endif