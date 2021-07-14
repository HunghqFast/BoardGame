using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputValueToIcon : IValueConverter
    {
        private readonly Func<object> Default, Value;
        private readonly object D;

        public FInputValueToIcon(object valD, Func<object> defaultValue, Func<object> value)
        {
            Default = defaultValue;
            Value = value;
            D = valD;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == D ? Default.Invoke() : Value.Invoke();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == Default.Invoke() ? Default.Invoke() : Value.Invoke();
        }
    }
}