using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputHourValueToIcon : IValueConverter
    {
        private readonly Func<object> Default, Value;

        public FInputHourValueToIcon(Func<object> defaultValue, Func<object> value)
        {
            Default = defaultValue;
            Value = value;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == FInputHour.BaseValue ? Default.Invoke() : Value.Invoke();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == FInputHour.BaseValue ? Default.Invoke() : Value.Invoke();
        }
    }
}