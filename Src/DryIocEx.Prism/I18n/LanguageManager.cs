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
            if (EqualityComparer<CultureInfo>.Default.Equals(value, CultureInfo.DefaultThreadCurrentUICulture)) return;
            CultureInfo.DefaultThreadCurrentUICulture = value;
            CultureInfo.DefaultThreadCurrentCulture = value;
            OnCurrentUICultureChanged();
        }
    }


    public event Action CurrentCultureInfoChanged;

    public void Register(ResourceManager manager)
    {
        if (_storage.ContainsKey(manager.BaseName)) return;
        _storage[manager.BaseName] = manager;
    }

    public object Get(ComponentResourceKey key)
    {
        return GetCurrentResurceManager(key.TypeInTargetAssembly.FullName)
                ?.GetObject(key.ResourceId.ToString(), CurrentCultureInfo) ?? $"Miss_{key}";
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return "Miss";
        var result = string.Empty;
        var success = false;
        foreach (var resourceManager in _storage.Values)
        {
            try
            {
                result = resourceManager.GetString(key, CurrentCultureInfo);
                if (!string.IsNullOrEmpty(result)) success = true;
            }
            catch (Exception e)
            {
                continue;
            }

            if (success) break;
        }

        return success ? result : key;
    }

    private void OnCurrentUICultureChanged()
    {
        CurrentCultureInfoChanged?.Invoke();
    }

    private ResourceManager GetCurrentResurceManager(string key)
    {
        return _storage.TryGetValue(key, out var value) ? value : null;
    }
}