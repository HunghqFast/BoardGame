using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputDateValueToIcon : IValueConverter
    {
        private readonly Func<object> Default, Value;

        public FInputDateValueToIcon(Func<object> defaultValue, Func<object> value)
        {
            Default = defaultValue;
            Value = value;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime val && val == default ? Default.Invoke() : Value.Invoke();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == Default.Invoke() ? value : Value.Invoke();
        }
    }
}