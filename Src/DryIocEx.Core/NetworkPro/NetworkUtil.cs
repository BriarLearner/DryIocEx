using System;
using System.Buffers;
using System.Linq;
using System.Text;

namespace DryIocEx.Core.NetworkPro;

public static class NetworkExtension
{
    public static TOption As<TOption>(this IOption option)
    {
        throw new NotImplementedException();
    }

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

    /// <summary>
    ///     之后要调用flush
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static int Write(this IBufferWriter<byte> writer, ReadOnlySpan<char> text, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}