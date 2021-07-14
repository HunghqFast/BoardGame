using System;
using System.Globalization;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FConvertEntryTextToBackground : IValueConverter
    {
        private readonly Entry current;
        private readonly Color Same, Dif;
        private object Value;

        public FConvertEntryTextToBackground(Entry entry, Color same, Color dif)
        {
            current = entry;
            Same = same;
            Dif = dif;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            if (value == null)
                return current.Text == null ? Same : Dif;
            return current.Text.Equals(value) ? Same : Dif;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}