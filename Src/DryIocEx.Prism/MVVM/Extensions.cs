using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DryIocEx.Prism.I18n;

namespace DryIocEx.Prism.MVVM;

public static class Extensions
{
    public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
    {
        throw new NotImplementedException();
    }

    public static DependencyObject GetParentObject(this DependencyObject child)
    {
        throw new NotImplementedException();
    }

    public static bool IsInDesign(this DependencyObject fe)
    {
        return DesignerProperties.GetIsInDesignMode(fe);
    }

    public static void SetDesignLanguage(this DependencyObject fe, Func<ComponentResourceKey, object> func)
    {
       throw new NotImplementedException();
    }
}