using DryIocEx.Core.Common;

namespace DryIocEx.Core.Log;

public interface ILoggerNULL : ILogger
{
}

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