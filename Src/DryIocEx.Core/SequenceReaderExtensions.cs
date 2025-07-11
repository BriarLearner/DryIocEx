﻿// <auto-generated /> // Copied from https://raw.githubusercontent.com/dotnet/runtime/cf5b231fcbea483df3b081939b422adfb6fd486a/src/libraries/System.Memory/src/System/Buffers/SequenceReaderExtensions.Binary.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

#if NETSTANDARD2_0 || NETFRAMEWORK

using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    /// <summary>
    /// Provides extended functionality for the <see cref="SequenceReader{T}"/> class that allows reading of endian specific numeric values from binary data.
    /// </summary>
    public static partial class SequenceReaderExtensions
    {
        /// <summary>
        /// Try to read the given type out of the buffer if possible. Warning: this is dangerous to use with arbitrary
        /// structs- see remarks for full details.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: The read is a straight copy of bits. If a struct depends on specific state of it's members to
        /// behave correctly this can lead to exceptions, etc. If reading endian specific integers, use the explicit
        /// overloads such as <see cref="TryReadLittleEndian(ref SequenceReader{byte}, out short)"/>
        /// </remarks>
        /// <returns>
        /// True if successful. <paramref name="value"/> will be default if failed (due to lack of space).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe bool TryRead<T>(ref this SequenceReader<byte> reader, out T value) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        private static unsafe bool TryReadMultisegment<T>(ref SequenceReader<byte> reader, out T value) where T : unmanaged
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="short"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="short"/>.</returns>
        public static bool TryReadLittleEndian(ref this SequenceReader<byte> reader, out short value)
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="short"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="short"/>.</returns>
        public static bool TryReadBigEndian(ref this SequenceReader<byte> reader, out short value)
        {
            throw new NotImplementedException();
        }

        private static bool TryReadReverseEndianness(ref SequenceReader<byte> reader, out short value)
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="int"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="int"/>.</returns>
        public static bool TryReadLittleEndian(ref this SequenceReader<byte> reader, out int value)
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="int"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="int"/>.</returns>
        public static bool TryReadBigEndian(ref this SequenceReader<byte> reader, out int value)
        {
           throw new NotImplementedException();
        }

        private static bool TryReadReverseEndianness(ref SequenceReader<byte> reader, out int value)
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="long"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public static bool TryReadLittleEndian(ref this SequenceReader<byte> reader, out long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads an <see cref="long"/> as big endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public static bool TryReadBigEndian(ref this SequenceReader<byte> reader, out long value)
        {
           throw new NotImplementedException();
        }

        private static bool TryReadReverseEndianness(ref SequenceReader<byte> reader, out long value)
        {
           throw new NotImplementedException();
        }
    }
}

#else

using System.Buffers;
using System.Runtime.CompilerServices;

#pragma warning disable RS0016
#pragma warning disable RS0041
[assembly: TypeForwardedTo(typeof(SequenceReaderExtensions))]
#pragma warning restore RS0041
#pragma warning restore RS0016

#endif
