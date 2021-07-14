using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FPageReport), typeof(FPageReportRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FPageReportRenderer : FPageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            var navctrl = this.ViewController.NavigationController;
            navctrl.InteractivePopGestureRecognizer.Enabled = false;
        }
    }
}