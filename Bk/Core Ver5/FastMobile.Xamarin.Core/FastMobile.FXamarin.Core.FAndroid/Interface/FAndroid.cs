using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FAndroid : IFAndroid
    {
        Window Window => Platform.CurrentActivity.Window;

        public void SetCurentWindowBackground(Color left, Color right)
        {
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetBackgroundDrawable(new GradientDrawable(GradientDrawable.Orientation.LeftRight, new int[] { left.ToAndroid(), right.ToAndroid() }));
        }

        public void TransparentStatusBar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                SetWindowFlag(WindowManagerFlags.TranslucentStatus, true);
                return;
            }
            SetWindowFlag(WindowManagerFlags.TranslucentStatus, false);
            Window.SetStatusBarColor(Color.Transparent.ToAndroid());
        }

        private void SetWindowFlag(WindowManagerFlags bits, bool on)
        {
            if (on)
            {
                Window.Attributes.Flags |= bits;
                return;
            }
            Window.Attributes.Flags &= ~bits;
        }
    }
}