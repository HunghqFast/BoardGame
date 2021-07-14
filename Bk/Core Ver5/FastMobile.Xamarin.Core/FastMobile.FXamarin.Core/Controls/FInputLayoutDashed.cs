using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputLayoutDashed : SfTextInputLayout
    {
        public FInputLayoutDashed() : base()
        {
            Base();
        }

        private void Base()
        {
            FocusedColor = UnfocusedColor = Color.FromHex("#e8e8e8");
            FocusedStrokeWidth = UnfocusedStrokeWidth = 1;
        }
    }
}