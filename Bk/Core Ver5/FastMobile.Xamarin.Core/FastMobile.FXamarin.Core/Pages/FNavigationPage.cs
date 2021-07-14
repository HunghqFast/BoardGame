using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.Page;

namespace FastMobile.FXamarin.Core
{
    public partial class FNavigationPage : NavigationPage, IFGradientBackground
    {
        public static readonly BindableProperty TransitionTypeProperty = BindableProperty.Create("TransitionType", typeof(FTransitionType), typeof(FNavigationPage), FTransitionType.None);
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create("StartColor", typeof(Color), typeof(FNavigationPage), Color.Default);
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create("EndColor", typeof(Color), typeof(FNavigationPage), Color.Default);
        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create("TitleFontFamily", typeof(string), typeof(FNavigationPage), default(string));
        public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create("TitleFontSize", typeof(float), typeof(FNavigationPage), default(float));
        public static readonly BindableProperty TitleTextAlignmentProperty = BindableProperty.Create("TitleHorizontalAlignment", typeof(TextAlignment), typeof(FNavigationPage), TextAlignment.Center);
        public static readonly BindableProperty TitleFontAttributesProperty = BindableProperty.Create("TitleFontAttributes", typeof(FontAttributes), typeof(FNavigationPage), FontAttributes.None);

        public FTransitionType TransitionType
        {
            get => (FTransitionType)GetValue(TransitionTypeProperty);
            set => SetValue(TransitionTypeProperty, value);
        }

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

        public string TitleFontFamily
        {
            get => (string)GetValue(TitleFontFamilyProperty);
            set => SetValue(TitleFontFamilyProperty, value);
        }

        public TextAlignment TitleTextAlignment
        {
            get => (TextAlignment)GetValue(TitleTextAlignmentProperty);
            set => SetValue(TitleTextAlignmentProperty, value);
        }

        public float TitleFontSize
        {
            get => (float)GetValue(TitleFontSizeProperty);
            set => SetValue(TitleFontSizeProperty, value);
        }

        public FontAttributes TitleFontAttributes
        {
            get => (FontAttributes)GetValue(TitleFontAttributesProperty);
            set => SetValue(TitleFontAttributesProperty, value);
        }

        public event EventHandler<EventArgs> Appeared, Disappeared;

        public FNavigationPage() : base()
        {
            InitBase();
        }

        public FNavigationPage(Page root) : base(root)
        {
            InitBase();
        }

        public virtual void OnAppreared()
        {
            Appeared?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnDisappreared()
        {
            Disappeared?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FInterface.IFAndroid?.SetCurentWindowBackground(StartColor, EndColor);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(StartColor) || propertyName == nameof(EndColor))
            {
                FInterface.IFAndroid?.SetCurentWindowBackground(StartColor, EndColor);
                return;
            }

            if (propertyName == IsBusyProperty.PropertyName)
            {
                CurrentPage.IsBusy = IsBusy;
                return;
            }
        }

        private void InitBase()
        {
            StartColor = FSetting.StartColor;
            EndColor = FSetting.EndColor;
            BarTextColor = Color.White;
            BarBackgroundColor = Color.Transparent;
            TitleFontSize = FSetting.IsAndroid ? FSetting.FontSizeLabelTitle + 3 : FSetting.FontSizeLabelTitle + 2;
            TitleFontFamily = FSetting.FontTextMedium;
            TitleFontAttributes = FontAttributes.Bold;
            TitleTextAlignment = TextAlignment.Center;
            if (FSetting.IsAndroid)
                TransitionType = FTransitionType.Fade;
        }
    }
}