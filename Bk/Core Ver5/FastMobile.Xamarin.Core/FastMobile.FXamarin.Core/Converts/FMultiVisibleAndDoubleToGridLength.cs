using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FMultiVisibleAndDoubleToGridLength : IMultiValueConverter
    {
        public static FMultiVisibleAndDoubleToGridLength Default { get; }
        private object[] Values;

        static FMultiVisibleAndDoubleToGridLength()
        {
            Default = new FMultiVisibleAndDoubleToGridLength();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Values = values;
            if (values[0] is bool b && values[1] is double d && values[2] is View V)
            {
                return b ? (d <= 0 ? GridLength.Auto : d) : (V.Height <= 0 ? GridLength.Auto : V.Height);
            }
            return GridLength.Auto;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Values;
        }
    }
}