using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SuddenGale.Core.IOCPNetwork
{
    public static class NetworkUtil
    {
        public static IPEndPoint GetEndPoint(string ip, int port)
        {
            if (string.IsNullOrEmpty(ip) || port == 0) return null;
            IPAddress ipaddress;
            if ("any".Equals(ip, StringComparison.OrdinalIgnoreCase))
                ipaddress = IPAddress.Any;
            else if ("IpV6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
                ipaddress = IPAddress.IPv6Any;
            else
                ipaddress = IPAddress.Parse(ip);
            //var ipbytes = ipaddress.GetAddressBytes();
            //ipbytes[3] = 255;
            //ipaddress = new IPAddress(ipbytes);
            return new IPEndPoint(ipaddress, port);
        }
    }

    public static class NetworkExtension
    {


        public static TOption As<TOption>(this IChannelOption option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            if (typeof(TOption).IsAssignableFrom(option.GetType()))
            {
                return (TOption)option;
            }
            throw new ArgumentException($"{typeof(TOption)} is not form {option.GetType()}");
        }
        public static TOption As<TOption>(this IConnectorOption option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            if (typeof(TOption).IsAssignableFrom(option.GetType()))
            {
                return (TOption)option;
            }
            throw new ArgumentException($"{typeof(TOption)} is not form {option.GetType()}");
        }




        /// <summary>
        ///     将buffer通过Encoding转换成字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static unsafe string GetString(this ReadOnlySequence<byte> buffer, Encoding encoding)
        {

            if (buffer.IsSingleSegment)
            {
#if NET
                return encoding.GetString(buffer.First.Span);
#else
                fixed(byte* byteref = &MemoryMarshal.GetReference(buffer.First.Span))
                    return encoding.GetString(byteref, buffer.First.Span.Length);
#endif
            }

            if (encoding.IsSingleByte)
            {
#if NET
                return string.Create((int) buffer.Length, buffer, (span, sequence) =>
                {
                    foreach (var segment in sequence)
                    {
                         var count = encoding.GetChars(segment.Span, span);
                        span = span.Slice(count);
                    }
                });
#else

                StringBuilder temsb = new StringBuilder();
                foreach (var segment in buffer)
                {
                    fixed (byte* bytes1 = &MemoryMarshal.GetReference(segment.Span))
                    {
                        var chars = new char[encoding.GetCharCount(bytes1,segment.Span.Length)];
                        fixed (char* chars1 = &MemoryMarshal.GetReference(new Span<char>(chars)))
                        {
                            encoding.GetChars(bytes1, segment.Span.Length, chars1, chars.Length);
                            temsb.Append(chars);
                        }
                    }
                } 
                return temsb.ToString();
#endif
            }

            var sb = new StringBuilder();
            var decoder = encoding.GetDecoder();

            foreach (var piece in buffer)
            {
                var charbuffer = new char[piece.Length].AsSpan();

#if NET
                var len = decoder.GetChars(piece.Span, charbuffer, false);
                sb.Append(new string(len == charbuffer.Length ? charbuffer : charbuffer.Slice(0, len)));
#else

                fixed (byte* bytes1 = &MemoryMarshal.GetReference(piece.Span))
                fixed (char* chars1 = &MemoryMarshal.GetReference(charbuffer))
                {
                    var len = decoder.GetChars(bytes1, piece.Span.Length, chars1, charbuffer.Length, false);
                    fixed(char* chars2= &MemoryMarshal.GetReference(len == charbuffer.Length ? charbuffer : charbuffer.Slice(0, len)))
                        sb.Append(new string(chars2));
                }
#endif
            }

            return sb.ToString();
        }

        public static unsafe int Write(this IBufferWriter<byte> writer, ReadOnlySpan<char> text, Encoding encoding)
        {
            var encoder = encoding.GetEncoder();
            var completed = false;
            var totalBytes = 0;
            var minSpanSizeHint = encoding.GetMaxByteCount(1);
            while (!completed)
            {
                var span = writer.GetSpan(minSpanSizeHint);
                //encoder.Convert(text, span, false, out var charsUsed, out var bytesUsed, out completed);

                fixed (char* chars1 = &MemoryMarshal.GetReference(text))
                fixed (byte* bytes1 = &MemoryMarshal.GetReference(span))
                {
                    encoder.Convert(chars1, text.Length, bytes1, span.Length, false, out var charsUsed,
                        out var bytesUsed, out completed);


                    //this.Convert(chars1, chars.Length, bytes1, bytes.Length, flush, out charsUsed, out bytesUsed, out completed);
                    //encoder.Convert(text, text.Length, bytes1, span.Length, false, out charsUsed, out bytesUsed, out completed);
                    if (charsUsed > 0)
                        text = text.Slice(charsUsed);

                    totalBytes += bytesUsed;
                    writer.Advance(bytesUsed);
                }
            }

            return totalBytes;
        }
    }
}
