using System;
using System.Globalization;

namespace SrtVideoPlayer.Mobile.Converters
{
    class NegateBooleanConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            !(bool)value;

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            !(bool)value;
    }
}
