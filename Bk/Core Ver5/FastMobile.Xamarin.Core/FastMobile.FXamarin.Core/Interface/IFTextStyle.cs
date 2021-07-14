using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFTextStyle : IFLayoutStyle
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(IFTextStyle), string.Empty, BindingMode.TwoWay);
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(IFTextStyle), Font.Default.FontFamily);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(IFTextStyle), Font.Default.FontSize);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(IFTextStyle), Color.Default);
        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(IFTextStyle), FontAttributes.None);
        public static readonly BindableProperty HorizontalProperty = BindableProperty.Create("Horizontal", typeof(TextAlignment), typeof(IFTextStyle), TextAlignment.Start);
        public static readonly BindableProperty VerticalProperty = BindableProperty.Create("Vertical", typeof(TextAlignment), typeof(IFTextStyle), TextAlignment.Center);

        string Text { get; set; }
        string FontFamily { get; set; }
        double FontSize { get; set; }
        Color TextColor { get; set; }
        FontAttributes FontAttributes { get; set; }
        TextAlignment Horizontal { get; set; }
        TextAlignment Vertical { get; set; }
    }
}