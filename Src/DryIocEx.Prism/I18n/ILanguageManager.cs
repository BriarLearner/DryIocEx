using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using DryIocEx.Core.IOC;

namespace DryIocEx.Prism.I18n;

public interface ILanguageManager
{
    CultureInfo CurrentCultureInfo { get; set; }

    event Action CurrentCultureInfoChanged;
    void Register(ResourceManager manager);


    object Get(ComponentResourceKey key);

    string Get(string key);
}

public static class LanguageExtension
{
    public static string Get(this ComponentResourceKey key)
    {
        return (string)LanguageLocator.Language.Get(key);
    }

    public static string Get(this ILanguageManager manager, ComponentResourceKey key)
    {
        return (string)manager.Get(key);
    }

    public static string GetI18n(this string str)
    {
        return LanguageLocator.Language.Get(str);
    }
}

public static class LanguageLocator
{
    private static ILanguageManager _language;

    public static ILanguageManager Language
    {
        get
        {
            if (_language != null) return _language;
            _language = ContainerLocator.Container.Resolve<ILanguageManager>();
            return _language;
        }
    }

    public static Func<ComponentResourceKey, object> DesignerGet { get; set; }


    public static void SetLanguageManager(ILanguageManager manager)
    {
        _language = manager;
    }
}