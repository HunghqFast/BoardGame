using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    internal class FHiddenFilterExpanderConvert : IValueConverter
    {
        private readonly IFPageFilter form;
        private object Value;

        public FHiddenFilterExpanderConvert(IFPageFilter form)
        {
            this.form = form;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Value = value;
            var exp = value.ToString();
            if (string.IsNullOrEmpty(exp)) return true;
            exp = Regex.Replace(exp, @"\[(.+?)\]", m =>
            {
                return form.Input.TryGetValue(FFunc.ReplaceBinding(m.ToString()), out FInput input) ? input.GetInput(0).ToString() : m.ToString();
            });
            var result = FFunc.Compute(form, exp);
            return result == null || !FFunc.StringToBoolean(result.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Value;
        }
    }
}