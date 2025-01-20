using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuddenGale.Core.IOCPNetwork
{
    public  interface IPackageRamp<TPackage>
    {
        TPackage Filter(ReadOnlySequence<byte> sequence, out long consumed);

        ValueTask WriteAsync(IBufferWriter<Byte> writer, TPackage package);
    }

    public interface IStringPackage
    {
        public string Content { get; set; }
    }

    public class StringPackage: IStringPackage
    {
        public string Content {  get; set; }

       
    }

    public class StringPackageRamp<TPackage> : IPackageRamp<TPackage> where TPackage : IStringPackage,new()
    {
        public TPackage Filter(ReadOnlySequence<byte> sequence, out long consumed)
        {
            consumed = sequence.Length;
            var package = new TPackage();
            package.Content = sequence.GetString(Encoding.UTF8);
            return package;
        }

        public ValueTask WriteAsync(IBufferWriter<byte> writer, TPackage package)
        {
            writer.Write(package.Content.AsSpan(),Encoding.UTF8);
            return new ValueTask();
        }
    }

    public class TerminatorPackageFilter<TPackage> : IPackageRamp<TPackage>
    {
        private ReadOnlyMemory<byte> _terminator;

        public TerminatorPackageFilter(ReadOnlyMemory<byte> terminator)
        {
            _terminator = terminator;
        }
        public TPackage Filter(ReadOnlySequence<byte> sequence, out long consumed)
        {
            throw new NotImplementedException();
            consumed = 0;
#if NET
            var reader = new SequenceReader<byte>(sequence);
            if(!reader.TryReadTo(out sequence, _terminator.Span, false))
            {
                return default;
            }
            try
            {

            }
            finally
            {
                reader.Advance(_terminator.Length);
            }
#else





#endif
        }

        public ValueTask WriteAsync(IBufferWriter<byte> writer, TPackage package)
        {

            


            throw new NotSupportedException();
            //writer.Write(package.Content.AsSpan(), Encoding.UTF8);
            return new ValueTask();
        }
    }


}
