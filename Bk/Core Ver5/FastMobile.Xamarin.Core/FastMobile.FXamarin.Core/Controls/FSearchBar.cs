using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSearchBar : SearchBar
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create("StartColor", typeof(Color), typeof(FSearchBar), Color.White);
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create("EndColor", typeof(Color), typeof(FSearchBar), Color.White);
        public static readonly BindableProperty SearchBoxColorProperty = BindableProperty.Create("SearchBoxColor", typeof(Color), typeof(FSearchBar), Color.Default);
        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(ImageSource), typeof(FSearchBar));
        public static readonly BindableProperty IOSShowCancelButtonProperty = BindableProperty.Create("IOSShowCancelButton", typeof(bool), typeof(FSearchBar), true);
        public static readonly BindableProperty IOSCancelTextProperty = BindableProperty.Create("IOSCancelText", typeof(string), typeof(FSearchBar));

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public Color SearchBoxColor
        {
            get => (Color)GetValue(SearchBoxColorProperty);
            set => SetValue(SearchBoxColorProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public bool IOSShowCancelButton
        {
            get => (bool)GetValue(IOSShowCancelButtonProperty);
            set => SetValue(IOSShowCancelButtonProperty, value);
        }

        public string IOSCancelText
        {
            get => (string)GetValue(IOSCancelTextProperty);
            set => SetValue(IOSCancelTextProperty, value);
        }

        private readonly GradientStop Start, End;

        public FSearchBar() : base()
        {
            Start = new GradientStop { Offset = 0.0f };
            End = new GradientStop { Offset = 1.0f };
            IOSCancelText = FText.Cancel;
            SearchBoxColor = Color.FromHex("#306e6e6e");
            TextColor = Color.White;

            Start.BindingContext = End.BindingContext = this;
            Start.SetBinding(GradientStop.ColorProperty, StartColorProperty.PropertyName);
            End.SetBinding(GradientStop.ColorProperty, EndColorProperty.PropertyName);
            Background = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection { Start, End } };
        }
    }
}