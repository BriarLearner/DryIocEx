using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DryIocEx.Core.Util;

public interface IUtilBuffer : IUtil
{
    byte[] Convert<T>(T value, bool isbigendian = false) where T : struct;

    T Convert<T>(byte[] buffer, bool isbigendian = false) where T : struct;
    byte[] ConvertBit(bool[] value, bool isbigendian = false);
    bool[] ConvertBit<T>(T value, bool isbigendian = false) where T : struct;
}

[Util]
public class UtilBuffer : IUtilBuffer
{
    public byte[] Convert<T>(T value, bool isbigendian = false) where T : struct
    {
        throw new NotImplementedException();
    }

    public bool[] ConvertBit<T>(T value, bool isbigendian = false) where T : struct
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     转换错误,返回默认
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public T Convert<T>(byte[] buffer, bool isbigendian = false) where T : struct
    {
        throw new NotImplementedException();
    }


    public byte[] ConvertBit(bool[] value, bool isbigendian = false)
    {
        throw new NotImplementedException();
    }
}