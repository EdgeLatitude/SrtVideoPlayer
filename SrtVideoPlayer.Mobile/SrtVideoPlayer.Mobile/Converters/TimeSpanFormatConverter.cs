using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Converters
{
    class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            value is TimeSpan timeSpanValue ?
                General.ConvertTimeSpanToShortestString(timeSpanValue) :
                LocalizedStrings.NullTime;

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture) =>
            value is string stringValue ?
                General.ConvertShortestStringToTimeSpan(stringValue) :
                null;
    }
}
