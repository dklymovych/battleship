using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Services;

public class ValueToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case 1:
                return "•";
            case 2:
                return "X";
            default:
                return "";  // Or any default value you want for other cases
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
