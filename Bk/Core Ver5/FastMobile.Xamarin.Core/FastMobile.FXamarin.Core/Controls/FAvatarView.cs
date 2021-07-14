using Syncfusion.XForms.Border;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FAvatarView : ContentView
    {
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FAvatarView), null);
        public static readonly BindableProperty ImagePaddingProperty = BindableProperty.Create("ImagePadding", typeof(Thickness), typeof(FAvatarView), default(Thickness));
        public static readonly BindableProperty ImageMarginProperty = BindableProperty.Create("ImageMargin", typeof(Thickness), typeof(FAvatarView), default(Thickness));
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(Thickness), typeof(FAvatarView), default(Thickness));
        public static readonly BindableProperty StartBackgroundColorProperty = BindableProperty.Create("StartBackgroundColor", typeof(Color), typeof(FAvatarView), Color.White);
        public static readonly BindableProperty EndBackgroundColorProperty = BindableProperty.Create("EndBackgroundColor", typeof(Color), typeof(FAvatarView), Color.White);

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public Thickness ImagePadding
        {
            get => (Thickness)GetValue(ImagePaddingProperty);
            set => SetValue(ImagePaddingProperty, value);
        }

        public Thickness ImageMargin
        {
            get => (Thickness)GetValue(ImageMarginProperty);
            set => SetValue(ImageMarginProperty, value);
        }

        public Thickness CornerRadius
        {
            get => (Thickness)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Color StartBackgroundColor
        {
            get => (Color)GetValue(StartBackgroundColorProperty);
            set => SetValue(StartBackgroundColorProperty, value);
        }

        public Color EndBackgroundColor
        {
            get => (Color)GetValue(EndBackgroundColorProperty);
            set => SetValue(EndBackgroundColorProperty, value);
        }

        private readonly ContentView ImageContent;
        private readonly Image Image;
        private readonly SfBorder Border;
        private readonly GradientStop Start, End;

        public FAvatarView() : base()
        {
            ImageContent = new ContentView();
            Image = new Image();
            Border = new SfBorder();
            Start = new GradientStop();
            End = new GradientStop();
            Base();
        }

        private void Base()
        {
            Image.BindingContext = this;
            Image.HorizontalOptions = Image.VerticalOptions = LayoutOptions.CenterAndExpand;
            Image.SetBinding(Image.SourceProperty, ImageSourceProperty.PropertyName);
            Image.SetBinding(Image.MarginProperty, ImagePaddingProperty.PropertyName);

            Start.BindingContext = this;
            Start.Offset = 0.0f;
            Start.SetBinding(GradientStop.ColorProperty, StartBackgroundColorProperty.PropertyName);

            End.BindingContext = this;
            End.Offset = 1.0f;
            End.SetBinding(GradientStop.ColorProperty, EndBackgroundColorProperty.PropertyName);

            ImageContent.BindingContext = this;
            ImageContent.Content = Image;
            ImageContent.Background = new LinearGradientBrush { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5), GradientStops = new GradientStopCollection() { Start, End } };
            ImageContent.SetBinding(ContentView.PaddingProperty, ImageMarginProperty.PropertyName);

            Border.BindingContext = this;
            Border.BorderColor = Color.Transparent;
            Border.BorderWidth = 0;
            Border.Content = ImageContent;
            Border.SetBinding(SfBorder.CornerRadiusProperty, CornerRadiusProperty.PropertyName);

            Content = Border;
        }
    }
}