using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client.Services;

public class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Type viewModelType = value.GetType();
        string[] targetTypes = parameter.ToString().Split(',');

        foreach (string typeName in targetTypes)
        {
            Type targetTypeToShow = Type.GetType(typeName.Trim());
            if (viewModelType == targetTypeToShow)
            {
                return Visibility.Visible;
            }
        }

        return Visibility.Collapsed;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}