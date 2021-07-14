using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FZoomView : ScrollView
    {
        public FZoomView()
        {
            Orientation = ScrollOrientation.Both;
            VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        }
    }
}