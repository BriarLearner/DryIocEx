#if NET


using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Network;

public interface IPackageFilterFactory<TPackage>
{
    IPackageFilter<TPackage> Create(object obj = null);
}

public interface IPackageFilter<TPackage>
{
    object Context { set; get; }
    IPackageDecoder<TPackage> Decoder { set; get; }
    IPackageFilter<TPackage> NextFilter { get; }
    void Reset();
    TPackage Filter(ref SequenceReader<byte> reader);
}

public abstract class PackageFilter<TPackage> : IPackageFilter<TPackage>
{
    public object Context { set; get; }
    public IPackageDecoder<TPackage> Decoder { get; set; }
    public abstract TPackage Filter(ref SequenceReader<byte> reader);

    public virtual void Reset()
    {
    }


    public IPackageFilter<TPackage> NextFilter { get; }
}

public class StringPackageFilter : PackageFilter<StringPackage>
{
    public Encoding Encoding { set; get; } = Encoding.UTF8;

    public override StringPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }
}

public interface IPackageDecoder<out TPackage>
{
    TPackage Decode(ref ReadOnlySequence<byte> buffer, object context);
}

public class StringPackageDecoder : IPackageDecoder<StringPackage>
{
    public StringPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        throw new NotImplementedException();
    }
}

public interface IPackageEncoder<in TPackage>
{
    int Encode(IBufferWriter<byte> writer, TPackage pack);
}

public class StringPackageEncoder : IPackageEncoder<StringPackage>
{
    public int Encode(IBufferWriter<byte> writer, StringPackage pack)
    {
        throw new NotImplementedException();
    }
}

public class StringPackage
{
    public string Text { set; get; }
}

public interface IPackageHandle<TSession, TPackage>
    where TSession : ISession<TPackage>
{
    Action<ISession<TPackage>, TPackage> HandlePackage { set; get; }

    ValueTask Handle(TSession session, TPackage package);
}

public class PackageHandle<TPackage> : IPackageHandle<ISession<TPackage>, TPackage>
{
    public Action<ISession<TPackage>, TPackage> HandlePackage { get; set; }

    public ValueTask Handle(ISession<TPackage> session, TPackage package)
    {
        throw new NotImplementedException();
    }
}

public abstract class FixedHeaderPackageFilter<TPackage> : PackageFilter<TPackage>
{
    private readonly int _headSize;
    private bool _foundHeader;
    protected int _totalSize;

    public FixedHeaderPackageFilter(int headsize)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override TPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException(); 
    }

    protected abstract int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> header);
}

public class TerminatorPackageFilter<TPackage> : PackageFilter<TPackage>
{
    private readonly ReadOnlyMemory<byte> _terminator;

    public TerminatorPackageFilter(ReadOnlyMemory<byte> terminator)
    {
        throw new NotImplementedException();
    }

    public override TPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }
}

public class LinePackageFilter<TPackage> : TerminatorPackageFilter<TPackage>
{
    public LinePackageFilter() : base(new[] { (byte)'\r', (byte)'\n' })
    {
    }
}

#endif