using Syncfusion.XForms.Border;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FUnderLine : ContentView
    {
        public const string LineColor = "#DEDEDE";

        public FUnderLine(View content)
        {
            var t = new SfBorder();
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            content.Margin = new Thickness(0, 0, FSetting.IsAndroid ? 1 : 0, -15);
            t.BackgroundColor = Color.Transparent;
            t.BorderColor = Color.FromHex(LineColor);
            t.BorderThickness = new Thickness(0, 0, 0, 1);
            g.Children.Add(content, 0, 0);
            t.Content = g;
            t.Margin = new Thickness(0, 0, 0, 14);
            Content = t;
        }
    }
}