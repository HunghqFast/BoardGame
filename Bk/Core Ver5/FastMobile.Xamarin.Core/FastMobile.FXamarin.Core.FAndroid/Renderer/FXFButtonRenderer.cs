using Android.Content;
using FastMobile.FXamarin.Core.FAndroid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(FXFButtonRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FXFButtonRenderer : ButtonRenderer
    {
        public FXFButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control?.SetAllCaps(false);
        }
    }
}