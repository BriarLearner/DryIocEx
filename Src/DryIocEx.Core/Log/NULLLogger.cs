using DryIocEx.Core.Common;

namespace DryIocEx.Core.Log;
/// <summary>
/// 日志空
/// </summary>
public interface ILoggerNULL : ILogger
{
}
/// <summary>
/// 空日志
/// </summary>
public class NullLogger : BaseLogger, ILoggerNULL
{
    protected override void ProcessLogInfo(LogInfo loginfo)
    {
    }
}

public class NullLogBuilder : BaseBuilder<NullLogBuilder, NullLogger>
{
    //所有的空logger都用同一个实例
    private static readonly NullLogger logger = new();

    public override NullLogger Build()
    {
        return logger;
    }
}