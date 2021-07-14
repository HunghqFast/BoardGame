using Android.Content;
using Android.OS;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FTimestampServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private readonly FMainActivity mainActivity;

        public FTimestampServiceConnection(FMainActivity activity)
        {
            IsConnected = false;
            Binder = null;
            mainActivity = activity;
        }

        public bool IsConnected { get; private set; }
        public FTimestampBinder Binder { get; private set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as FTimestampBinder;
            IsConnected = Binder != null;

            if (IsConnected) mainActivity.OnConnect();
            else mainActivity.OnDisconnect();
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
            mainActivity.OnDisconnect();
        }
    }
}