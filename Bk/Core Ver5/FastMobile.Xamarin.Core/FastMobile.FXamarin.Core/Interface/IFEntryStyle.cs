using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFEntryStyle : IFTextStyle
    {
        public static readonly BindableProperty HintTextProperty = BindableProperty.Create("HintText", typeof(string), typeof(IFEntryStyle), string.Empty, BindingMode.TwoWay);
        public static readonly BindableProperty HintFontFamilyProperty = BindableProperty.Create("HintFontFamily", typeof(string), typeof(IFEntryStyle), Font.Default.FontFamily);
        public static readonly BindableProperty HintFontSizeProperty = BindableProperty.Create("HintFontSize", typeof(double), typeof(IFEntryStyle), Font.Default.FontSize);
        public static readonly BindableProperty HintFontAttributesProperty = BindableProperty.Create("HintFontAttributes", typeof(FontAttributes), typeof(IFEntryStyle), FontAttributes.None);
        public static readonly BindableProperty FocusColorProperty = BindableProperty.Create("FocusColor", typeof(Color), typeof(IFEntryStyle), Color.Default);
        public static readonly BindableProperty UnFocusColorProperty = BindableProperty.Create("UnFocusColor", typeof(Color), typeof(IFEntryStyle), Color.Default);
        string HintText { get; set; }
        string HintFontFamily { get; set; }
        double HintFontSize { get; set; }
        FontAttributes HintFontAttributes { get; set; }
        Color FocusColor { get; set; }
        Color UnFocusColor { get; set; }
    }
}