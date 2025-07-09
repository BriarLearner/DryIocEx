using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace DryIocEx.Prism.I18n;

public class LanguageManager : ILanguageManager
{
    private readonly ConcurrentDictionary<string, ResourceManager> _storage = new();

    public CultureInfo CurrentCultureInfo
    {
        get => CultureInfo.DefaultThreadCurrentUICulture;
        set
        {
            throw new NotImplementedException();
        }
    }


    public event Action CurrentCultureInfoChanged;

    public void Register(ResourceManager manager)
    {
       throw new NotImplementedException();
    }

    public object Get(ComponentResourceKey key)
    {
        throw new NotImplementedException();
    }

    public string Get(string key)
    {
        throw new NotImplementedException();
    }

    private void OnCurrentUICultureChanged()
    {
        throw new NotImplementedException();
    }

    private ResourceManager GetCurrentResurceManager(string key)
    {
        throw new NotImplementedException();
    }
}