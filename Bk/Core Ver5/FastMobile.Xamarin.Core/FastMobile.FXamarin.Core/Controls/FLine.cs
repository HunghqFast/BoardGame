using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLine : BoxView
    {
        public FLine(string color = "")
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            HeightRequest = 1;
            Color = string.IsNullOrWhiteSpace(color) ? FSetting.LineBoxReportColor : Color.FromHex(color);
        }
    }
}