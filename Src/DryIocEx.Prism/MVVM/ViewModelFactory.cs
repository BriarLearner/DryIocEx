using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using DryIocEx.Core.Common;
using DryIocEx.Core.IOC;
using IContainer = DryIocEx.Core.IOC.IContainer;

namespace DryIocEx.Prism.MVVM;

public static class ViewModelFactory
{
    private static Func<Type, Type> _resolveViewModelHandle = viewtype =>
    {
       throw new NotImplementedException();
    };


    private static IViewAndViewModelRelationshipHandler _relationshipHandler =
        new RelationshipHandler().BindRelationship();

    public static void SerRelationShipHandler(IViewAndViewModelRelationshipHandler handler)
    {
        _relationshipHandler = handler;
    }


    public static void SetResolverViewModelHandle(Func<Type, Type> func)
    {
        _resolveViewModelHandle = func;
    }

    public static void WireViewModel(FrameworkElement fe, Action<object, object> bind)
    {
        throw new NotImplementedException();
    }

    private static object CreateViewModel(Type viewmodeltype)
    {
        throw new NotImplementedException();
    }
}

public class ViewModelAttribute : Attribute
{
    /// <summary>
    ///     默认ViewModel生命周期Singleton
    /// </summary>
    /// <param name="servicetype"></param>
    public ViewModelAttribute(Type viewmodeltype)
    {
        ViewModelType = viewmodeltype;
    }

    public Type ViewModelType { get; }
}

public interface IViewAndViewModelRelationshipHandler
{
    void Handler(object view, object viewmodel, IContainer contaienr);

    IViewAndViewModelRelationshipHandler InheritsFrom<TView, TViewModel>(
        Action<TView, TViewModel, IContainer> action);
}

public class RelationshipHandler : IViewAndViewModelRelationshipHandler
{
    private Action<object, object, IContainer> _relationshipchain;

    public void Handler(object view, object viewmodel, IContainer container)
    {
       throw new NotImplementedException();
    }

    public IViewAndViewModelRelationshipHandler InheritsFrom<TView, TViewModel>(
        Action<TView, TViewModel, IContainer> configuration)
    {
        throw new NotImplementedException();
    }
}

public static class BindExtension
{
    public static IViewAndViewModelRelationshipHandler BindRelationship(this IViewAndViewModelRelationshipHandler @this)
    {
        return @this.ViewModelInheritsFrom<IViewModelDispatcher>((view, viewmodel) =>
            viewmodel.Dispatcher = view.Dispatcher
        ).ViewModelInheritsFrom<IViewModelLoadedAndUnloaded>((view, viewmodel) =>
        {
            RoutedEventHandler loadedhandler = null;
            loadedhandler = (o, e) =>
            {
                view.Loaded -= loadedhandler;
                viewmodel.OnLoaded();
            };
            view.Loaded += loadedhandler;
            RoutedEventHandler unloadedhandler = null;
            unloadedhandler = (o, e) =>
            {
                view.Unloaded -= unloadedhandler;
                viewmodel.OnUnloaded();
            };
            view.Unloaded += unloadedhandler;
        }).ViewModelInheritsFrom<IViewControl, ICallViewExecuted>((view, viewmodel) =>
        {
            viewmodel.CallViewHandler = view.Execute;
        }).ViewModelInheritsFrom<IViewExecuted, ICallViewExecuted>((view, viewmodel) =>
        {
            viewmodel.CallViewHandler = view.Execute;
        }).ViewModelInheritsFrom<IViewWindow, IViewModelWindow>((view, viewmodel) =>
        {
            throw new NotImplementedException();
        });
    }


    public static IViewAndViewModelRelationshipHandler ViewModelInheritsFrom<TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<FrameworkElement, TViewModel> configuration)
    {
        throw new NotImplementedException();
    }

    public static IViewAndViewModelRelationshipHandler ViewModelInheritsFrom<TView, TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<TView, TViewModel> configuration)
    {
       throw new NotImplementedException();
    }

    public static IViewAndViewModelRelationshipHandler OnlyViewModelInheritsFrom<TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<TViewModel, IContainer> configuration)
    {
       throw new NotImplementedException();
    }
}

/// <summary>
///     线程接口
/// </summary>
public interface IViewModelDispatcher
{
    Dispatcher Dispatcher { set; get; }
}

/// <summary>
///     Load和Unload接口
/// </summary>
public interface IViewModelLoadedAndUnloaded
{
    void OnLoaded();
    void OnUnloaded();
}

public interface IViewModelLoadedAndUnloaded<in TView>
{
    void OnLoaded(TView view);
    void OnUnloaded(TView view);
}

/// <summary>
///     Windows接口
/// </summary>
public interface IViewModelWindow
{
    bool CanClosed();
    void OnClosed();
    void OnActivated();
    void Deactivated();

    event Action<IDialogResult> RequestClose;
}

/// <summary>
///     windowsview接口
/// </summary>
public interface IViewWindow
{
    IDialogResult Result { set; get; }

    Window Owner { get; set; }

    object DataContext { get; set; }

    event RoutedEventHandler Loaded;

    event EventHandler Closed;

    event CancelEventHandler Closing;

    event EventHandler Activated;

    event EventHandler Deactivated;

    void Show();
    bool? ShowDialog();

    void Close();
}

/// <summary>
///     UserControl由UI主动触发调用
/// </summary>
public interface IViewModelLifeTime
{
    void OnOpening();
    void OnClosing();
}

/// <summary>
///     视图控制器
///     ViewControl 的范围比较大，想增加 IViewExecuted
/// </summary>
public interface IViewControl : IControl
{
    T GetView<T>();
}

/// <summary>
///     缩小IViewControl范围
/// </summary>
public interface IViewExecuted
{
    Operate Execute(Operate operate);
}

public interface ICallViewExecuted
{
    Func<Operate, Operate> CallViewHandler { set; get; }
}

/// <summary>
///     DialogResult 弹窗结果
/// </summary>
public interface IDialogResult
{
    object Parameter { get; }

    int Code { get; }

    ButtonResult Result { get; }
}

/// <summary>
///     对话框结果类型
/// </summary>
public enum ButtonResult : byte
{
    /// <summary>
    ///     无
    /// </summary>
    None = 0,

    /// <summary>
    ///     OK
    /// </summary>
    OK = 1,

    /// <summary>
    ///     取消
    /// </summary>
    Cancel = 2,

    /// <summary>
    ///     中断，异常中断
    /// </summary>
    Abort = 3,

    /// <summary>
    ///     重试
    /// </summary>
    Retry = 4,

    /// <summary>
    ///     忽略
    /// </summary>
    Ignore = 5,

    /// <summary>
    ///     是
    /// </summary>
    Yes = 6,

    /// <summary>
    ///     否
    /// </summary>
    No = 7
}

public class DialogResult : IDialogResult
{
    public DialogResult()
    {
    }

    public DialogResult(ButtonResult result)
    {
        Result = result;
    }

    public DialogResult(ButtonResult result, object parameter)
    {
        Result = result;
        Parameter = parameter;
    }

    public DialogResult(ButtonResult result, object parameter, int code)
    {
        Parameter = parameter;
        Result = result;
        Code = code;
    }

    public DialogResult(ButtonResult result, int code)
    {
        Result = result;
        Code = code;
    }

    public object Parameter { get; }

    public ButtonResult Result { get; } = ButtonResult.None;

    public int Code { get; }
}