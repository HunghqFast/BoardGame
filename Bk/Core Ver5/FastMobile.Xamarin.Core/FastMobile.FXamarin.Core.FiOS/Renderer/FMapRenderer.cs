using FastMobile.FXamarin.Core;
using FastMobile.FXamarin.Core.FiOS;
using MapKit;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FMap), typeof(FMapRenderer))]

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FMapRenderer : MapRenderer
    {
        IList<Pin> Pins => ((FMap)Element).Pins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is MKUserLocation)
                return base.GetViewForAnnotation(mapView, annotation);

            var customPin = GetFPin(annotation as MKPointAnnotation);
            if (customPin == null)
                return base.GetViewForAnnotation(mapView, annotation);

            var view = base.GetViewForAnnotation(mapView, annotation);
            if (view != null) SetImage(view, customPin.Icon);
            return view;
        }

        private FPin GetFPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var x in Pins) if (x.Position == position) return x as FPin;
            return null;
        }

        private async void SetImage(MKAnnotationView view, ImageSource source)
        {
            view.Image = await source.ToImageFromImageSource();
        }
    }
}