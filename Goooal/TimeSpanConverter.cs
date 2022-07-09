using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Goooal
{
    class TimeSpanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan v = (TimeSpan)value;
            if (v > TimeSpan.FromMinutes(1))
            {
                return v.Minutes.ToString("00") + ":" + v.Seconds.ToString("00");
            }
            else
            {
                return v.Minutes.ToString("00") + ":" + v.Seconds.ToString("00") + ":" + (v.Milliseconds / 100).ToString("00");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    class TimeSpanConverter_1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan v = (TimeSpan)value;
            return v.Seconds.ToString("#0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            if (v) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility r = (Visibility)value;
            if (r == Visibility.Hidden) return false;
            else return true;
        }
    }
}
