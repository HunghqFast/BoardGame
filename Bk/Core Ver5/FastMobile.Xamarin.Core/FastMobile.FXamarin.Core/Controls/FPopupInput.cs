using Syncfusion.XForms.PopupLayout;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPopupInput : SfPopupLayout
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(FPopupInput));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public readonly FButton Submiter, Closer;

        protected readonly ContentView V;
        protected readonly Grid G, B;
        private readonly Label L;

        public FPopupInput() : base()
        {
            Submiter = new FButton(FText.Accept, FIcons.Check);
            Closer = new FButton(FText.Cancel, FIcons.Close);
            L = new Label();
            G = new Grid();
            B = new Grid();
            V = new ContentView();
            Base();
        }

        private void Base()
        {
            V.VerticalOptions = LayoutOptions.Fill;
            V.HorizontalOptions = LayoutOptions.Fill;

            L.BindingContext = this;
            L.FontFamily = FSetting.FontTextBold;
            L.FontSize = FSetting.FontSizeLabelTitle;
            L.TextColor = FSetting.TextColorTitle;
            L.FontAttributes = FontAttributes.Bold;
            L.HorizontalOptions = L.VerticalOptions = LayoutOptions.Fill;
            L.VerticalTextAlignment = L.HorizontalTextAlignment = TextAlignment.Center;
            L.SetBinding(Label.TextProperty, TitleProperty.PropertyName);

            B.Padding = new Thickness(10, 0);
            B.ColumnSpacing = FSetting.SpacingButtons;

            B.RowDefinitions.Add(new RowDefinition { Height = FSetting.HeightRowGrid });
            B.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            B.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            B.Children.Add(Submiter, 0, 0);
            B.Children.Add(Closer, 1, 0);
            B.BackgroundColor = Color.White;

            G.BindingContext = PopupView;
            G.RowSpacing = 0;
            G.VerticalOptions = G.HorizontalOptions = LayoutOptions.Fill;
            G.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = FSetting.HeightRowGrid });
            G.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            G.RowDefinitions.Add(new RowDefinition { Height = 1 });
            G.RowDefinitions.Add(new RowDefinition { Height = FSetting.HeightRowGrid });
            G.Children.Add(L, 0, 0);
            G.Children.Add(V, 0, 1);
            G.Children.Add(new FLine(), 0, 2);
            G.Children.Add(new ScrollView { Content = B, Orientation = ScrollOrientation.Horizontal }, 0, 3);
            G.BackgroundColor = Color.White;

            StaysOpen = true;
            PopupView.ShowCloseButton = false;
            PopupView.ShowHeader = false;
            PopupView.ShowFooter = false;
            PopupView.PopupStyle.CornerRadius = 5;
            PopupView.WidthRequest = FSetting.ScreenWidth - 20;
            PopupView.AutoSizeMode = AutoSizeMode.None;
            PopupView.BackgroundColor = Color.White;
            PopupView.ContentTemplate = new DataTemplate(() => G);
        }
    }
}