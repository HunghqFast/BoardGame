using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFViewStyle
    {
        public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create("HorizontalOptions", typeof(LayoutOptions), typeof(IFViewStyle), View.HorizontalOptionsProperty.DefaultValue);
        public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create("VerticalOptions", typeof(LayoutOptions), typeof(IFViewStyle), View.VerticalOptionsProperty.DefaultValue);
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create("BackgroundColor", typeof(Color), typeof(IFViewStyle), View.BackgroundColorProperty.DefaultValue);
        public static readonly BindableProperty MarginProperty = BindableProperty.Create("Margin", typeof(Thickness), typeof(IFViewStyle), View.MarginProperty.DefaultValue);
        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create("HeightRequest", typeof(double), typeof(IFViewStyle), View.HeightRequestProperty.DefaultValue);
        public static readonly BindableProperty WidthRequestProperty = BindableProperty.Create("WidthRequest", typeof(double), typeof(IFViewStyle), View.WidthRequestProperty.DefaultValue);

        LayoutOptions HorizontalOptions { get; set; }
        LayoutOptions VerticalOptions { get; set; }
        Color BackgroundColor { get; set; }
        Thickness Margin { get; set; }
        double HeightRequest { get; set; }
        double WidthRequest { get; set; }

        void SetBindingStyle(View view);
    }
}