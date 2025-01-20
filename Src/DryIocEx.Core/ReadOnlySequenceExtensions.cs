// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Buffers;

/// <summary>
///     Extension methods for the <see cref="ReadOnlySequence{T}" /> type.
/// </summary>
public static class ReadOnlySequenceExtensions
{
    /// <summary>
    ///     Polyfill method used by the <see cref="SequenceReader{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of element kept by the sequence.</typeparam>
    /// <param name="sequence">The sequence to retrieve.</param>
    /// <param name="first">The first span in the sequence.</param>
    /// <param name="next">The next position.</param>
    internal static void GetFirstSpan<T>(this ReadOnlySequence<T> sequence, out ReadOnlySpan<T> first,
        out SequencePosition next)
    {
        throw new NotImplementedException();
    }
}