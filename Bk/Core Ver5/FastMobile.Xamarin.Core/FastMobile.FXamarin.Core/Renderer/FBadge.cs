using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBadge : Frame
    {
        public static BindableProperty BadgeTextProperty = BindableProperty.Create("BadgeText", typeof(string), typeof(FBadge), default(string), propertyChanged: BadgePropertyChanged);
        public static BindableProperty BadgeTextColorProperty = BindableProperty.Create("BadgeTextColor", typeof(Color), typeof(FBadge), Color.Default, propertyChanged: BadgePropertyChanged);
        public static BindableProperty BadgeFontAttributesProperty = BindableProperty.Create("BadgeFontAttributes", typeof(FontAttributes), typeof(FBadge), FontAttributes.Bold, propertyChanged: BadgePropertyChanged);
        public static BindableProperty BadgeFontFamilyProperty = BindableProperty.Create("BadgeFontFamily", typeof(string), typeof(FBadge), Font.Default.FontFamily, propertyChanged: BadgePropertyChanged);
        public static BindableProperty BadgeFontSizeProperty = BindableProperty.Create("BadgeFontSizeProperty", typeof(double), typeof(FBadge), 5, propertyChanged: BadgePropertyChanged);

        public string BadgeText
        {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        public Color BadgeTextColor
        {
            get => (Color)GetValue(BadgeTextColorProperty);
            set => SetValue(BadgeTextColorProperty, value);
        }

        public FontAttributes BadgeFontAttributes
        {
            get => (FontAttributes)GetValue(BadgeFontAttributesProperty);
            set => SetValue(BadgeFontAttributesProperty, value);
        }

        public string BadgeFontFamily
        {
            get => (string)GetValue(BadgeFontFamilyProperty);
            set => SetValue(BadgeFontFamilyProperty, value);
        }

        public double BadgeFontSize
        {
            get => (double)GetValue(BadgeFontSizeProperty);
            set => SetValue(BadgeFontSizeProperty, value);
        }

        private new Label Content => (Label)base.Content;

        public FBadge()
        {
            base.Content = new Label
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = FSetting.FontText,
                FontSize = 5
            };

            Padding = new Thickness(7, 3);
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.End;
            BackgroundColor = Color.Red;
            UpdateBadgeProperties();
        }

        protected virtual void UpdateBadgeProperties()
        {
            if (Content == null)
            {
                return;
            }

            if (Content.FontAttributes != BadgeFontAttributes)
            {
                Content.FontAttributes = BadgeFontAttributes;
            }

            if (!string.IsNullOrWhiteSpace(BadgeFontFamily))
            {
                Content.FontFamily = BadgeFontFamily;
            }

            var fontSize = BadgeFontSize > 0 ? BadgeFontSize : Content.FontSize;
            if (Content.FontSize != fontSize)
            {
                Content.FontSize = fontSize;
            }

            if (Content.TextColor != BadgeTextColor)
            {
                Content.TextColor = BadgeTextColor;
            }

            var isVisible = !string.IsNullOrEmpty(BadgeText);
            if (IsVisible != isVisible)
            {
                IsVisible = isVisible;
            }

            if (Content.Text != BadgeText)
            {
                Content.Text = BadgeText;
            }
        }

        private static void BadgePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as FBadge)?.UpdateBadgeProperties();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateCornerRadius(height);
        }

        private void UpdateCornerRadius(double heightHint)
        {
            var cornerRadius = Height > 0f ? (float)Height / 5f : heightHint > 0 ? (float)heightHint / 5f : (float)(BadgeFontSize + Padding.VerticalThickness) / 5f;
            if (CornerRadius != cornerRadius)
            {
                CornerRadius = cornerRadius;
            }
        }
    }
}