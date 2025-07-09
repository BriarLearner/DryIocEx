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
        throw new NotImplementedException();
    }

    protected virtual bool SetProperty<T>(T value, [CallerMemberName] string propertyname = null)
    {
        throw new NotImplementedException();
    }

    private bool AutoSetPropertyCore<T>(string propertyname, T value, out T oldvalue)
    {
       throw new NotImplementedException();
    }

    protected virtual bool SetProperty<T>(T value, Action<T> changedCallback,
        [CallerMemberName] string propertyname = null)
    {
        throw new NotImplementedException();
    }

    protected virtual bool SetProperty<T>(T value, Action changedCallback,
        [CallerMemberName] string propertyname = null)
    {
        throw new NotImplementedException();
    }

    protected T GetValue<T>([CallerMemberName] string propertyname = null)
    {
        throw new NotImplementedException();
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, Action onchanged,
        [CallerMemberName] string propertyname = null)
    {
       throw new NotImplementedException();
    }

    private void RaisePropertyChanged(string propertyname)
    {
        throw new NotImplementedException();
    }

    #endregion
}