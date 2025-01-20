using System;

namespace DryIocEx.Core.Util;

[Obsolete("推荐使用IUtilSecurity")]
public interface IUtilSafety : IUtil
{
    byte[] GetEncrypt(byte[] enBytes);
}

[Util]
public class UtilSafety : IUtilSafety
{
    public byte[] GetEncrypt(byte[] enBytes)
    {
        throw new NotImplementedException();
    }
}