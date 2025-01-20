using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DryIocEx.Core.Event;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;
using DryIocEx.Core.Manager;
using DryIocEx.Core.Util;

namespace DryIocEx.Prism;

public abstract class ApplicationPro : Application
{
    protected Type MainViewType;

    private IDisposable single;


    protected IContainer Container { private set; get; }

    protected override void OnStartup(StartupEventArgs e)
    {
        throw new NotImplementedException();
    }

    public abstract Type GetMainType();

    /// <summary>
    ///     设置语言
    /// </summary>
    /// <param name="container"></param>
    protected virtual void SetLocalization(IContainer container)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     设置服务
    /// </summary>
    /// <param name="container"></param>
    protected virtual void HandleService(IContainer container)
    {
    }

    /// <summary>
    ///     检测单例
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckSingleton()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     创建日志
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    protected void CreateLogManager(IContainer container)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     处理异常
    /// </summary>
    protected void HandleException()
    {
        throw new NotImplementedException();
    }

    protected void RegisterTryCatchFunction<T>(Action<T> action, T assembly)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     注册类
    /// </summary>
    /// <param name="container"></param>
    protected virtual void RegisterTypes(IContainer container)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     显示窗体
    /// </summary>
    /// <param name="container"></param>
    protected virtual void ShowWindow(IContainer container)
    {
        throw new NotImplementedException();
    }

    #region 异常

    private void App_Exit(object sender, ExitEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }

    protected void HandleException(Exception e)
    {
        throw new NotImplementedException();
    }

    protected void ShowException(Exception e)
    {
        throw new NotImplementedException();
    }

    protected virtual void CurrentDomainOnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        throw new NotImplementedException();
    }

    protected virtual void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        throw new NotImplementedException();
    }

    #endregion
}

public class ExceptionEvent : PubSubEvent<Exception>
{
}