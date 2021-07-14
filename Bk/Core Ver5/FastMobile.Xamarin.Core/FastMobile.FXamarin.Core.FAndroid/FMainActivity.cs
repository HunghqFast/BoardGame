using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Syncfusion.XForms.Android.PopupLayout;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FMainActivity : FormsAppCompatActivity
    {
        public const int RequestLocationId = 0;
        public readonly string[] LocationPermissions = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };

        protected virtual FApplication FormsApp { get; set; }
        //protected virtual FTimestampServiceConnection ServiceConnection { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.SetFlags("Expander_Experimental", "FastRenderers_Experimental", "UseLegacyRenderers");
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
        }

        protected override void OnStart()
        {
            //ServiceConnection ??= new FTimestampServiceConnection(this);
            base.OnStart();
            //DoBindService();
        }

        protected override void OnDestroy()
        {
            //DoUnBindService();
            base.OnDestroy();
        }

        protected void InitAll(FormsAppCompatActivity context, Bundle bundle)
        {
            FormsMaps.Init(context, bundle);
            InitSyncfusion();
        }

        protected void InitSyncfusion()
        {
            SfPopupLayoutRenderer.Init(this);
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public virtual void OnConnect()
        {
        }

        public virtual void OnDisconnect()
        {
            FormsApp?.OnFinish();
        }

        //void DoBindService()
        //{
        //    BindService(new Intent(this, typeof(FTimestampService)), ServiceConnection, Bind.AutoCreate);
        //}

        //void DoUnBindService()
        //{
        //    UnbindService(ServiceConnection);
        //}
    }
}