using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DryIocEx.Prism.MVVM;

public abstract class BaseBindable : INotifyPropertyChanged
{
    #region NotifyProperty

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private Dictionary<string, object> _propertyBag;
    private Dictionary<string, object> PropertyBag => _propertyBag ?? (_propertyBag = new Dictionary<string, object>());

    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyname = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        RaisePropertyChanged(propertyname);
        return true;
    }

    protected virtual bool SetProperty<T>(T value, [CallerMemberName] string propertyname = null)
    {
        return AutoSetPropertyCore(propertyname, value, out var oldvalue);
    }

    private bool AutoSetPropertyCore<T>(string propertyname, T value, out T oldvalue)
    {
        object obj;
        oldvalue = default;
        if (PropertyBag.TryGetValue(propertyname, out obj))
        {
            oldvalue = (T)obj;
            if (EqualityComparer<T>.Default.Equals(oldvalue, value))
                return false;
        }

        var propertybag = _propertyBag;
        var lockToken = false;
        try
        {
            Monitor.Enter(propertybag, ref lockToken);
            _propertyBag[propertyname] = value;
        }
        finally
        {
            if (lockToken)
                Monitor.Exit(propertybag);
        }

        RaisePropertyChanged(propertyname);
        return true;
    }

    protected virtual bool SetProperty<T>(T value, Action<T> changedCallback,
        [CallerMemberName] string propertyname = null)
    {
        T oldvalue = default;
        var flag = AutoSetPropertyCore(propertyname, value, out oldvalue);
        if (flag && changedCallback != null)
            changedCallback(oldvalue);
        return flag;
    }

    protected virtual bool SetProperty<T>(T value, Action changedCallback,
        [CallerMemberName] string propertyname = null)
    {
        T oldvalue = default;
        var flag = AutoSetPropertyCore(propertyname, value, out oldvalue);
        if (flag && changedCallback != null)
            changedCallback();
        return flag;
    }

    protected T GetValue<T>([CallerMemberName] string propertyname = null)
    {
        if (string.IsNullOrEmpty(propertyname)) throw new ArgumentNullException(nameof(propertyname));
        object obj;
        return PropertyBag.TryGetValue(propertyname, out obj) ? (T)obj : default;
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, Action onchanged,
        [CallerMemberName] string propertyname = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        if (onchanged != null) onchanged();
        RaisePropertyChanged(propertyname);
        return true;
    }

    private void RaisePropertyChanged(string propertyname)
    {
        OnPropertyChanged(propertyname);
    }

    #endregion
}