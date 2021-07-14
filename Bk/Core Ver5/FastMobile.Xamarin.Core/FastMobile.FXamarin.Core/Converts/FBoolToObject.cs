using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBoolToObject : IValueConverter
    {
        private readonly object A, B;

        public FBoolToObject(object trueObj, object falseObj)
        {
            A = trueObj;
            B = falseObj;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? A : B;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == A;
        }
    }
}