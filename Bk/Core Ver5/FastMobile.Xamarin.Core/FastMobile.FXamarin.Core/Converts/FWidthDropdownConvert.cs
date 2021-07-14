using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FWidthDropdownConvert : IValueConverter
    {
        private object Value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            return System.Convert.ToInt32(value) - 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}