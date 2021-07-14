using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    internal class FStringStyleConvert : IValueConverter
    {
        private readonly string parameter;
        private object Value;

        public FStringStyleConvert(string parameter)
        {
            this.parameter = parameter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            if (this.parameter.ToString() == "X" || this.parameter.ToString() == "U")
                return value.ToString().ToUpper();
            if (this.parameter.ToString() == "x" || this.parameter.ToString() == "u")
                return value.ToString().ToLower();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}