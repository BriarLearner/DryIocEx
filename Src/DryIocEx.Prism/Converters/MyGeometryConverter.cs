using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DryIocEx.Prism.Converters;

[ValueConversion(typeof(string), typeof(PathGeometry))]
public class MyGeometryConverter : IValueConverter
{
    private static readonly ResourceDictionary _dict;


    static MyGeometryConverter()
    {
        _dict = new ResourceDictionary();
        _dict.Source = new Uri("pack://application:,,,/DryIocEx.Prism;component/Themes/icons.xaml",
            UriKind.RelativeOrAbsolute);
    }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var key = (string)value;
        if (!string.IsNullOrEmpty(key))
        {
            var geometry = (PathGeometry)_dict[key];
            if (geometry != null)
                return geometry;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}