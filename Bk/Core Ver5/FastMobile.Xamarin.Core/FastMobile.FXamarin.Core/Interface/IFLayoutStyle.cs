using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFLayoutStyle : IFViewStyle
    {
        public static readonly BindableProperty PaddingProperty = BindableProperty.Create("Padding", typeof(Thickness), typeof(IFViewStyle), new Thickness(0));
        Thickness Padding { get; set; }
    }
}