using Android.Content;
using Android.Gms.Maps.Model;
using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FAndroid;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(FMap), typeof(FMapRenderer))]

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FMapRenderer : MapRenderer
    {
        private List<FPin> Pins => (List<FPin>)Element.Pins;

        public FMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            if (pin is not FPin cpin)
                return base.CreateMarker(pin);
            if (cpin.Icon == null)
                return base.CreateMarker(pin);
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            UpdateIcon(marker, cpin.Icon);
            return marker;
        }

        private async void UpdateIcon(MarkerOptions marker, ImageSource source)
        {
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(await source.ToImageFromImageSource(Context)));
        }
    }
}