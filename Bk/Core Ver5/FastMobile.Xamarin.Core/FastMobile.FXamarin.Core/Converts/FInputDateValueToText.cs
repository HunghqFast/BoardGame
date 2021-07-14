using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputDateValueToText : IValueConverter
    {
        private readonly object DefaultValue;
        private readonly string Format;

        public FInputDateValueToText(object defaultValue, string format = "dd/MM/yyyy")
        {
            DefaultValue = defaultValue;
            Format = format;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime val)
                return val == default ? DefaultValue : val.ToString(Format);
            return DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == DefaultValue ? default : DateTime.ParseExact(value.ToString(), Format, null);
        }
    }
}