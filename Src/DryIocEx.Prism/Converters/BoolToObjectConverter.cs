using System;
using System.Globalization;
using System.Windows.Data;

namespace DryIocEx.Prism.Converters;

public class BoolToObjectConverter : IValueConverter
{
    public object TrueValue { get; set; }

    public object FalseValue { get; set; }

    public object NullValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return NullValue;
        //if (value is bool?)
        //{
        //    var tem = value as bool?;
        //    return tem.Value ? TrueValue : FalseValue;
        //}
        if (!(value is bool bvalue)) return NullValue;

        return bvalue ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

//<dxmvvm:BooleanToObjectConverter x:Key="BooleanToColorConverter"> 
//<dxmvvm:BooleanToObjectConverter.TrueValue> 
//<SolidColorBrush Color = "Green" />
//</ dxmvvm:BooleanToObjectConverter.TrueValue> 
//<dxmvvm:BooleanToObjectConverter.FalseValue> 
//<SolidColorBrush Color = "Red" />
//</ dxmvvm:BooleanToObjectConverter.FalseValue> 
//</dxmvvm:BooleanToObjectConverter> 