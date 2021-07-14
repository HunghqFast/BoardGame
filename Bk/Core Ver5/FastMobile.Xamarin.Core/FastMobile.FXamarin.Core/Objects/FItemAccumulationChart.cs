using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FItemAccumulationChart : BindableObject
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create("Name", typeof(string), typeof(FItemAccumulationChart));
        public static readonly BindableProperty Value1Property = BindableProperty.Create("Value1", typeof(double), typeof(FItemAccumulationChart), default(double));
        public static readonly BindableProperty StringValue1Property = BindableProperty.Create("StringValue1", typeof(string), typeof(FItemAccumulationChart));
        public static readonly BindableProperty ColorMaker1Property = BindableProperty.Create("ColorMaker1", typeof(Color), typeof(FItemAccumulationChart));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public double Value1
        {
            get => (double)GetValue(Value1Property);
            set => SetValue(Value1Property, value);
        }

        public string StringValue1
        {
            get => (string)GetValue(StringValue1Property);
            set => SetValue(StringValue1Property, value);
        }

        public Color ColorMaker1
        {
            get => (Color)GetValue(ColorMaker1Property);
            set => SetValue(ColorMaker1Property, value);
        }
    }
}