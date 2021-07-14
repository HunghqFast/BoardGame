using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FStringNoNullConvert : IValueConverter
    {
        private object Value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            if (value.ToString().Trim().Length < 5)
                return value + "        ";
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}