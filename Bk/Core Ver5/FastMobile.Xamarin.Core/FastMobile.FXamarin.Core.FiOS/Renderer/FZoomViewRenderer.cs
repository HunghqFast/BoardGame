using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using Foundation;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FZoomView), typeof(FZoomViewRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    [Preserve(AllMembers = true)]
    public class FZoomViewRenderer : ScrollViewRenderer
    {
        public FZoomViewRenderer()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            MaximumZoomScale = 3f;
            MinimumZoomScale = 1.0f;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Subviews.Length > 0)
                ViewForZoomingInScrollView += GetViewForZooming;
            else
                ViewForZoomingInScrollView -= GetViewForZooming;
        }

        public UIView GetViewForZooming(UIScrollView sv)
        {
            return Subviews.FirstOrDefault();
        }
    }
}