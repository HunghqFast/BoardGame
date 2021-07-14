using Syncfusion.XForms.Buttons;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FCheckBox : SfCheckBox
    {
        public FCheckBox(bool isBorder)
        {
            if (isBorder) Base(Color.White, FSetting.CheckColor, FSetting.CheckColor);
            else Base(FSetting.CheckColor, Color.Transparent, Color.Transparent);
            BorderWidth = isBorder ? 1 : 0;
        }

        private void Base(Color tick, Color check, Color uncheck)
        {
            Margin = 0;
            HeightRequest = 30;
            WidthRequest = 30;
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;
            InputTransparent = true;
            TickColor = tick;
            CheckedColor = check;
            UncheckedColor = uncheck;
        }
    }
}