using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FDField : FBParam
    {
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create("Header", typeof(string), typeof(FDField));
        public static readonly BindableProperty DataFormatStringProperty = BindableProperty.Create("DataFormatString", typeof(string), typeof(FDField));
        public static readonly BindableProperty BindingNameProperty = BindableProperty.Create("BindingName", typeof(string), typeof(FDField));
        public static readonly BindableProperty WidthProperty = BindableProperty.Create("Width", typeof(double), typeof(FDField));
        public static readonly BindableProperty HiddenProperty = BindableProperty.Create("Hidden", typeof(bool), typeof(FDField), false);
        public static readonly BindableProperty TypeProperty = BindableProperty.Create("Type", typeof(FDFieldType), typeof(FDField));
        public static readonly BindableProperty StatusProperty = BindableProperty.Create("Status", typeof(FieldStatus), typeof(FDField));
        public static readonly BindableProperty AlignProperty = BindableProperty.Create("Align", typeof(TextAlignment), typeof(FDField));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public string DataFormatString
        {
            get => (string)GetValue(DataFormatStringProperty);
            set => SetValue(DataFormatStringProperty, value);
        }

        public string BindingName
        {
            get => (string)GetValue(BindingNameProperty);
            set => SetValue(BindingNameProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public bool Hidden
        {
            get => (bool)GetValue(HiddenProperty);
            set => SetValue(HiddenProperty, value);
        }

        public FDFieldType Type
        {
            get => (FDFieldType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public FieldStatus Status
        {
            get => (FieldStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public TextAlignment Align
        {
            get => (TextAlignment)GetValue(AlignProperty);
            set => SetValue(AlignProperty, value);
        }
    }
}