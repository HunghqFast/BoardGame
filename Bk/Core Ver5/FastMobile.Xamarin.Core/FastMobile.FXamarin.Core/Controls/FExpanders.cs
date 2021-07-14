using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FExpanders : Grid
    {
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create("Header", typeof(Label), typeof(FExpanders));
        public static readonly BindableProperty HeaderImageSourceProperty = BindableProperty.Create("HeaderImageSource", typeof(ImageSource), typeof(FExpanders), FIcons.ChevronDown.ToFontImageSource(FSetting.DisableColor, FSetting.SizeIconLegend));
        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create("HeaderBackgroundColor", typeof(Color), typeof(FExpanders), Color.FromHex("#f4f4f4"));
        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(FExpanders));
        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create("ContentTemplate", typeof(DataTemplate), typeof(FExpanders));
        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create("IsExpanded", typeof(bool), typeof(FExpanders), false);
        public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create("HeaderHeight", typeof(GridLength), typeof(FExpanders), new GridLength(35, GridUnitType.Absolute));

        public ImageSource HeaderImageSource
        {
            get => (ImageSource)GetValue(HeaderImageSourceProperty);
            set => SetValue(HeaderImageSourceProperty, value);
        }

        public Label Header
        {
            get => (Label)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(HeaderBackgroundColorProperty);
            set => SetValue(HeaderBackgroundColorProperty, value);
        }

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public GridLength HeaderHeight
        {
            get => (GridLength)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        public event EventHandler<EventArgs> Tapped;

        private readonly Grid H;
        private readonly ContentView IC, C;
        private readonly Image I;
        public FExpanders() : base()
        {
            H = new Grid();
            I = new Image();
            IC = new ContentView();
            C = new ContentView();
            Header = new Label();
            Base();
        }

        public virtual void RenderContent()
        {
            Content ??= ContentTemplate?.CreateContent() as View;
        }

        private void Base()
        {
            H.BindingContext = IC.BindingContext = I.BindingContext = this;

            Header.Padding = new Thickness(10, 0);
            Header.FontSize = FSetting.FontSizeLabelContent;
            Header.TextColor = FSetting.TextColorContent;
            Header.VerticalOptions = LayoutOptions.CenterAndExpand;
            Header.VerticalTextAlignment = TextAlignment.Center;
            Header.LineBreakMode = LineBreakMode.TailTruncation;
            Header.MaxLines = 1;

            I.SetBinding(Image.SourceProperty, HeaderImageSourceProperty.PropertyName);

            IC.Content = I;
            IC.Padding = new Thickness(10, 0);
            IC.SetBinding(View.BackgroundColorProperty, HeaderBackgroundColorProperty.PropertyName);

            H.GestureRecognizers.Add(new TapGestureRecognizer());
            (H.GestureRecognizers[0] as TapGestureRecognizer).Tapped += (s, e) => { IsExpanded = !IsExpanded; Tapped?.Invoke(this, EventArgs.Empty); };
            H.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            H.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            H.RowDefinitions.Add(new RowDefinition { BindingContext = this });
            H.RowDefinitions[^1].SetBinding(RowDefinition.HeightProperty, HeaderHeightProperty.PropertyName, converter: new FDoubleToGridLength());
            H.SetBinding(View.BackgroundColorProperty, HeaderBackgroundColorProperty.PropertyName);

            H.Children.Add(Header, 0, 0);
            H.Children.Add(IC, 1, 0);

            C.BindingContext = this;
            C.SetBinding(ContentView.ContentProperty, ContentProperty.PropertyName);
            C.SetBinding(View.IsVisibleProperty, IsExpandedProperty.PropertyName);

            RowSpacing = ColumnSpacing = 0;
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Children.Add(H, 0, 0);
            Children.Add(C, 0, 1);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsExpandedProperty.PropertyName)
            {
                RenderContent();
                I.RotateTo(IsExpanded ? 180 : 0, 150, Easing.Linear);
                Content?.FadeTo(IsExpanded ? 1 : 0, easing: Easing.Linear);
                HeightRequest = IsExpanded ? -1 : HeaderHeight.Value;
                return;
            }
        }
    }
}
