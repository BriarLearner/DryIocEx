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
        Text = text;
    }

    public string Text { set; get; }
}

public class StringPackageFilter : IPackageFilter<StringPackage>
{
    public StringPackage Filter(ref SequenceReader<byte> reader)
    {
        return new StringPackage(reader.ReadString(Encoding.UTF8));
    }

    public ReadOnlyMemory<byte> Converter(StringPackage package)
    {
        return Encoding.UTF8.GetBytes(package.Text);
    }

    //public ValueTask WriteAsync(IBufferWriter<byte> writer, StringPackage package)
    //{

    //    var count=  writer.Write(package.Text.AsSpan(), Encoding.UTF8);
    //     return new ValueTask();
    //}

    public void Reset()
    {
    }
}

public abstract class TerminatorPackageFilter<TPackage> : IPackageFilter<TPackage>
{
    private readonly ReadOnlyMemory<byte> _terminator;

    public TerminatorPackageFilter(ReadOnlyMemory<byte> terminator)
    {
        _terminator = terminator;
    }

    public TPackage Filter(ref SequenceReader<byte> reader)
    {
        var terminator = _terminator;
        var span = terminator.Span;
        ReadOnlySequence<byte> sequence;
        if (!reader.TryReadTo(out sequence, span, false))
            return default;
        try
        {
            return Decoder(ref sequence);
        }
        finally
        {
            reader.Advance(terminator.Length);
        }
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
        return new StringPackage(sequence.GetString(Encoding.UTF8));
    }

    public override ReadOnlyMemory<byte> Converter(StringPackage package)
    {
        return Encoding.UTF8.GetBytes(package.Text);
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