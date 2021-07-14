using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLHeaderNaviBar : Grid
    {
        public FTLHeaderNaviBar(ImageSource headerIcon) : base()
        {
            var img = new Image
            {
                Source = headerIcon,
                Aspect = Aspect.AspectFit,
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand
            };
            if (DeviceInfo.Platform == DevicePlatform.iOS)
                img.Margin = new Thickness(0, 0, 0, 10);

            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = Color.Transparent;
            Children.Add(img);
        }
    }
}