using Android.App;
using Android.Content;
using Android.OS;
using MediaManager;
using Syncfusion.XForms.Android.PopupLayout;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FSplashActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.SetFlags("Expander_Experimental", "FastRenderers_Experimental", "UseLegacyRenderers");
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
        }

        protected void InitAll(FormsAppCompatActivity context, Bundle bundle)
        {
            FormsMaps.Init(context, bundle);
            InitSyncfusion();
            CrossMediaManager.Current.Init(context);
        }

        protected void InitSyncfusion()
        {
            SfPopupLayoutRenderer.Init();
        }

        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;
            var channelID = Android.App.Application.Context.PackageName;
            var channel = new NotificationChannel(channelID, channelID, NotificationImportance.High)
            {
                Description = "Firebase Cloud Messages appear in this channel",
                LockscreenVisibility = NotificationVisibility.Public,
                Importance = NotificationImportance.Default,
            };
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public void ClearBadge()
        {
            try
            {
                NotificationManager manager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
                manager?.CancelAll();
            }
            catch { }
        }
    }
}