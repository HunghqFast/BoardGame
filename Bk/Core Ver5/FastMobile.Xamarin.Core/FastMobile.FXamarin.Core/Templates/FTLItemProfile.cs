using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLItemProfile : ViewCell
    {
        public FTLItemProfile() : base()
        {
            var main = new Grid();
            var avt = new FAvatarView();
            var title = new Label();
            var line = new FLine();
            var check = new Image();
            var checkView = new ContentView() { Content = check };

            title.FontFamily = FSetting.FontText;
            title.TextColor = FSetting.TextColorContent;
            title.HorizontalOptions = LayoutOptions.Fill;
            title.LineBreakMode = LineBreakMode.TailTruncation;
            title.FontSize = FSetting.FontSizeLabelTitle + 1;
            title.MaxLines = 2;
            title.SetBinding(Label.TextProperty, "Name");

            avt.HeightRequest = avt.WidthRequest = 50;
            avt.CornerRadius = 100;
            avt.StartBackgroundColor = FSetting.StartColor;
            avt.EndBackgroundColor = FSetting.EndColor;
            avt.VerticalOptions = avt.HorizontalOptions = LayoutOptions.CenterAndExpand;
            avt.SetBinding(FAvatarView.ImageSourceProperty, "Avatar", BindingMode.TwoWay, new FStringToFontImageSource(Color.White, FSetting.SizeIconButton));

            check.SetBinding(Image.SourceProperty, "Current", BindingMode.Default, new FStringToFontImageSource(FSetting.SuccessColor, FSetting.SizeIconButton));
            checkView.VerticalOptions = checkView.HorizontalOptions = LayoutOptions.CenterAndExpand;

            main.ColumnSpacing = main.RowSpacing = 0;
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = 12 });
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = 12 });
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            main.ColumnDefinitions.Add(new ColumnDefinition { Width = 70 });
            main.RowDefinitions.Add(new RowDefinition { Height = 8 });
            main.RowDefinitions.Add(new RowDefinition { Height = 5 });
            main.RowDefinitions.Add(new RowDefinition { Height = 50 });
            main.RowDefinitions.Add(new RowDefinition { Height = 8 });
            main.RowDefinitions.Add(new RowDefinition { Height = 1 });
            main.Children.Add(avt, 1, 1);
            main.Children.Add(title, 3, 2);
            main.Children.Add(line, 3, 4);
            main.Children.Add(checkView, 4, 0);
            Grid.SetColumnSpan(line, 2);
            Grid.SetRowSpan(avt, 2);
            Grid.SetRowSpan(checkView, main.RowDefinitions.Count);
            View = main;
        }
    }
}