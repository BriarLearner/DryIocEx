using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;

/* 项目“DryIocEx.CorePro (netstandard2.1)”的未合并的更改
在此之前:
using DryIocEx.CorePro.Common;
在此之后:
using DryIocEx.CorePro.Common;
using DryIocEx;
using DryIocEx.CorePro;
using DryIocEx.CorePro.Log;
using DryIocEx.CorePro.Imp;
*/

namespace DryIocEx.Core.Log;

public enum EnumFileLogType
{
    /// <summary>
    ///     1个文件
    /// </summary>
    One,

    /// <summary>
    ///     每天一个文件
    /// </summary>
    Day,

    /// <summary>
    ///     每周一个文件
    /// </summary>
    Week,

    /// <summary>
    ///     每月一个文件
    /// </summary>
    Month,

    /// <summary>
    ///     每个季度
    /// </summary>
    Season,

    /// <summary>
    ///     固定大小文件
    /// </summary>
    Size
}

public class FileLogOption
{
    public string FileFolder { set; get; }

    public EnumFileLogType LogType { set; get; }


    public int FileMaxSize { set; get; }

    public string Header { set; get; }

    public EnumLogDegree Degree { set; get; }

    /// <summary>
    ///     文件名缓存，不需要每次计算使用
    /// </summary>
    public string FullFileName { set; get; }
}

public interface ILoggerFile : ILogger
{
}

public class FileLogger : BaseLogger, ILoggerFile
{
    private readonly Dictionary<EnumFileLogType, Func<FileLogOption, string>> _filenamedict =
        new()
        {
            {
                EnumFileLogType.One, o =>
                {
                    if (o.FullFileName.IsNullOrEmpty())
                    {
                        var logname = $"{o.Header}_All.txt";
                        o.FullFileName = Path.Combine(o.FileFolder, logname);
                    }

                    return o.FullFileName;
                }
            },
            {
                EnumFileLogType.Day, o =>
                {
                    var logname = $"{o.Header}_{DateTime.Now:yyyy_MM_dd}.txt";
                    return Path.Combine(o.FileFolder, logname);
                }
            },
            {
                EnumFileLogType.Week, o =>
                {
                    var gc = new GregorianCalendar();
                    var week = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    var logname = $"{o.Header}_{DateTime.Now.Year}_W{week:D2}.txt";
                    return Path.Combine(o.FileFolder, logname);
                }
            },
            {
                EnumFileLogType.Month, o =>
                {
                    var logname = $"{o.Header}_{DateTime.Now:yyyy_MM}.txt";
                    return Path.Combine(o.FileFolder, logname);
                }
            },
            {
                EnumFileLogType.Season, o =>
                {
                    var season = DateTime.Now.Month / 3 + 1;
                    var logname = $"{o.Header}_{DateTime.Now.Year}_Q{season}.txt";
                    return Path.Combine(o.FileFolder, logname);
                }
            },
            {
                EnumFileLogType.Size, o =>
                {
                    var result = string.Empty;
                    IEnumerable<string> logs = Directory.GetFiles(o.FileFolder, $"{o.Header}*.txt");
                    if (logs.NotNull().And.HasAny(s => s))
                        foreach (var log in logs)
                        {
                            var file = new FileInfo(log);
                            if (file.Length < o.FileMaxSize)
                            {
                                result = log;
                                break;
                            }
                        }

                    if (result.IsNullOrEmpty())
                        result = Path.Combine(o.FileFolder, $"{o.Header}_S{logs.Count() + 1:D2}.txt");
                    return result;
                }
            }
        };

    public FileLogOption Option { get; } = new();


    public ILogInfoFormater<string> LogInfoFormatter { set; get; }

    #region Template Pattern

    protected override void StopProcessLogInfo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     单线程执行模型最好不要异步
    /// </summary>
    //private TaskCompletionSource<bool> _tasksource;
    private StreamWriter _writer;

    protected override void StartProcessLogInfo()
    {
        throw new NotImplementedException();
    }

    protected override void ProcessLogInfo(LogInfo loginfo)
    {
        throw new NotImplementedException();
    }

    #endregion
}

public interface IBuilderFileLogger
{
}

public class FileLogBuilder : BaseBuilder<FileLogBuilder, FileLogger>, IBuilderFileLogger
{
    public FileLogBuilder()
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetLogFolder(string folder)
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetLogType(EnumFileLogType type)
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetLogHeader(string header)
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetLogDegree(EnumLogDegree degree)
    {
        throw new NotImplementedException();
    }


    public FileLogBuilder SetLogSize(int filesize)
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetLogInfoFormater(ILogInfoFormater<string> formater)
    {
        throw new NotImplementedException();
    }


    public FileLogBuilder SetDefaultOption()
    {
        throw new NotImplementedException();
    }

    public FileLogBuilder SetOption(Action<FileLogOption> action)
    {
        throw new NotImplementedException();
    }
}

public class LogInfoFileFormatter : ILogInfoFormater<string>
{
    public string FormatInfo(LogInfo info)
    {
        throw new NotImplementedException();
    }

    public LogInfo UnformatInfo(string message)
    {
        throw new NotImplementedException();
    }
}