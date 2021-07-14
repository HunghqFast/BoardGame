using Syncfusion.SfPullToRefresh.XForms;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPullToRefresh : SfPullToRefresh
    {
        public FPullToRefresh() : base()
        {
            RefreshContentThreshold = 30;
            RefreshContentHeight = 40;
            RefreshContentWidth = 40;
            PullingThreshold = 100;
            ProgressStrokeColor = Color.FromHex("#5ca0d4");
            Pulling += OnPulling;
        }

        private void OnPulling(object sender, PullingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}