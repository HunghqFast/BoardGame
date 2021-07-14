using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace FastMobile.FXamarin.Core.FAndroid
{
    [Service(Name = "com.Fast.BusinessOnline.FTimestampService", ForegroundServiceType = ForegroundService.TypeConnectedDevice, IsolatedProcess = false)]
    public class FTimestampService : Service
    {
        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new FTimestampBinder(this);
            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            Binder = null;
            base.OnDestroy();
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
        }
    }
}