using System;
using System.Diagnostics;
using System.Text;

namespace DryIocEx.Core.Util;

public interface IUtilCommandLine : IUtil
{
    string ExecuteCommandLine(string command);
    string ExecuteAdminCommandLine(string command);
}

[Util]
public class CommandLineUtil : IUtilCommandLine
{
    public string ExecuteCommandLine(string command)
    {
        throw new NotImplementedException();
    }

    public string ExecuteAdminCommandLine(string command)
    {
        throw new NotImplementedException();
    }
}