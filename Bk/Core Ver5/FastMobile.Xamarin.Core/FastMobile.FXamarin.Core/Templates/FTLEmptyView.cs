using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FTLEmptyView : ContentView
    {
        private readonly FLabel Label;
        public FTLEmptyView(string nodata) : base()
        {
            Label = new FLabel();
            Label.Text = nodata;
            Base();
        }

        public FTLEmptyView(object bindingContext, string propertyName) : base()
        {
            Content.BindingContext = bindingContext;
            Label.SetBinding(FLabel.TextProperty, propertyName);
            Base();
        }

        private void Base()
        {
            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;
            BackgroundColor = FSetting.BackgroundMain;
            Label.Init(LayoutOptions.CenterAndExpand, LayoutOptions.CenterAndExpand, TextAlignment.Center, TextAlignment.Center);
            Label.Margin = new Thickness(30, 0);
            Label.MaxLines = 10;
            Content = Label;
        }
    }
}