using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FLabel : Label
    {
        public FLabel() : base()
        {
            FontFamily = FSetting.FontText;
            FontSize = FSetting.FontSizeLabelContent;
            TextColor = FSetting.TextColorContent;
            LineBreakMode = LineBreakMode.TailTruncation;
        }

        public void Init(LayoutOptions horizontal, LayoutOptions vertical, TextAlignment horizontalText, TextAlignment verticalText)
        {
            HorizontalOptions = horizontal;
            VerticalOptions = vertical;
            HorizontalTextAlignment = horizontalText;
            VerticalTextAlignment = verticalText;
        }
    }
}