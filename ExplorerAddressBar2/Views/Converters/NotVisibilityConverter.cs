using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExplorerAddressBar2.Views.Converters
{
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    class NotVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v && v == Visibility.Visible)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

    }
}
