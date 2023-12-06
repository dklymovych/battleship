using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Services;

public class BooleanToPrivacyTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Private" : "Public";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
