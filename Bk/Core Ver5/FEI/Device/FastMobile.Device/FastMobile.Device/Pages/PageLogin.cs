using FastMobile.Core;
using FastMobile.FXamarin.Core;

namespace FastMobile.Device
{
    public partial class PageLogin : CPageLogin
    {
        private readonly FItemProfile Default = new() { ID = 1, Link = "https://app-portal.fast.com.vn", Name = "Fast e-Invoice", IsInternal = "0" };

        public PageLogin(bool showBio) : base(showBio)
        {
            if (!FUtility.CurrentIsUrl)
            {
                ProfileButton.Text = Default.Name;
                FString.SetCurrentProfile(Default);
            }
        }

        protected override void InitPrimaryProfile()
        {
            Profile.PrimarySource.Add(Default);
            Profile.PrimarySource.Add(new() { ID = 2, Link = "https://app-tportal.fast.com.vn", Name = "Demo Fast e-Invoice", IsInternal = "0" });
            Profile.PrimarySource.Add(new() { ID = 3, Link = "http://fetaxapp.fast.com.vn", Name = "Nội bộ (Local)", IsInternal = "0" });
            Profile.PrimarySource.Add(new() { ID = 4, Link = "http://frd.fast.com.vn:9996/HDDT_PTSP_AppService", Name = "PTSP", IsInternal = "0" });
            Profile.PrimarySource.Add(new() { ID = 5, Link = "http://frd.fast.com.vn:8891/HDDT_Xamarin_003_AppService", Name = "003", IsInternal = "0" });
            Profile.PrimarySource.Add(new() { ID = 6, Link = "http://frd.fast.com.vn:8891/HDDT_Xamarin_004_AppService", Name = "004", IsInternal = "0" });
        }
    }
}