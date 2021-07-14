using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FGridUnitTypeAbsoluteConvert : IValueConverter
    {
        private object Value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            if ((int)value < 0)
                return GridLength.Auto;
            return new GridLength((int)value, GridUnitType.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}