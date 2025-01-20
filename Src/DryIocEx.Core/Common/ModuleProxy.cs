using System;
using System.Collections.Generic;

namespace DryIocEx.Core.Common;

public class ModuleProxy
{
    internal Dictionary<Type, Stack<ModuleInfo>> _modules = new();

    public TModule GetModule<TModule>()
    {
        throw new NotImplementedException();
    }

    public List<TModule> GetModules<TModule>()
    {
        throw new NotImplementedException();
    }

    public void AddModule<TModule>(Func<TModule> func)
    {
        throw new NotImplementedException();
    }

    public void AddModule<TModule>(TModule obj)
    {
        throw new NotImplementedException();
    }
}

public class BaseModule
{
    internal Dictionary<Type, ModuleInfo> _modules = new();

    protected TModule GetModule<TModule>()
    {
        throw new NotImplementedException();
    }

    public void AddModule<TModule>(Func<TModule> func)
    {
        throw new NotImplementedException();
    }

    public void AddModule<TModule>(TModule obj)
    {
        throw new NotImplementedException();
    }
}

internal class ModuleInfo
{
    private object _instance;

    public ModuleInfo(object instance)
    {
        throw new NotImplementedException();
    }

    public Func<object> Factory { get; }

    public object Instance
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}