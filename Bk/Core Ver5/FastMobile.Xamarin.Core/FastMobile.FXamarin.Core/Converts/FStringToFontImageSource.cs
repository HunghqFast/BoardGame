using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FStringToFontImageSource : IValueConverter
    {
        private Color Color;
        private double Size;
        private object Value;

        public FStringToFontImageSource(Color convertColor, double size)
        {
            Color = convertColor;
            Size = size;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            return string.IsNullOrEmpty(value?.ToString()) ? null : value.ToString().ToFontImageSource(Color, Size);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}