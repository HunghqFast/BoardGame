using Android.App;
using FastMobile.FXamarin.Core.FAndroid;
using Firebase.Messaging;

namespace FastMobile.Device.Droid.FastFirebase
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class NotifyMessagingService : FNotifyMessagingServices
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
        }
    }
}