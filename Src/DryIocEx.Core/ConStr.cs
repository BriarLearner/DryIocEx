namespace DryIocEx.Core;

public class SGConStr
{
    public const string PFC = "PFC";
    public const string Operate = "Operate";
    public const string Success = "Success";
    public const string Message = "Message";
    public const string LogInfo = "LogInfo";
    public const string LogName = "LogName";
    public const string GlobalFileLog = "GlobalFileLog";
    public const string CreateTime = "CreateTime";
}

/// <summary>
///     10000以下内核使用
/// </summary>
public class SGPFC
{
    public const int Null = 0;
    public const int BroadcastLog = 1;
    public const int Log = 2;
}