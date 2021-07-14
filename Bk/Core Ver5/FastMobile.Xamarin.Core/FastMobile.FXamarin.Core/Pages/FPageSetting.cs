using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageSetting : FPage
    {
        public FPageSetting() : base(false, true)
        {
            Title = FText.Settings;
        }

        protected Grid RecordView(string title, GridLength titleHeight)
        {
            var grid = new Grid();
            var l1 = new Label();
            l1.Margin = new Thickness(10, 5);
            l1.Text = title;
            l1.FontSize = FSetting.FontSizeLabelHint;
            l1.FontFamily = FSetting.FontText;
            l1.BackgroundColor = Color.Transparent;
            l1.TextColor = FSetting.DisableColor;
            l1.LineBreakMode = LineBreakMode.TailTruncation;
            l1.MaxLines = 1;
            l1.VerticalTextAlignment = TextAlignment.Center;

            grid.ColumnSpacing = grid.RowSpacing = 0;
            grid.RowDefinitions.Add(new RowDefinition { Height = titleHeight });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.Children.Add(l1, 0, 0);
            return grid;
        }
    }
}