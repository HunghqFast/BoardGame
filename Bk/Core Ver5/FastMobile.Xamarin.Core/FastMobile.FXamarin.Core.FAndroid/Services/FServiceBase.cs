using Android.App;
using Android.Content;
using Android.OS;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FServiceBase : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}