using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Converters;

namespace CoinPulse.UI;

public class WidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double width)
        {
            double substract = 0;
            if (parameter != null && double.TryParse(parameter.ToString(), out double paramVal))
            {
                substract = paramVal;
            }
            
            return Math.Max(0, width - substract - 40);
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}