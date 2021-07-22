using System;
using System.Globalization;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Converters
{
    internal class HexColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            value is string stringValue ?
                Color.FromHex(stringValue) :
                Color.Transparent;

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is Color colorValue))
                return null;
            var alpha = (int)(colorValue.A * 255);
            var red = (int)(colorValue.R * 255);
            var green = (int)(colorValue.G * 255);
            var blue = (int)(colorValue.B * 255);
            return $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
        }
    }
}
