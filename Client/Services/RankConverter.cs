using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Services;

public class RankConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index + 1; // Add 1 to start ranking from 1
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}