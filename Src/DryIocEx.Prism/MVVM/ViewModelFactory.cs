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
        var attribute = viewtype.GetCustomAttribute<ViewModelAttribute>();
        if (attribute != null)
            return attribute.ViewModelType;
        //没有特性就修改ViewModel
        var viewtypename = viewtype.FullName;
        var assemblyname = viewtype.GetTypeInfo().Assembly.FullName;
        viewtypename = viewtypename.Replace(".Views.", ".ViewModels.");
        var suffix = viewtypename.EndsWith("View") ? "Model" : "ViewModel";
        var viewmodelname = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewtypename, suffix,
            assemblyname);
        return Type.GetType(viewmodelname);
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
        var viewtype = fe.GetType();
        var viewmodeltype = _resolveViewModelHandle.Invoke(viewtype);
        var view = fe;
        var viewmodel = CreateViewModel(viewmodeltype);

        if (view != null && viewmodel != null)
        {
            if (!DesignerProperties.GetIsInDesignMode(fe))
                _relationshipHandler.Handler(view, viewmodel, ContainerLocator.Container);
            bind(view, viewmodel);
        }
    }

    private static object CreateViewModel(Type viewmodeltype)
    {
        try
        {
            return ContainerLocator.Container?.Resolve(viewmodeltype) ?? Activator.CreateInstance(viewmodeltype);
        }
        catch (Exception e)
        {
            return null;
        }
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
        if (view == null || viewmodel == null)
            throw new ArgumentNullException("view or viewmodel is null");
        _relationshipchain?.Invoke(view, viewmodel, container);
    }

    public IViewAndViewModelRelationshipHandler InheritsFrom<TView, TViewModel>(
        Action<TView, TViewModel, IContainer> configuration)
    {
        var previousAction = _relationshipchain;
        _relationshipchain = (view, viewModel, container) =>
        {
            previousAction?.Invoke(view, viewModel, container);
            if (view is TView tView && viewModel is TViewModel tViewModel) //不是继承的基类就不执行
                configuration?.Invoke(tView, tViewModel, container);
        };
        return this;
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
            Action<IDialogResult> resquestclosehandler = null;
            resquestclosehandler = o =>
            {
                view.Result = o;
                view.Close();
            };

            RoutedEventHandler loadedhandler = null;
            loadedhandler = (o, e) =>
            {
                view.Loaded -= loadedhandler;
                viewmodel.RequestClose += resquestclosehandler;
            };
            view.Loaded += loadedhandler;

            EventHandler activatedhandler = null;
            activatedhandler = (o, e) => { viewmodel.OnActivated(); };
            view.Activated += activatedhandler;
            EventHandler deactivatedhandler = null;
            deactivatedhandler = (o, e) => { viewmodel.Deactivated(); };
            view.Deactivated += deactivatedhandler;
            CancelEventHandler closinghandler = null;
            closinghandler = (o, e) =>
            {
                if (!viewmodel.CanClosed())
                    e.Cancel = true;
            };
            view.Closing += closinghandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
            {
                view.Activated -= activatedhandler;
                view.Deactivated -= deactivatedhandler;
                view.Closing -= closinghandler;
                view.Closed -= closedHandler;
                viewmodel.RequestClose -= resquestclosehandler;
                viewmodel.OnClosed();
                if (view.Result == null) view.Result = new DialogResult();
            };
            view.Closed += closedHandler;
        });
    }


    public static IViewAndViewModelRelationshipHandler ViewModelInheritsFrom<TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<FrameworkElement, TViewModel> configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException("configuration");
        return @this.InheritsFrom<FrameworkElement, TViewModel>((view, viewModel, container) =>
            configuration(view, viewModel));
    }

    public static IViewAndViewModelRelationshipHandler ViewModelInheritsFrom<TView, TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<TView, TViewModel> configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException("configuration");
        return @this.InheritsFrom<TView, TViewModel>((view, viewModel, container) => configuration(view, viewModel));
    }

    public static IViewAndViewModelRelationshipHandler OnlyViewModelInheritsFrom<TViewModel>(
        this IViewAndViewModelRelationshipHandler @this, Action<TViewModel, IContainer> configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException("configuration");
        return @this.InheritsFrom<FrameworkElement, TViewModel>((view, viewModel, container) =>
            configuration(viewModel, container));
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