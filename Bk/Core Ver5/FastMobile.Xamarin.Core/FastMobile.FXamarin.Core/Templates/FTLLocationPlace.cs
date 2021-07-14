using Syncfusion.XForms.Border;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLLocationPlace : ViewCell
    {
        public FTLLocationPlace()
        {
            var v = new StackLayout();
            var t = new StackLayout();
            var title = new Label();
            var address = new Label();
            var image = new Image();
            var border = new SfBorder();

            title.FontSize = FSetting.FontSizeLabelContent;
            title.TextColor = FSetting.TextColorTitle;
            title.LineBreakMode = LineBreakMode.TailTruncation;
            title.MaxLines = 1;
            title.SetBinding(Label.TextProperty, FPlace.PlaceNameProperty.PropertyName);

            address.FontSize = FSetting.FontSizeLabelHint;
            address.TextColor = FSetting.TextColorContent;
            address.LineBreakMode = LineBreakMode.TailTruncation;
            address.MaxLines = 2;
            address.SetBinding(Label.TextProperty, FPlace.AddressProperty.PropertyName);

            image.VerticalOptions = image.HorizontalOptions = LayoutOptions.CenterAndExpand;
            image.HeightRequest = image.WidthRequest = FSetting.SizeIconButton;
            image.SetBinding(Image.SourceProperty, FPlace.IconProperty.PropertyName);

            border.WidthRequest = border.HeightRequest = 45;
            border.CornerRadius = 50;
            border.BorderWidth = 0;
            border.VerticalOptions = border.HorizontalOptions = LayoutOptions.Start;
            border.Content = image;
            border.SetBinding(SfBorder.BackgroundColorProperty, FPlace.IconBackgroundColorProperty.PropertyName);

            t.Spacing = 4;
            t.HorizontalOptions = LayoutOptions.StartAndExpand;
            t.Children.Add(title);
            t.Children.Add(address);

            v.Spacing = 10;
            v.Orientation = StackOrientation.Horizontal;
            v.Children.Add(border);
            v.Children.Add(t);

            View = v;
        }
    }
}