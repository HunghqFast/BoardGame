using Android.Content;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FPage), typeof(FPageRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FPageRenderer : PageRenderer
    {
        private FPage Curent => Element as FPage;

        public FPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Curent?.OnAppeared();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            Curent?.OnDisappeared();
        }
    }
}