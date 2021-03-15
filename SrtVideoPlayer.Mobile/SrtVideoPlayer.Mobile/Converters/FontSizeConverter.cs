using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Converters
{
    class FontSizeConverter : IValueConverter
    {
        private static readonly Dictionary<TargetIdiom, double> _scaleByIdiom = new Dictionary<TargetIdiom, double>
        {
            { TargetIdiom.Phone, 1.5 },
            { TargetIdiom.Tablet, 2 },
            { TargetIdiom.Desktop, 2.5 },
            { TargetIdiom.TV, 3 },
            { TargetIdiom.Watch, 1 }
        };

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            value is int intValue ?
                _scaleByIdiom.ContainsKey(Device.Idiom) ?
                    intValue * _scaleByIdiom[Device.Idiom] :
                    intValue :
                default;

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            (int)(value is double doubleValue ?
                _scaleByIdiom.ContainsKey(Device.Idiom) ?
                    doubleValue / _scaleByIdiom[Device.Idiom] :
                    doubleValue :
                default);
    }
}
