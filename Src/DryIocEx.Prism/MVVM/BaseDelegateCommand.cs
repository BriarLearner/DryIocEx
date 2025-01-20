using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace DryIocEx.Prism.MVVM;

public interface IActiveAware
{
    bool IsActive { get; set; }
    event EventHandler IsActiveChanged;
}

public abstract class BaseDelegateCommand : ICommand, IActiveAware
{
    private readonly SynchronizationContext _synchronizationContext;


    #region Ctor

    protected BaseDelegateCommand()
    {
        _synchronizationContext = SynchronizationContext.Current;
    }

    #endregion

    #region ObserableProperties

    private readonly HashSet<string> _observedPropertiesExpressions = new();

    protected internal void ObservesPropertyInternal<T>(Expression<Func<T>> propertyExpression)
    {
        if (_observedPropertiesExpressions.Contains(propertyExpression.ToString()))
            throw new ArgumentException($"{propertyExpression} is already being observed.",
                nameof(propertyExpression));

        _observedPropertiesExpressions.Add(propertyExpression.ToString());
        PropertyObserver.Observes(propertyExpression, RaiseCanExecuteChanged);
    }

    #endregion


    #region IActiveAware

    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            OnIsActiveChanged();
        }
    }

    protected virtual void OnIsActiveChanged()
    {
        IsActiveChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler IsActiveChanged;

    #endregion


    #region ICommand

    bool ICommand.CanExecute(object parameter)
    {
        return CanExecute(parameter);
    }

    void ICommand.Execute(object parameter)
    {
        Execute(parameter);
    }

    public abstract void Execute(object parameter);

    public abstract bool CanExecute(object parameter);

    public virtual event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
        var handler = CanExecuteChanged;
        if (handler != null)
        {
            if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
                _synchronizationContext.Post(o => handler.Invoke(this, EventArgs.Empty), null);
            else
                handler.Invoke(this, EventArgs.Empty);
        }
    }

    public void RaiseCanExecuteChanged()
    {
        OnCanExecuteChanged();
    }

    #endregion
}

public class DelegateCommand<T> : BaseDelegateCommand
{
    private readonly Action<T> _executeMethod;
    private Func<T, bool> _canExecuteMethod;

    public DelegateCommand(Action<T> executeMethod)
        : this(executeMethod, o => true)
    {
    }

    public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
    {
        if (executeMethod == null || canExecuteMethod == null)
            throw new ArgumentNullException(nameof(executeMethod), "executeMethod 和 canExecuteMethod 委托都不能为空。");

        var genericTypeInfo = typeof(T).GetTypeInfo();


        if (genericTypeInfo.IsValueType)
            if (!genericTypeInfo.IsGenericType || !typeof(Nullable<>).GetTypeInfo()
                    .IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                throw new InvalidCastException("T代表DelegateCommand<T> 不是对象也不是 Nullable。");

        _executeMethod = executeMethod;
        _canExecuteMethod = canExecuteMethod;
    }

    public override void Execute(object parameter)
    {
        _executeMethod((T)parameter);
    }

    public override bool CanExecute(object parameter)
    {
        return _canExecuteMethod((T)parameter);
    }


    #region Observes

    public DelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    public DelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        var expression =
            Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
        _canExecuteMethod = expression.Compile();
        ObservesPropertyInternal(canExecuteExpression);
        return this;
    }

    #endregion
}

public class DelegateCommand : BaseDelegateCommand
{
    private readonly Action _executeMethod;
    private Func<bool> _canExecuteMethod;

    public DelegateCommand(Action executeMethod)
        : this(executeMethod, () => true)
    {
    }

    public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
    {
        if (executeMethod == null || canExecuteMethod == null)
            throw new ArgumentNullException(nameof(executeMethod), "executeMethod 和 canExecuteMethod 委托都不能为空。");

        _executeMethod = executeMethod;
        _canExecuteMethod = canExecuteMethod;
    }

    public override void Execute(object parameter = null)
    {
        _executeMethod();
    }


    public override bool CanExecute(object parameter = null)
    {
        return _canExecuteMethod();
    }


    #region Observes

    public DelegateCommand ObservesProperty<T>(Expression<Func<T>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    public DelegateCommand ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        _canExecuteMethod = canExecuteExpression.Compile();
        ObservesPropertyInternal(canExecuteExpression);
        return this;
    }

    #endregion
}

internal class PropertyObserver
{
    private readonly Action _action;

    private PropertyObserver(Expression propertyExpression, Action action)
    {
        _action = action;
        SubscribeListeners(propertyExpression);
    }

    private void SubscribeListeners(Expression propertyExpression)
    {
        var propNameStack = new Stack<PropertyInfo>();
        while (propertyExpression is MemberExpression temp) // Gets the root of the property chain.
        {
            propertyExpression = temp.Expression;
            propNameStack.Push(temp.Member as PropertyInfo); // Records the member info as property info
        }

        if (!(propertyExpression is ConstantExpression constantExpression))
            throw new NotSupportedException("表达式类型不支持，目前只支持MemberExpression和ConstantExpression");

        var propObserverNodeRoot = new PropertyObserverNode(propNameStack.Pop(), _action); //根节点
        var previousNode = propObserverNodeRoot;
        foreach (var propName in propNameStack) //创建与属性链对应的节点链。
        {
            var currentNode = new PropertyObserverNode(propName, _action);
            previousNode.Next = currentNode;
            previousNode = currentNode;
        }

        var propOwnerObject = constantExpression.Value;

        if (!(propOwnerObject is INotifyPropertyChanged inpcObject))
            throw new InvalidOperationException("尝试订阅该属性 " +
                                                $" '{propObserverNodeRoot.PropertyInfo.Name}' , 但是该属性没有实现 INotifyPropertyChanged接口");

        propObserverNodeRoot.SubscribeListenerFor(inpcObject);
    }


    internal static PropertyObserver Observes<T>(Expression<Func<T>> propertyExpression, Action action)
    {
        return new PropertyObserver(propertyExpression.Body, action);
    }
}

internal class PropertyObserverNode
{
    private readonly Action _action;
    private INotifyPropertyChanged _inpcObject;

    public PropertyObserverNode(PropertyInfo propertyInfo, Action action)
    {
        PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        _action = () =>
        {
            action?.Invoke();
            if (Next == null) return;
            Next.UnsubscribeListener(); //确保只调用一次
            GenerateNextNode(); //重新订阅
        };
    }

    public PropertyInfo PropertyInfo { get; }
    public PropertyObserverNode Next { get; set; }


    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e?.PropertyName == PropertyInfo.Name || e?.PropertyName == null) _action?.Invoke();
    }

    #region Unsubscribe

    private void UnsubscribeListener()
    {
        if (_inpcObject != null)
            _inpcObject.PropertyChanged -= OnPropertyChanged;

        Next?.UnsubscribeListener(); //确保只调用一次
    }

    #endregion

    #region Subscribe

    public void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
    {
        _inpcObject = inpcObject;
        _inpcObject.PropertyChanged += OnPropertyChanged; //订阅属性改变事件

        if (Next != null) GenerateNextNode(); //子节点订阅
    }

    private void GenerateNextNode()
    {
        var nextProperty = PropertyInfo.GetValue(_inpcObject);
        if (nextProperty == null) return;
        if (!(nextProperty is INotifyPropertyChanged nextInpcObject))
            throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                $"owns '{Next.PropertyInfo.Name}' property, but the object does not implements INotifyPropertyChanged.");

        Next.SubscribeListenerFor(nextInpcObject);
    }

    #endregion
}