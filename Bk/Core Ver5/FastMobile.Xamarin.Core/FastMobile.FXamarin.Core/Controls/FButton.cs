using Syncfusion.XForms.Buttons;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FButton : SfButton
    {
        private readonly ImageSource DisableSource, EnableSource;

        public FButton(string text, string icon, string disableIcon = null)
        {
            EnableSource = ImageSource = icon.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconMenu);
            DisableSource = string.IsNullOrEmpty(disableIcon) ? EnableSource : disableIcon.ToFontImageSource(FSetting.DisableColor, FSetting.SizeIconMenu);
            Base(text);
        }

        public FButton(string text, ImageSource icon, ImageSource disableIcon = null)
        {
            EnableSource = ImageSource = icon;
            DisableSource = disableIcon ?? EnableSource;
            Base(text);
        }

        private void Base(string text)
        {
            Text = text;
            ShowIcon = true;
            TextColor = FSetting.PrimaryColor;
            FontSize = FSetting.FontSizeButton;
            FontFamily = FSetting.FontText;
            BackgroundColor = FSetting.BackgroundMain;
            BorderColor = Color.Transparent;
            CornerRadius = BorderWidth = 0;
            VerticalTextAlignment = TextAlignment.Center;
            HorizontalTextAlignment = TextAlignment.Start;
            ImageAlignment = Alignment.Start;
            VerticalOptions = LayoutOptions.Fill;
            Padding = 0;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == IsEnabledProperty.PropertyName)
            {
                ImageSource = IsEnabled ? EnableSource : DisableSource;
                return;
            }
        }
    }
}