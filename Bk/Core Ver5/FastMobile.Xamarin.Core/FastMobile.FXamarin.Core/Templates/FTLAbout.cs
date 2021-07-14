using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLAbout : Frame
    {
        public FTLAbout(object sender, Action<object, IFDataEvent> action) : base()
        {
            var grid = new Grid();
            var img = new Image();
            var title = new Label();
            var subTitle = new Label();
            var btn = new FButtonEffect(sender, action);

            img.SetBinding(Image.SourceProperty, FItemAbout.ImageSourceProperty.PropertyName);
            title.SetBinding(Label.TextProperty, FItemAbout.TitleProperty.PropertyName);
            subTitle.SetBinding(Label.TextProperty, FItemAbout.SubtitleProperty.PropertyName);

            img.WidthRequest = 130;
            img.VerticalOptions = LayoutOptions.Start;

            title.FontFamily = FSetting.FontText;
            title.FontSize = FSetting.FontSizeLabelTitle;
            title.HorizontalTextAlignment = TextAlignment.Start;
            title.LineBreakMode = LineBreakMode.TailTruncation;
            title.TextColor = FSetting.ColorTime;
            title.MaxLines = 2;

            subTitle.FontFamily = FSetting.FontText;
            subTitle.FontSize = FSetting.FontSizeLabelContent;
            subTitle.HorizontalTextAlignment = TextAlignment.Start;
            subTitle.TextColor = FSetting.TextColorContent;
            subTitle.LineBreakMode = LineBreakMode.TailTruncation;
            subTitle.MaxLines = 4;

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.Padding = 5;
            grid.ColumnSpacing = 10;
            grid.Children.Add(img, 0, 0);
            grid.Children.Add(title, 1, 0);
            grid.Children.Add(subTitle, 1, 1);
            grid.Padding = new Thickness(10);
            btn.Content = grid;

            Grid.SetRowSpan(img, 2);
            HasShadow = false;
            BorderColor = FSetting.LineBoxReportColor;
            BackgroundColor = FSetting.BackgroundMain;
            Padding = CornerRadius = 0;
            Margin = new Thickness(0, 0, 0, 10);
            Content = btn;
        }
    }
}