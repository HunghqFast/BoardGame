using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public class FConvertPositionToText : IValueConverter
    {
        public static FConvertPositionToText Instance { get; }

        static FConvertPositionToText()
        {
            Instance = new FConvertPositionToText();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Position position)
                return string.Empty;
            if (position == default)
                return string.Empty;
            return $"{position.Latitude},{position.Longitude}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FUtility.ToPosition((string)value);
        }
    }
}