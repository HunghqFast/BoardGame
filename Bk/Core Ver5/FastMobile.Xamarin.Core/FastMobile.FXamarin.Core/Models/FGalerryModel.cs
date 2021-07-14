using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FGalerryModel : BindableObject
    {
        public static readonly BindableProperty ByteArrayProperty = BindableProperty.Create("ByteArray", typeof(byte[]), typeof(FGalerryModel));
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create("Checked", typeof(bool), typeof(FGalerryModel));
        public static readonly BindableProperty PathProperty = BindableProperty.Create("Path", typeof(string), typeof(FGalerryModel));

        public byte[] ByteArray
        {
            get => (byte[])GetValue(ByteArrayProperty);
            set => SetValue(ByteArrayProperty, value);
        }

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set => SetValue(CheckedProperty, value);
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
    }
}