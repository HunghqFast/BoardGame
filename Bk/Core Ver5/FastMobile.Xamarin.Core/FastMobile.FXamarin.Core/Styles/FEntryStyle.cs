using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FEntryStyle : FTextStyle, IFEntryStyle
    {
        public string HintText
        {
            get => (string)GetValue(IFEntryStyle.HintTextProperty);
            set => SetValue(IFEntryStyle.HintTextProperty, value);
        }

        public string HintFontFamily
        {
            get => (string)GetValue(IFEntryStyle.HintFontFamilyProperty);
            set => SetValue(IFEntryStyle.HintFontFamilyProperty, value);
        }

        public double HintFontSize
        {
            get => (double)GetValue(IFEntryStyle.HintFontSizeProperty);
            set => SetValue(IFEntryStyle.HintFontSizeProperty, value);
        }

        public FontAttributes HintFontAttributes
        {
            get => (FontAttributes)GetValue(IFEntryStyle.HintFontAttributesProperty);
            set => SetValue(IFEntryStyle.HintFontAttributesProperty, value);
        }

        public Color FocusColor
        {
            get => (Color)GetValue(IFEntryStyle.FocusColorProperty);
            set => SetValue(IFEntryStyle.FocusColorProperty, value);
        }

        public Color UnFocusColor
        {
            get => (Color)GetValue(IFEntryStyle.UnFocusColorProperty);
            set => SetValue(IFEntryStyle.UnFocusColorProperty, value);
        }
    }
}