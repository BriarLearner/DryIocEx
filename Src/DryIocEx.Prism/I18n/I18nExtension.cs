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
        if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget providerValueTarget))
            throw new ArgumentException(
                $"the {serviceProvider} not implement {nameof(IProvideValueTarget)} interface");
        if (providerValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;

        var fe = providerValueTarget.TargetObject is DependencyObject dependencyObject
            ? dependencyObject as FrameworkElement ?? dependencyObject.TryFindParent<FrameworkElement>()
            : null;


        return new Binding(nameof(I18nSource.Value))
        {
            Source = new I18nSource(key, fe),
            Mode = BindingMode.OneWay
        }.ProvideValue(serviceProvider);
    }
}

public class I18nSource : INotifyPropertyChanged
{
    private readonly bool _isInDesign;
    private readonly ComponentResourceKey _key;


    public I18nSource(ComponentResourceKey key, FrameworkElement fe = null)
    {
        _key = key;
        if (fe != null)
        {
            _isInDesign = DesignerProperties.GetIsInDesignMode(fe);
            if (!_isInDesign)
            {
                fe.Loaded += OnLoaded;
                fe.Unloaded += OnUnLoaded;
            }
        }
    }

    public object Value
    {
        get
        {
            if (_isInDesign)
                return LanguageLocator.DesignerGet?.Invoke(_key) ?? "MISS";
            //return ResourceManagerProxy.NGet(_key);
            return ContainerLocator.Container.Resolve<ILanguageManager>().Get(_key);
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