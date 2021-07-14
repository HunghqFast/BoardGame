using CoreLocation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FLocation : BindableObject, IFLocation
    {
        public static readonly BindableProperty IsUpdatingProperty = BindableProperty.Create("IsUpdating", typeof(bool), typeof(FLocation), false);

        public bool IsUpdating
        {
            get => (bool)GetValue(IsUpdatingProperty);
            private set => SetValue(IsUpdatingProperty, value);
        }

        public Action<bool> Updated { get; set; }

        public event EventHandler<FObjectPropertyArgs<(Position Position, double Accuracy)>> LocationChanged;

        public event EventHandler<FObjectPropertyArgs<bool>> GPSEnabled;

        private CLLocationManager LocationManager;
        private FLocationListener LocationListener;

        public FLocation()
        {
        }

        public bool IsGpsEnable()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public async void OpenSetting()
        {
            await Launcher.OpenAsync(UIApplication.OpenSettingsUrlString);
        }

        public void StopUpdating()
        {
            if (LocationManager != null) LocationManager.StopUpdatingLocation();
            IsUpdating = false;
        }

        public async Task<bool> StartUpdating(FLocationAccuracy accuracy, CancellationToken cancellation)
        {
            try
            {
                var requested = false;
                var tcs = new TaskCompletionSource<bool>();
                cancellation.Register(Cancel);

                LocationManager?.StopUpdatingLocation();
                LocationListener?.Remove();
                IsUpdating = false;
                LocationManager = await Device.InvokeOnMainThreadAsync(() => new CLLocationManager());
                LocationListener = new FLocationListener(false);

                LocationManager.DesiredAccuracy = accuracy.ToPlatformDesiredAccuracy();
                LocationManager.Delegate = LocationListener;
                LocationListener.Requested = HandleLocation;
                LocationListener.Updated = OnLocationsUpdated;
                LocationListener.Authored = OnAuthorizationChanged;

                LocationManager.StartUpdatingLocation();

                var result = await tcs.Task;
                return result;

                void HandleLocation(Position location, double accuracy)
                {
                    requested = true;
                    IsUpdating = location != default;
                    tcs.TrySetResult(IsUpdating);
                    Updated?.Invoke(IsUpdating);
                }

                void Cancel()
                {
                    if (requested) return;
                    IsUpdating = false;
                    tcs.TrySetResult(IsUpdating);
                }

                void OnLocationsUpdated(Position position, double accuracy)
                {
                    LocationChanged?.Invoke(this, new FObjectPropertyArgs<(Position, double)>((position, accuracy)));
                }

                void OnAuthorizationChanged(bool value)
                {
                    GPSEnabled?.Invoke(this, new FObjectPropertyArgs<bool>(value));
                }
            }
            catch { return false; }
        }

        public async Task<(Position Position, double Accuracy)> GetCurrentPosition(FLocationAccuracy accuracy, CancellationToken cancellation, bool requirePermission)
        {
            try
            {
                var manager = await Device.InvokeOnMainThreadAsync(() => new CLLocationManager());
                if (requirePermission)
                    manager.RequestWhenInUseAuthorization();

                if (requirePermission && !await Core.FPermissions.HasPermission<Permissions.LocationWhenInUse>(true))
                    return default;

                if (!IsGpsEnable())
                    return default;

                var requested = false;
                var tcs = new TaskCompletionSource<(Position Position, double Accuracy)>(manager);
                cancellation.Register(Cancel);

                var listener = new FLocationListener(true);
                listener.Requested += HandleLocation;

                manager.DesiredAccuracy = accuracy.ToPlatformDesiredAccuracy();
                manager.Delegate = listener;
                manager.PausesLocationUpdatesAutomatically = false;
                manager.StartUpdatingLocation();

                var result = await tcs.Task;
                return result;

                void HandleLocation(Position location, double accuracy)
                {
                    requested = true;
                    manager.StopUpdatingLocation();
                    tcs.TrySetResult((location, accuracy));
                }

                void Cancel()
                {
                    if (requested) return;
                    manager.StopUpdatingLocation();
                    tcs.TrySetResult((default, 0));
                }
            }
            catch { return default; }
        }

        private class FLocationListener : CLLocationManagerDelegate
        {
            internal Action<Position, double> Requested { get; set; }
            internal Action<Position, double> Updated { get; set; }
            internal Action<bool> Authored { get; set; }

            private bool wasRaised = false;
            private readonly bool isReq;

            public FLocationListener(bool isRequest)
            {
                isReq = isRequest;
            }

            public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
            {
                var location = locations?.LastOrDefault();
                if (location == null)
                    return;

                if (isReq && wasRaised)
                    return;
                wasRaised = true;

                Updated?.Invoke(new Position(location.Coordinate.Latitude, location.Coordinate.Longitude), location.HorizontalAccuracy);
                Requested?.Invoke(new Position(location.Coordinate.Latitude, location.Coordinate.Longitude), location.HorizontalAccuracy);
            }

            public override void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
            {
                Authored?.Invoke(status == CLAuthorizationStatus.Authorized || status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.AuthorizedWhenInUse);
            }

            public void Remove()
            {
                Requested = null;
                Authored = null;
            }

            public override bool ShouldDisplayHeadingCalibration(CLLocationManager manager) => false;
        }
    }
}