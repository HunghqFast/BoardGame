using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FInputLocation), typeof(FInputLocationRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FInputLocationRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is FInputLocation input)
                input.Location.StopUpdating();
        }
    }
}