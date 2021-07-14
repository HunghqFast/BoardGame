using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FColorConvert : IValueConverter
    {
        private object Value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            return Color.FromHex(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}