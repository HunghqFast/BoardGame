using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLAttachment : ViewCell
    {
        public FTLAttachment()
        {
            var s = new StackLayout();
            var l = new Label();
            var i = new Image();

            s.Spacing = 0;
            s.WidthRequest = 60;
            s.VerticalOptions = LayoutOptions.Center;

            l.MaxLines = 1;
            l.FontFamily = FSetting.FontText;
            l.FontSize = FSetting.FontSizeLabelHint;
            l.TextColor = FSetting.DisableColor;
            l.LineBreakMode = LineBreakMode.TailTruncation;
            l.HorizontalOptions = LayoutOptions.Center;
            l.SetBinding(Label.TextProperty, "Title");

            i.SetBinding(Image.SourceProperty, "Icon");

            s.Children.Add(i);
            s.Children.Add(l);
            View = s;
        }
    }
}