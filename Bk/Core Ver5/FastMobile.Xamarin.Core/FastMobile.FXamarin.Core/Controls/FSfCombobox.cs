using Syncfusion.XForms.ComboBox;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSfComboBox : SfComboBox
    {
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create("BorderWidth", typeof(float), typeof(FSfComboBox), 0f);
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(float), typeof(FSfComboBox), 0f);
        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create("ReturnType", typeof(ReturnType), typeof(FSfComboBox), ReturnType.Default);

        public float BorderWidth
        {
            get => (float)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public ReturnType ReturnType
        {
            get => (ReturnType)GetValue(ReturnTypeProperty);
            set => SetValue(ReturnTypeProperty, value);
        }

        public FSfComboBox() : base()
        {
            ShowClearButton = false;
            SuggestionBoxPlacement = SuggestionBoxPlacement.Auto;
            DropDownButtonSettings.Width = FSetting.SizeIconButton;
            DropDownButtonSettings.FontFamily = FSetting.FontIcon;
            DropDownButtonSettings.FontIcon = FIcons.ChevronDown;
            DropDownButtonSettings.FontColor = FSetting.TextColorContent;
            DropDownButtonSettings.BackgroundColor = DropDownButtonSettings.HighlightFontColor = DropDownButtonSettings.HighlightedBackgroundColor = Color.Transparent;
        }
    }
}
