using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FDoubleToGridLength : IValueConverter
    {
        private object Value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            return System.Convert.ToDouble(value is GridLength g ? g.Value : value) <= 0 ? GridLength.Auto : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}