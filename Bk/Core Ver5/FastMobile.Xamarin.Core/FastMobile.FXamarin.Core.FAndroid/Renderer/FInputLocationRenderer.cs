using Android.Content;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FInputLocation), typeof(FInputLocationRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FInputLocationRenderer : ViewRenderer
    {
        public FInputLocationRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is FInputLocation input)
                input.Location.StopUpdating();
        }
    }
}