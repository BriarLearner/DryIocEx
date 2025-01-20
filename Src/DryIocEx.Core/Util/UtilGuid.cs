using System;

namespace DryIocEx.Core.Util;

public interface IUtilGuid : IUtil
{
    string GetUniqueGuid();
}

[Util]
public class GuidUtil : IUtilGuid
{
    public string GetUniqueGuid()
    {
        throw new NotImplementedException();
    }
}