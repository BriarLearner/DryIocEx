using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DryIocEx.Core.Common;

internal class CExecutor : IEquatable<CExecutor>
{
    public CExecutor(string name, string description, Action action)
    {
        throw new NotImplementedException();
    }

    public string Name { set; get; }
    public string Description { set; get; }

    public Action Action { set; get; }

    public bool Equals(CExecutor other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(CExecutor left, CExecutor right)
    {
        throw new NotImplementedException();
    }

    public static bool operator !=(CExecutor left, CExecutor right)
    {
        throw new NotImplementedException();
    }
}

public class ConsoleManager
{
    private readonly Dictionary<string, CExecutor> executors = new(StringComparer.OrdinalIgnoreCase);

    private readonly string defaultdescription = $@"
Version    :    {Assembly.GetExecutingAssembly().GetName().Version.ToString()}
";

    public ConsoleManager(string description)
    {
        throw new NotImplementedException();
    }


    public static bool IsInConsole => Environment.UserInteractive;

    public void Add(string key, string descrption, Action action)
    {
        throw new NotImplementedException();
    }

    public void RunCommandLine()
    {
        throw new NotImplementedException();
    }

    public bool Run(string order)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<Exception> ActionExeception;

    public void ShowHelper()
    {
        throw new NotImplementedException();
    }
}