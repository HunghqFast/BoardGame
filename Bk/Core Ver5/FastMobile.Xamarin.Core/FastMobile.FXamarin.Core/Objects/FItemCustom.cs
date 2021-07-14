using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemCustom : BindableObject
    {
        public static readonly BindableProperty IDProperty = BindableProperty.Create("ID", typeof(string), typeof(FItemCustom));
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FItemCustom));

        public string ID
        {
            get => (string)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public FItemCustom()
        {
        }

        public FItemCustom(string id, string value)
        {
            ID = id;
            Value = value;
        }
    }
}