using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace FastMobile.Device.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FAppDelegate
    {
        protected override FApplication FormsApp
        {
            get => base.FormsApp;
            set => base.FormsApp = value;
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            InitAll(app, options);
            LoadApplication(FormsApp ??= new App());
            return base.FinishedLaunching(app, options);
        }
    }
}