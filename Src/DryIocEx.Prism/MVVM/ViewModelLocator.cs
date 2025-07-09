using System.Windows;

namespace DryIocEx.Prism.MVVM;

public static class ViewModelLocator
{
    public static DependencyProperty AutoWireViewModelProperty =
        DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator),
            new PropertyMetadata(false, OnAutoWireViewModelChanged));

    private static void OnAutoWireViewModelChanged(DependencyObject dependencyobject,
        DependencyPropertyChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     基于约定方法，必须要实现
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetAutoWireViewModel(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoWireViewModelProperty, value);
    }

    private static void Bind(object view, object viewmodel)
    {
        if (view is FrameworkElement fe) fe.DataContext = viewmodel;
    }
}