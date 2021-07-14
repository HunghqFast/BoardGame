using System;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FCapchaImage : Grid
    {
        public static readonly BindableProperty CapchaWidthProperty = BindableProperty.Create("CapchaWidth", typeof(double), typeof(FCapchaImage), 120d);
        public static readonly BindableProperty CapchaHeightProperty = BindableProperty.Create("CapchaHeight", typeof(double), typeof(FCapchaImage), 40d);
        public static readonly BindableProperty CapchaOpacityProperty = BindableProperty.Create("CapchaOpacity", typeof(double), typeof(FCapchaImage), 1d);
        public static readonly BindableProperty ReloadTextProperty = BindableProperty.Create("ReloadText", typeof(string), typeof(FCapchaImage), string.Empty);
        public static readonly BindableProperty ReloadFontSizeProperty = BindableProperty.Create("ReloadFontSize", typeof(double), typeof(FCapchaImage), 16d);
        public static readonly BindableProperty ReloadFontFamilyProperty = BindableProperty.Create("ReloadFontFamily", typeof(string), typeof(FCapchaImage), Font.Default.FontFamily);
        public static readonly BindableProperty ReloadColorProperty = BindableProperty.Create("ReloadColor", typeof(Color), typeof(FCapchaImage), Color.Default);
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FCapchaImage));
        public static readonly BindableProperty DownloadingTextProperty = BindableProperty.Create("DownloadingText", typeof(string), typeof(FCapchaImage));
        public static readonly BindableProperty DownloadingFontFamilyProperty = BindableProperty.Create("DownloadingFontFamily", typeof(string), typeof(FCapchaImage));
        public static readonly BindableProperty DownloadingFontSizeProperty = BindableProperty.Create("DownloadingFontSize", typeof(double), typeof(FCapchaImage));
        public static readonly BindableProperty DownloadingTextColorProperty = BindableProperty.Create("DownloadingTextColor", typeof(Color), typeof(FCapchaImage), Color.Gray);
        public static readonly BindableProperty DownloadingIsVisibleProperty = BindableProperty.Create("DownloadingIsVisible", typeof(bool), typeof(FCapchaImage), true);

        public double CapchaWidth
        {
            get => (double)GetValue(CapchaWidthProperty);
            set => SetValue(CapchaWidthProperty, value);
        }

        public double CapchaHeight
        {
            get => (double)GetValue(CapchaHeightProperty);
            set => SetValue(CapchaHeightProperty, value);
        }

        public double CapchaOpacity
        {
            get => (double)GetValue(CapchaOpacityProperty);
            set => SetValue(CapchaOpacityProperty, value);
        }

        public double ReloadFontSize
        {
            get => (double)GetValue(ReloadFontSizeProperty);
            set => SetValue(ReloadFontSizeProperty, value);
        }

        public string ReloadFontFamily
        {
            get => (string)GetValue(ReloadFontFamilyProperty);
            set => SetValue(ReloadFontFamilyProperty, value);
        }

        public string ReloadText
        {
            get => (string)GetValue(ReloadTextProperty);
            set => SetValue(ReloadTextProperty, value);
        }

        public Color ReloadColor
        {
            get => (Color)GetValue(ReloadColorProperty);
            set => SetValue(ReloadColorProperty, value);
        }

        public string DownloadingText
        {
            get => (string)GetValue(DownloadingTextProperty);
            set => SetValue(DownloadingTextProperty, value);
        }

        public string DownloadingFontFamily
        {
            get => (string)GetValue(DownloadingFontFamilyProperty);
            set => SetValue(DownloadingFontFamilyProperty, value);
        }

        public double DownloadingFontSize
        {
            get => (double)GetValue(DownloadingFontSizeProperty);
            set => SetValue(DownloadingFontSizeProperty, value);
        }

        public bool DownloadingIsVisible
        {
            get => (bool)GetValue(DownloadingIsVisibleProperty);
            set => SetValue(DownloadingIsVisibleProperty, value);
        }

        public Color DownloadingTextColor
        {
            get => (Color)GetValue(DownloadingTextColorProperty);
            set => SetValue(DownloadingTextColorProperty, value);
        }

        public event EventHandler<EventArgs> Loading, Loaded;

        private readonly Image Image;
        private readonly Button Reload;
        private readonly Label Downloading;
        private readonly ContentView ImageContent;

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public FCapchaImage() : base()
        {
            ImageContent = new ContentView();
            Reload = new Button();
            Image = new Image();
            Downloading = new Label();
            Contructor();
            InitBackground();
            InitReload();
            InitGrid();
            NewCapcha();
            InitDownloading();
        }

        public virtual void NewCapcha()
        {
        }

        protected virtual void Contructor()
        {
        }

        protected virtual void OnReload(object sender, EventArgs e)
        {
            Loading?.Invoke(sender, e);
            NewCapcha();
            Loaded?.Invoke(sender, e);
        }

        private void InitReload()
        {
            Reload.BindingContext = this;
            Reload.HorizontalOptions = LayoutOptions.StartAndExpand;
            Reload.BackgroundColor = Color.Transparent;
            Reload.Margin = new Thickness(15, 0);
            Reload.Clicked += OnReload;
            Reload.SetBinding(Button.TextProperty, ReloadTextProperty.PropertyName);
            Reload.SetBinding(Button.TextColorProperty, ReloadColorProperty.PropertyName);
            Reload.SetBinding(Button.FontFamilyProperty, ReloadFontFamilyProperty.PropertyName);
            Reload.SetBinding(Button.FontSizeProperty, ReloadFontSizeProperty.PropertyName);
        }

        private void InitBackground()
        {
            Image.BindingContext = ImageContent.BindingContext = this;
            Image.Aspect = Aspect.AspectFit;
            Image.SetBinding(Image.SourceProperty, ImageSourceProperty.PropertyName);
            ImageContent.HorizontalOptions = ImageContent.VerticalOptions = LayoutOptions.Fill;
            ImageContent.WidthRequest = 120;
            ImageContent.Content = Image;
            ImageContent.SetBinding(ContentView.WidthRequestProperty, CapchaWidthProperty.PropertyName);
            ImageContent.SetBinding(ContentView.HeightRequestProperty, CapchaHeightProperty.PropertyName);
        }

        private void InitGrid()
        {
            RowSpacing = 0;
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Children.Add(Downloading, 0, 0);
            Children.Add(ImageContent, 0, 0);
            Children.Add(Reload, 1, 0);
        }

        private void InitDownloading()
        {
            Downloading.BindingContext = this;
            DownloadingText = FText.Downloading;
            DownloadingFontSize = FSetting.FontSizeLabelHint;
            DownloadingFontFamily = FSetting.FontText;
            Downloading.VerticalOptions = LayoutOptions.EndAndExpand;
            Downloading.HorizontalOptions = LayoutOptions.StartAndExpand;
            Downloading.SetBinding(Label.TextProperty, DownloadingTextProperty.PropertyName);
            Downloading.SetBinding(Label.FontFamilyProperty, DownloadingFontFamilyProperty.PropertyName);
            Downloading.SetBinding(Label.FontSizeProperty, DownloadingFontSizeProperty.PropertyName);
            Downloading.SetBinding(Label.TextColorProperty, DownloadingTextColorProperty.PropertyName);
            Downloading.SetBinding(Label.IsVisibleProperty, DownloadingIsVisibleProperty.PropertyName);
        }
    }
}