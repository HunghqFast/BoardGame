using Android.OS;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FTimestampBinder : Binder
    {
        public FTimestampService Service { get; private set; }

        public FTimestampBinder(FTimestampService service)
        {
            Service = service;
        }
    }
}