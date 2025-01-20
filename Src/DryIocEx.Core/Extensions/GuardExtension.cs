using System;
using System.Collections.Generic;
using System.Linq;

namespace DryIocEx.Core.Extensions;

/// <summary>
///     Bool Decorator
/// </summary>
/// <typeparam name="T"></typeparam>
public struct BoolMarket<T>
{
    /// <summary>
    ///     操作枚举
    /// </summary>
    public enum Operation
    {
        None,
        And,
        Or
    }

    /// <summary>
    ///     结果
    /// </summary>
    public bool Result { set; get; }

    public T Subject { set; get; }

    /// <summary>
    ///     操作
    /// </summary>
    internal Operation PendingOp { set; get; }

    public BoolMarket(bool result, T subject, Operation pendingOp)
    {
        throw new NotImplementedException();
    }

    public BoolMarket(bool result, T subject) : this(result, subject, Operation.None)
    {
    }

    public static implicit operator bool(BoolMarket<T> market)
    {
        throw new NotImplementedException();
    }


    public BoolMarket<T> And => throw new NotImplementedException();

    public BoolMarket<T> Or => throw new NotImplementedException();
}

public static class GuardExtension
{
    public static BoolMarket<TSubject> NotNull<TSubject>(this TSubject subject)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsNull<TSubject>(this TSubject subject)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<string> NotNullOrEmpty(this string str)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<string> IsNullOrEmpty(this string str)
    {
        throw new NotImplementedException();
    }


    public static BoolMarket<TSubject> HasAny<TSubject, U>(this TSubject subject, Func<TSubject, IEnumerable<U>> props)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> HasAny<TSubject, U>(
        this BoolMarket<TSubject> market, Func<TSubject, IEnumerable<U>> props)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> HasAny<TSubject>(this BoolMarket<TSubject> market)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsTrue<TSubject>(this BoolMarket<TSubject> market, Func<TSubject, bool> func)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsTrue<TSubject>(this TSubject subject, Func<TSubject, bool> func)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsFalse<TSubject>(this TSubject subject, Func<TSubject, bool> func)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsFalse<TSubject>(this BoolMarket<TSubject> market, Func<TSubject, bool> func)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsOneOf<TSubject>(this TSubject subject, params TSubject[] values)
    {
        throw new NotImplementedException();
    }

    public static BoolMarket<TSubject> IsOneOf<TSubject>(this BoolMarket<TSubject> market,
        params TSubject[] values)
    {
        throw new NotImplementedException();
    }
}