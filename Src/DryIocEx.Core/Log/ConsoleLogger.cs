using System;
using System.Text;
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;

namespace DryIocEx.Core.Log;

public class ConsoleLogOption
{
    public EnumLogDegree Degree { set; get; }
    public ConsoleColor FatalColor { set; get; }
    public ConsoleColor ErrorColor { set; get; }
    public ConsoleColor WarnColor { set; get; }
    public ConsoleColor InfoColor { set; get; }

    public ConsoleColor DebugColor { set; get; }
}

public class ConsoleLogBuilder : BaseBuilder<ConsoleLogBuilder, ConsoleLogger>
{
    public ConsoleLogBuilder()
    {
        throw new NotImplementedException();
    }


    public ConsoleLogBuilder SetLogDegree(EnumLogDegree degree)
    {
        throw new NotImplementedException();
    }

    public ConsoleLogBuilder SetConsoleColor(EnumLogDegree degree, ConsoleColor color)
    {
        throw new NotImplementedException();
    }

    public ConsoleLogBuilder SetLogInfoFormatter(ILogInfoFormater<string> formater)
    {
        throw new NotImplementedException();
    }


    public ConsoleLogBuilder SetDefaultOption()
    {
        throw new NotImplementedException();
    }
}

public interface IConsoleLogger : ILogger
{
}

public class ConsoleLogger : BaseLogger, IConsoleLogger
{
    public ConsoleLogOption Option { get; } = new();

    public ILogInfoFormater<string> LogInfoFormatter { set; get; }


    protected override void ProcessLogInfo(LogInfo loginfo)
    {
        throw new NotImplementedException();
    }
}

public class LogInfoConsoleFormatter : ILogInfoFormater<string>
{
    public string FormatInfo(LogInfo info)
    {
        throw new NotImplementedException();
    }

    public LogInfo UnformatInfo(string data)
    {
        throw new NotImplementedException();
    }
}