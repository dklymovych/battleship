using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Services;

public class ActiveViewModelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        Type currentViewModelType = value.GetType();
        Type targetViewModelType = Type.GetType((string)parameter);

        return currentViewModelType == targetViewModelType;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}