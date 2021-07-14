using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using FastMobile.FXamarin.Core.FAndroid;

namespace FastMobile.Device.Droid
{
    [Activity(Theme = "@style/FastLaunchScreen.Splash", MainLauncher = true, NoHistory = true)]
    public class LaunchScreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            base.OnCreate(bundle);
            FUtility.InitExtrasByNotify(Intent);
            FNotificationCenter.NotifyNotificationTapped(Intent);
            StartActivity(new Intent(this, typeof(MainActivity)).AddFlags(ActivityFlags.ClearTop));
        }
    }
}