using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Services;

public class BackgroundColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isActive = false;
        var currentViewModelType = value.GetType();
        var targetViewModelType = Type.GetType((string)parameter);

        if (currentViewModelType == targetViewModelType)
            isActive = true;

        return isActive ? new SolidColorBrush(Color.FromRgb(55, 55, 55)) : new SolidColorBrush(Color.FromRgb(105, 105, 105));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}