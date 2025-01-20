using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DryIocEx.Core.Util;

public interface IUtilProcess : IUtil
{
    IEnumerable<ProcessInfo> GetAllProcess();
}

[Util]
public class UtilProcess : IUtilProcess
{
    public IEnumerable<ProcessInfo> GetAllProcess()
    {
        throw new NotImplementedException();
    }
}

public class ProcessInfo
{
    public int Id { set; get; }

    public string Name { set; get; }
    public string FileName { set; get; }
}