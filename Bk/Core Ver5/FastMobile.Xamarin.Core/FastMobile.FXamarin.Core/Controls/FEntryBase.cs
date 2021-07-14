using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FEntryBase : Entry
    {
        public static readonly BindableProperty PlaceholderFontFamilyProperty = BindableProperty.Create("PlaceholderFontFamily", typeof(string), typeof(FEntryBase));
        public static readonly BindableProperty PlaceholderFontAttributesProperty = BindableProperty.Create("PlaceholderFontAttributes", typeof(FontAttributes), typeof(FEntryBase), FontAttributes.Italic);

        public string PlaceholderFontFamily
        {
            get => (string)GetValue(PlaceholderFontFamilyProperty);
            set => SetValue(PlaceholderFontFamilyProperty, value);
        }

        public FontAttributes PlaceholderFontAttributes
        {
            get => (FontAttributes)GetValue(PlaceholderFontAttributesProperty);
            set => SetValue(PlaceholderFontAttributesProperty, value);
        }

        public FEntryBase()
        {
            FontSize = FSetting.FontSizeLabelContent;
            TextColor = FSetting.TextColorContent;
            FontFamily = FSetting.FontText;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            PlaceholderFontFamily = FSetting.FontTextItalic;
        }
    }
}