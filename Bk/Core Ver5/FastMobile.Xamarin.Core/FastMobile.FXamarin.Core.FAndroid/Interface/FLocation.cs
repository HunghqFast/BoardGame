using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using AndroidLocation = Android.Locations.Location;

namespace FastMobile.FXamarin.Core.FAndroid
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

        private LocationManager LocationManager => (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
        private readonly string[] ignoredProviders;
        private LocationManager UpdatingManager;
        private FLocationListener LocationListener;

        public FLocation()
        {
            ignoredProviders = new string[] { LocationManager.PassiveProvider, "local_database" };
        }

        public bool IsGpsEnable()
        {
            return IsGpsEnable(LocationManager);
        }

        public void OpenSetting()
        {
            try
            {
                var intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (ActivityNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void StopUpdating()
        {
            if (LocationListener != null) UpdatingManager?.RemoveUpdates(LocationListener);
            IsUpdating = false;
        }

        public async Task<bool> StartUpdating(FLocationAccuracy accuracy, CancellationToken cancellation)
        {
            try
            {
                await Task.Delay(1);
                UpdatingManager = LocationManager;
                var providerInfo = FUtility.GetBestProvider(UpdatingManager, FLocationAccuracy.Default);

                if (string.IsNullOrEmpty(providerInfo.Provider))
                    return false;

                var requested = false;
                var tcs = new TaskCompletionSource<bool>();
                cancellation.Register(Cancel);
                LocationListener = new FLocationListener(UpdatingManager, providerInfo.Accuracy, new List<string> { LocationManager.GpsProvider }, false);
                LocationListener.LocationChangedHandler = OnLocationChanged;
                LocationListener.GPSEnabledHandler = OnGPSEnabledHandler;
                LocationListener.LocationHandler = HandleLocation;
                UpdatingManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 100, LocationListener, Looper.MyLooper() ?? Looper.MainLooper);

                var reuslt = await tcs.Task;
                return reuslt;

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
                    if (LocationListener.BestLocation == null)
                    {
                        IsUpdating = false;
                        tcs.TrySetResult(IsUpdating);
                        return;
                    }
                    IsUpdating = true;
                    tcs.TrySetResult(IsUpdating);
                }

                void OnLocationChanged(Position e, double accuracy)
                {
                    LocationChanged?.Invoke(this, new FObjectPropertyArgs<(Position Position, double Accuracy)>((e, accuracy)));
                }

                void OnGPSEnabledHandler(bool e)
                {
                    GPSEnabled?.Invoke(this, new FObjectPropertyArgs<bool>(e));
                }
            }
            catch { return false; }
        }

        public async Task<(Position Position, double Accuracy)> GetCurrentPosition(FLocationAccuracy accuracy, CancellationToken cancellation, bool requirePermission)
        {
            try
            {
                if (requirePermission && !await Core.FPermissions.HasPermission<Permissions.LocationWhenInUse>(true))
                    return default;
                var locationManager = LocationManager;

                if (!IsGpsEnable(locationManager))
                    return default;

                var enabledProviders = locationManager.GetProviders(true);
                var hasProviders = enabledProviders.Any(p => !ignoredProviders.Contains(p));

                if (!hasProviders)
                    return default;

                var providerInfo = FUtility.GetBestProvider(locationManager, accuracy);

                if (string.IsNullOrEmpty(providerInfo.Provider))
                    return default;

                var providers = new List<string>(locationManager.GetProviders(false));
                if (!providers.Contains(LocationManager.GpsProvider))
                    providers.Add(LocationManager.GpsProvider);
                if (!providers.Contains(LocationManager.NetworkProvider))
                    providers.Add(LocationManager.NetworkProvider);
                if (providers.Count == 0)
                    providers.Add(providerInfo.Provider);

                var requested = false;
                var tcs = new TaskCompletionSource<(Position, double)>();
                var listener = new FLocationListener(locationManager, providerInfo.Accuracy, providers, true);
                listener.LocationHandler = HandleLocation;
                cancellation.Register(Cancel);

                var looper = Looper.MyLooper() ?? Looper.MainLooper;
                providers.ForEach(x => locationManager.RequestLocationUpdates(x, 0, 0, listener, looper));

                var reuslt = await tcs.Task;
                return reuslt;

                void HandleLocation(Position location, double accuracy)
                {
                    requested = true;
                    Remove();
                    tcs.TrySetResult((location, accuracy));
                }

                void Cancel()
                {
                    if (requested) return;
                    Remove();
                    if (listener.BestLocation == null)
                    {
                        tcs.TrySetResult((default, 0));
                        return;
                    }
                    tcs.TrySetResult((new Position(listener.BestLocation.Latitude, listener.BestLocation.Longitude), listener.BestLocation.Accuracy));
                }

                void Remove()
                {
                    locationManager.RemoveUpdates(listener);
                }
            }
            catch { return default; }
        }

        private bool IsGpsEnable(LocationManager manager)
        {
            return manager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        private class FLocationListener : Java.Lang.Object, ILocationListener
        {
            private readonly object locationSync = new();
            private readonly float desiredAccuracy;
            private readonly HashSet<string> activeProviders;
            private readonly bool isReq = false;
            private bool wasRaised = false;

            public AndroidLocation BestLocation { get; set; }
            public Action<Position, double> LocationHandler { get; set; }
            public Action<Position, double> LocationChangedHandler { get; set; }
            public Action<bool> GPSEnabledHandler { get; set; }

            public FLocationListener(LocationManager manager, float accuracy, IEnumerable<string> providers, bool isRequest)
            {
                desiredAccuracy = accuracy;
                activeProviders = new HashSet<string>(providers);
                isReq = isRequest;

                foreach (var provider in activeProviders)
                {
                    var location = manager.GetLastKnownLocation(provider);
                    if (location != null && location.IsBetterLocation(BestLocation))
                        BestLocation = location;
                }
            }

            public virtual void OnLocationChanged(AndroidLocation location)
            {
                if (location.Accuracy <= desiredAccuracy)
                {
                    if (isReq && wasRaised)
                        return;
                    wasRaised = true;

                    var position = new Position(location.Latitude, location.Longitude);
                    LocationChangedHandler?.Invoke(position, Convert.ToDouble(location.Accuracy));
                    LocationHandler?.Invoke(position, Convert.ToDouble(location.Accuracy));
                    return;
                }

                lock (locationSync)
                {
                    if (location.IsBetterLocation(BestLocation))
                        BestLocation = location;
                }
            }

            public virtual void OnProviderDisabled(string provider)
            {
                GPSEnabledHandler?.Invoke(false);
            }

            public virtual void OnProviderEnabled(string provider)
            {
                GPSEnabledHandler?.Invoke(true);
            }

            public virtual void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
                switch (status)
                {
                    case Availability.Available:
                        OnProviderEnabled(provider);
                        break;

                    case Availability.OutOfService:
                        OnProviderDisabled(provider);
                        break;
                }
            }
        }
    }
}