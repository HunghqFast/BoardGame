using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FBParam : BindableObject
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create("Name", typeof(string), typeof(FBParam));
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(string), typeof(FBParam));
        public static readonly BindableProperty DefaultValueProperty = BindableProperty.Create("DefaultValue", typeof(string), typeof(FBParam));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string DefaultValue
        {
            get => (string)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }
    }
}