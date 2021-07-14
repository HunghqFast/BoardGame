using Android.App;
using Android.Content.PM;
using Android.OS;
using FastMobile.FXamarin.Core.FAndroid;
using Firebase;
using Xamarin.Essentials;
using Xamarin.Forms;
using FNotificationCenter = FastMobile.FXamarin.Core.FAndroid.FNotificationCenter;

namespace FastMobile.Device.Droid
{
    [Activity(Label = "Fast e-Invoice", Icon = "@drawable/appicon", Theme = "@style/MainTheme.Base", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : FMainActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Platform.Init(this, bundle);
            Forms.Init(this, bundle);
            FNotificationCenter.CreateNotificationChannel();
            Renderer();
            InitAll(this, bundle);
            FirebaseApp.InitializeApp(this);
            FNotificationCenter.Init(GetType());
            CreateNotificationChannel();
            LoadApplication(FormsApp ??= new App());
            new FAndroid().TransparentStatusBar();
        }

        private void Renderer()
        {
            FPageSearchRenderer.Init();
            FNavigationPageRenderer.Init();
        }
    }
}