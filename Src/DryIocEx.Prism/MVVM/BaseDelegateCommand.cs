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
       throw new NotImplementedException();
    }

    #endregion


    #region IActiveAware

    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set
        {
           throw new NotImplementedException();
        }
    }

    protected virtual void OnIsActiveChanged()
    {
        throw new NotImplementedException();
    }

    public event EventHandler IsActiveChanged;

    #endregion


    #region ICommand

    bool ICommand.CanExecute(object parameter)
    {
       throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void RaiseCanExecuteChanged()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public override void Execute(object parameter)
    {
        _executeMethod((T)parameter);
    }

    public override bool CanExecute(object parameter)
    {
        throw new NotImplementedException();
    }


    #region Observes

    public DelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    public DelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
       throw new NotImplementedException();
    }

    #endregion

    #region Subscribe

    public void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
    {
       throw new NotImplementedException();
    }

    private void GenerateNextNode()
    {
       throw new NotImplementedException();
    }

    #endregion
}