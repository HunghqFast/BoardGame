using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTextStyle : FLayoutStyle, IFTextStyle
    {
        public string Text
        {
            get => (string)GetValue(IFTextStyle.TextProperty);
            set => SetValue(IFTextStyle.TextProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(IFTextStyle.FontFamilyProperty);
            set => SetValue(IFTextStyle.FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(IFTextStyle.FontSizeProperty);
            set => SetValue(IFTextStyle.FontSizeProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(IFTextStyle.TextColorProperty);
            set => SetValue(IFTextStyle.TextColorProperty, value);
        }

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(IFTextStyle.FontAttributesProperty);
            set => SetValue(IFTextStyle.FontAttributesProperty, value);
        }

        public TextAlignment Horizontal
        {
            get => (TextAlignment)GetValue(IFTextStyle.HorizontalProperty);
            set => SetValue(IFTextStyle.HorizontalProperty, value);
        }

        public TextAlignment Vertical
        {
            get => (TextAlignment)GetValue(IFTextStyle.VerticalProperty);
            set => SetValue(IFTextStyle.VerticalProperty, value);
        }
    }
}