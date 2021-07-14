using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FByteArrayToImageSourceConverter : IValueConverter
    {
        private object BeforeValue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BeforeValue = value;
            if (value is not byte[] bytes)
                return null;
            return FUtility.ToImageSourceFromBase64(System.Convert.ToBase64String(bytes, 0, bytes.Length));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BeforeValue;
        }
    }
}