#if NET


using System;
using System.Buffers;
using System.Text;

namespace DryIocEx.Core.Extensions;

public static class BufferExtension
{
    public static string ReadString(ref this SequenceReader<byte> reader, Encoding encoding, long length = 0)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     将buffer通过Encoding转换成字符串
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string GetString(this ReadOnlySequence<byte> buffer, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    public static int Write(this IBufferWriter<byte> writer, ReadOnlySpan<char> text, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}

#endif