using Syncfusion.XForms.ComboBox;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FDropDownButtonSettings : DropDownButtonSettings
    {
        public FDropDownButtonSettings()
        {
            Base();
        }

        public FDropDownButtonSettings(string code, Color color)
        {
            Base();
            FontIcon = code;
            FontColor = color;
        }

        private void Base()
        {
            FontFamily = FSetting.FontIcon;
            FontSize = FSetting.SizeButtonIcon;
            BackgroundColor = Color.Transparent;
            Width = FSetting.SizeButtonIcon;
            Height = FSetting.SizeButtonIcon;
        }
    }
}