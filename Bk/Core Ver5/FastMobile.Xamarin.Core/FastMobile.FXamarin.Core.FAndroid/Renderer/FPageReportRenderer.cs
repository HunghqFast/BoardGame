using Android.Content;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FPageReport), typeof(FPageReportRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FPageReportRenderer : FPageSearchRenderer
    {
        public FPageReportRenderer(Context context) : base(context)
        {
        }
    }
}