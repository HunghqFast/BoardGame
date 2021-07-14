using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemAbout : BindableObject
    {
        public static readonly BindableProperty ActionProperty = BindableProperty.Create("Action", typeof(string), typeof(FItemAbout));
        public static readonly BindableProperty ControllerProperty = BindableProperty.Create("Controller", typeof(string), typeof(FItemAbout));
        public static readonly BindableProperty TitlePageProperty = BindableProperty.Create("Title", typeof(string), typeof(FItemAbout));
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(FItemAbout));
        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create("Subtitle", typeof(string), typeof(FItemAbout));
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FItemAbout));

        public string Action
        {
            get => (string)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public string Controller
        {
            get => (string)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public string TitlePage
        {
            get => (string)GetValue(TitlePageProperty);
            set => SetValue(TitlePageProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
    }
}