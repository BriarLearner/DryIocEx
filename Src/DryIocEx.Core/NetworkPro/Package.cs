using System;
using System.Buffers;
using System.Text;

namespace DryIocEx.Core.NetworkPro;

public interface IPackageFilter<TPackage>
{
    TPackage Filter(ref SequenceReader<byte> reader);

    ReadOnlyMemory<byte> Converter(TPackage package);

    /// <summary>
    ///     Framework4.8不支持接口方法
    /// </summary>
    void Reset();
}

public class StringPackage
{
    public StringPackage(string text)
    {
        throw new NotImplementedException();
    }

    public string Text { set; get; }
}

public class StringPackageFilter : IPackageFilter<StringPackage>
{
    public StringPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte> Converter(StringPackage package)
    {
        throw new NotImplementedException();
    }


    public void Reset()
    {
    }
}

public abstract class TerminatorPackageFilter<TPackage> : IPackageFilter<TPackage>
{
    private readonly ReadOnlyMemory<byte> _terminator;

    public TerminatorPackageFilter(ReadOnlyMemory<byte> terminator)
    {
        throw new NotImplementedException();
    }

    public TPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }


    public abstract ReadOnlyMemory<byte> Converter(TPackage package);

    public abstract void Reset();


    protected abstract TPackage Decoder(ref ReadOnlySequence<byte> sequence);
}

public class LineStringPackageFilter : TerminatorPackageFilter<StringPackage>
{
    public LineStringPackageFilter() : base(new[] { (byte)'\r', (byte)'\n' })
    {
    }

    protected override StringPackage Decoder(ref ReadOnlySequence<byte> sequence)
    {
        throw new NotImplementedException();
    }

    public override ReadOnlyMemory<byte> Converter(StringPackage package)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
    }
}

public abstract class FixedHeaderPackageFilter<TPackage> : IPackageFilter<TPackage>
{
    public TPackage Filter(ref SequenceReader<byte> reader)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte> Converter(TPackage package)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}