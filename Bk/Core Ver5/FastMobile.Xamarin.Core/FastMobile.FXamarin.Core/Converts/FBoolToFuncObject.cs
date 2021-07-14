using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBoolToFuncObject : IValueConverter
    {
        private readonly Func<object> A, B;

        public FBoolToFuncObject(Func<object> trueObj, Func<object> falseObj)
        {
            A = trueObj;
            B = falseObj;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? A.Invoke() : B.Invoke();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == A.Invoke();
        }
    }
}