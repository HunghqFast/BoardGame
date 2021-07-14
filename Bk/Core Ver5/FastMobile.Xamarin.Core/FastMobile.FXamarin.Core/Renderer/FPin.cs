using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core
{
    public class FPin : Pin
    {
        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(ImageSource), typeof(FPin));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}