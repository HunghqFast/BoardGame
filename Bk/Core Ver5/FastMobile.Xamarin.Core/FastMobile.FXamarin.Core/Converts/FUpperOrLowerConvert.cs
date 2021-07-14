using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FUpperOrLowerConvert : IValueConverter
    {
        private readonly bool iU;
        private string previous;

        public FUpperOrLowerConvert(bool isUpper)
        {
            iU = isUpper;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            previous = value?.ToString();
            return iU ? previous?.ToUpper() : previous?.ToLower();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return previous;
        }
    }
}