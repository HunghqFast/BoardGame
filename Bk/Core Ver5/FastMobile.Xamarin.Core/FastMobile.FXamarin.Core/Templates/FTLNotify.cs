using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLNotify : ViewCell
    {
        public FTLNotify() : base()
        {
            var grid = new Grid();
            var img = new Image();
            var stackImg = new StackLayout();
            var title = new FLabel();
            var subTitle = new FLabel();
            var lbTime = new FLabel();
            var box = new BoxView();

            img.HorizontalOptions = img.VerticalOptions = LayoutOptions.CenterAndExpand;
            img.SetBinding(Image.SourceProperty, FItemNotify.IconProperty.PropertyName);

            stackImg.VerticalOptions = LayoutOptions.StartAndExpand;
            stackImg.HorizontalOptions = LayoutOptions.CenterAndExpand;
            stackImg.Children.Add(img);

            title.FontSize = FSetting.FontSizeLabelTitle;
            title.FontAttributes = FontAttributes.Bold;
            title.FontFamily = FSetting.FontTextMedium;
            title.Init(LayoutOptions.Fill, LayoutOptions.Start, TextAlignment.Start, TextAlignment.Start);
            title.SetBinding(Label.TextProperty, FItemNotify.TitleProperty.PropertyName);
            title.SetBinding(Label.TextColorProperty, FItemNotify.ColorContentProperty.PropertyName);

            subTitle.MaxLines = 5;
            subTitle.LineHeight = 1.2;
            subTitle.Init(LayoutOptions.Fill, LayoutOptions.Start, TextAlignment.Start, TextAlignment.Start);
            subTitle.SetBinding(Label.TextProperty, FItemNotify.ContentProperty.PropertyName);
            subTitle.SetBinding(Label.TextColorProperty, FItemNotify.ColorContentProperty.PropertyName);

            lbTime.FontSize = FSetting.FontSizeLabelContent - 2;
            lbTime.Init(LayoutOptions.Start, LayoutOptions.EndAndExpand, TextAlignment.Start, TextAlignment.Start);
            lbTime.SetBinding(Label.TextColorProperty, FItemNotify.ColorTimeProperty.PropertyName);
            lbTime.SetBinding(Label.TextProperty, FItemNotify.TimeProperty.PropertyName);

            box.HorizontalOptions = box.VerticalOptions = LayoutOptions.Fill;
            box.BackgroundColor = FSetting.LineBoxReportColor;

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = 5 });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = 0 });
            grid.RowDefinitions.Add(new RowDefinition { Height = 1 });
            grid.RowSpacing = 5;
            grid.ColumnSpacing = 0;
            grid.VerticalOptions = grid.HorizontalOptions = LayoutOptions.Fill;
            grid.Children.Add(stackImg, 0, 1);
            grid.Children.Add(title, 1, 1);
            grid.Children.Add(subTitle, 1, 2);
            grid.Children.Add(lbTime, 1, 3);
            grid.Children.Add(box, 1, 5);
            View = grid;
        }
    }
}