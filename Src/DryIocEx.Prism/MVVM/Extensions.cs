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
        while (true)
        {
            var parentobj = child.GetParentObject();
            if (parentobj == null) return null;
            if (parentobj is T parent) return parent;
            child = parentobj;
        }
    }

    public static DependencyObject GetParentObject(this DependencyObject child)
    {
        if (child == null) return null;
        if (child is ContentElement contentelement)
        {
            var parent = ContentOperations.GetParent(contentelement);
            if (parent != null) return parent;

            return contentelement is FrameworkContentElement fce ? fce.Parent : null;
        }

        //从可视化树找父节点
        var childParent = VisualTreeHelper.GetParent(child);
        if (childParent != null) return childParent;
        if (child is FrameworkElement frameworkElement)
        {
            var parent = frameworkElement.Parent;
            if (parent != null) return parent;
        }

        return null;
    }

    public static bool IsInDesign(this DependencyObject fe)
    {
        return DesignerProperties.GetIsInDesignMode(fe);
    }

    public static void SetDesignLanguage(this DependencyObject fe, Func<ComponentResourceKey, object> func)
    {
        if (fe == null || func == null) return;
        if (IsInDesign(fe) && LanguageLocator.DesignerGet == null) LanguageLocator.DesignerGet = func;
    }
}