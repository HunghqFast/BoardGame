using FastMobile.Core;
using FastMobile.FXamarin.Core;
using Xamarin.Forms;

namespace FastMobile.Device
{
    public class TabContent : CTabContent
    {
        public TabContent(FPage root) : base(root)
        {
        }

        protected override ImageSource Icon(string group)
        {
            return group switch
            {
                "01" => FIcons.Cogs.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton),
                "02" => FIcons.Newspaper.ToFontImageSource(Color.FromHex("#e0218a"), FSetting.SizeIconButton),
                "03" => FIcons.Sale.ToFontImageSource(FSetting.WarningColor, FSetting.SizeIconButton),
                _ => FIcons.Cogs.ToFontImageSource(FSetting.PrimaryColor, FSetting.SizeIconButton)
            };
        }
    }
}