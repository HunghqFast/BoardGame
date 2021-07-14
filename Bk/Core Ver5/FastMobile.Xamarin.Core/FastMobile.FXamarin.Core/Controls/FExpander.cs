using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FExpander : FExpanders
    {
        public static readonly BindableProperty IndexProperty = BindableProperty.Create("Index", typeof(int), typeof(FExpanders), 0);

        public int Index
        {
            get => (int)GetValue(IndexProperty);
            set => SetValue(IndexProperty, value);
        }

        public FExpander() : base()
        {
        }
    }
}