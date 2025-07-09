using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DryIocEx.Core.IOC;
using DryIocEx.Prism.MVVM;

namespace DryIocEx.Prism.I18n;

[MarkupExtensionReturnType(typeof(object))]
public class I18nExtension : MarkupExtension
{
    public I18nExtension()
    {
    }

    public I18nExtension(ComponentResourceKey key)
    {
        Key = key;
    }

    [ConstructorArgument(nameof(Key))] public ComponentResourceKey Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Key == null)
            throw new NullReferenceException($"{nameof(Key)} can not null");

        return ProvideValueFromKey(serviceProvider, Key);
    }

    private object ProvideValueFromKey(IServiceProvider serviceProvider, ComponentResourceKey key)
    {
        throw new NotImplementedException();
    }
}

public class I18nSource : INotifyPropertyChanged
{
    private readonly bool _isInDesign;
    private readonly ComponentResourceKey _key;


    public I18nSource(ComponentResourceKey key, FrameworkElement fe = null)
    {
       throw new NotImplementedException();
    }

    public object Value
    {
        get
        {
            throw new NotImplementedException();
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    private void OnUnLoaded(object sender, RoutedEventArgs e)
    {
        ContainerLocator.Container.Resolve<ILanguageManager>().CurrentCultureInfoChanged -= OnLanguageChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        OnLanguageChanged();
        ContainerLocator.Container.Resolve<ILanguageManager>().CurrentCultureInfoChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }

    public static implicit operator I18nSource(ComponentResourceKey resourceKey)
    {
        return new I18nSource(resourceKey);
    }
}