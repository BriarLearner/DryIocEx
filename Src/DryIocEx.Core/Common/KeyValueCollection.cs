using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.Log;

namespace DryIocEx.Core.Common;

/// <summary>
///     不影响GC键值集合
///     内部使用WeakReference保存Value,防止值内存泄漏
/// </summary>
public class KeyValueCollection
{
    private readonly Dictionary<string, object> _data;

    public KeyValueCollection(bool isignorecase = true)
    {
        throw new NotImplementedException();
    }

    public KeyValueCollection(Dictionary<string, object> data)
    {
        _data = data;
    }

    public bool HasValue => _data.Count > 0;


    public static KeyValueCollectionBuilder New => new();

    public string DateTimeFormatter { set; get; } = "yyyy-MM-dd HH:mm:ss.fff";


    //private IFormatProvider _dateTimeFormatter=new CustomDateTimeFormatter();
    //public void SetDateTimeFormatter(IFormatProvider provider)
    //{
    //    _dateTimeFormatter = provider;
    //}

    /// <summary>
    ///     将所有vaue都转成 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, T>> GetAll<T>()
    {
        throw new NotImplementedException();
    }


    public KeyValueCollection Clone()
    {
        throw new NotImplementedException();
    }

    public bool Add(string str, object obj)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     不包含key返回false
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryGet<T>(string str, out T result)
    {
        throw new NotImplementedException();
    }


    public bool TryRemove(string str)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     不包含key返回默认值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public T Get<T>(string str)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     值类型转换，string和各种值类型强转
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    private T ConvertValue<T>(object obj)
    {
        throw new NotImplementedException();
    }
}

public class CustomDateTimeFormatter : IFormatProvider, ICustomFormatter
{
    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        throw new NotImplementedException();
    }

    public object GetFormat(Type formatType)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     使用Builder创建KeyValueCollection
///     是否由必要待考究，属于冗余设计
///     在KeyValue中暴露静态属性New
/// </summary>
public struct KeyValueCollectionBuilder
{
    private readonly KeyValueCollection _collection;

    public KeyValueCollectionBuilder()
    {
        throw new NotImplementedException();
    }

    public KeyValueCollectionBuilder(KeyValueCollection collection)
    {
        throw new NotImplementedException();
    }

    public KeyValueCollectionBuilder Add<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public KeyValueCollection Build()
    {
        throw new NotImplementedException();
    }
}