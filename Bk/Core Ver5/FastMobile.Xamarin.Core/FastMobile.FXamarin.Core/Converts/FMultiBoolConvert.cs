using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FMultiBoolConvert : IMultiValueConverter
    {
        private object[] Values;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Values = values;
            return new List<object>(values).Find(x => x == null || x.Equals(false)) == null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Values;
        }
    }
}