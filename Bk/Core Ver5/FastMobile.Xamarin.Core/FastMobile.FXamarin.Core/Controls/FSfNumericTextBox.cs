using Syncfusion.SfNumericTextBox.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FSfNumericTextBox : SfNumericTextBox
    {
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create("BorderWidth", typeof(float), typeof(FSfComboBox), 0f);
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(float), typeof(FSfComboBox), 0f);

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

        public FSfNumericTextBox() : base()
        {
            FontSize = FSetting.FontSizeLabelContent;
            TextColor = FSetting.TextColorContent;
            FontFamily = FSetting.FontText;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            TextAlignment = TextAlignment.Start;
            ReturnType = ReturnType.Done;
            ParserMode = Parsers.Decimal;
            ClearButtonVisibility = Syncfusion.XForms.Editors.ClearButtonVisibilityMode.WhileEditing;
        }
    }
}