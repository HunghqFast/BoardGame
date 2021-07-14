using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Biometric;
using AndroidX.Core.Hardware.Fingerprint;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Application = Android.App.Application;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FFingerprint : FFingerprintBase
    {
        private readonly BiometricManager manager;

        public FFingerprint()
        {
            manager = BiometricManager.From(Application.Context);
        }

        public override async Task<FAuthenticationType> GetAuthenticationTypeAsync()
        {
            var availability = await GetAvailabilityAsync(false);
            if (availability == FFingerprintAvailability.NoFingerprint || availability == FFingerprintAvailability.NoPermission || availability == FFingerprintAvailability.Available)
                return FAuthenticationType.Fingerprint | FAuthenticationType.Face;
            return FAuthenticationType.None;
        }

        public override FAuthenticationType GetAuthenticationType()
        {
            var availability = GetAvailability(false);
            if (availability == FFingerprintAvailability.NoFingerprint || availability == FFingerprintAvailability.NoPermission || availability == FFingerprintAvailability.Available)
                return FAuthenticationType.Face | FAuthenticationType.Fingerprint;
            return FAuthenticationType.None;
        }

        public override FFingerprintAvailability GetAvailability(bool allowAlternativeAuthentication = false)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return FFingerprintAvailability.NoApi;

            var biometricAvailability = GetBiometricAvailability();
            if (biometricAvailability == FFingerprintAvailability.Available || !allowAlternativeAuthentication)
                return biometricAvailability;

            var context = Application.Context;

            try
            {
                var manager = (KeyguardManager)context.GetSystemService(Android.Content.Context.KeyguardService);
                if (manager.IsDeviceSecure)
                {
                    return FFingerprintAvailability.Available;
                }

                return FFingerprintAvailability.NoFallback;
            }
            catch
            {
                return FFingerprintAvailability.NoFallback;
            }
        }

        public override async Task<FFingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            await Task.Delay(1);
            return GetAvailability(allowAlternativeAuthentication);
        }

        private FFingerprintAvailability GetBiometricAvailability()
        {
            var context = Application.Context;

            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted && context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
                return FFingerprintAvailability.NoPermission;

            return manager.CanAuthenticate() switch
            {
                BiometricManager.BiometricErrorNoHardware => FFingerprintAvailability.NoSensor,
                BiometricManager.BiometricErrorHwUnavailable => FFingerprintAvailability.Unknown,
                BiometricManager.BiometricErrorNoneEnrolled => FFingerprintAvailability.NoFingerprint,
                BiometricManager.BiometricSuccess => FFingerprintAvailability.Available,
                _ => FFingerprintAvailability.Unknown,
            };
        }

        protected override async Task<FFingerprintAuthenticationResult> NativeAuthenticateAsync(FAuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(authRequestConfig.Title))
                throw new ArgumentException("Title must not be null or empty on Android.", nameof(authRequestConfig.Title));

            if (!(Platform.CurrentActivity is FragmentActivity))
                throw new InvalidOperationException($"Expected current activity to be '{typeof(FragmentActivity).FullName}' but was '{Platform.CurrentActivity?.GetType().FullName}'. " + "You need to use AndroidX. Have you installed Xamarin.AndroidX.Migration in your Android App project!?");

            try
            {
                var cancel = string.IsNullOrWhiteSpace(authRequestConfig.CancelTitle) ? Application.Context.GetString(Android.Resource.String.Cancel) : authRequestConfig.CancelTitle;

                var handler = new FBiometricPromptHandler();
                var builder = new BiometricPrompt.PromptInfo.Builder()
                    .SetTitle(authRequestConfig.Title)
                    .SetSubtitle(authRequestConfig.AndroidOptions.SubTitle)
                    .SetDescription(authRequestConfig.Reason)
                    .SetConfirmationRequired(authRequestConfig.ConfirmationRequired);

                if (authRequestConfig.AllowAlternativeAuthentication)
                    builder = builder.SetDeviceCredentialAllowed(authRequestConfig.AllowAlternativeAuthentication);
                else builder = builder.SetNegativeButtonText(cancel);

                var activity = (FragmentActivity)Platform.CurrentActivity;
                using var dialog = new BiometricPrompt(activity, Executors.NewSingleThreadExecutor(), handler);
                await using (cancellationToken.Register(dialog.CancelAuthentication))
                {
                    authRequestConfig.BeforePromt?.Invoke();
                    dialog.Authenticate(builder.Build());
                    var result = await handler.GetTask();
                    authRequestConfig.AfterPromt?.Invoke();
                    TryReleaseLifecycleObserver(activity, dialog);
                    return result;
                }
            }
            catch (Exception e)
            {
                return new FFingerprintAuthenticationResult
                {
                    Status = FFingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = e.Message
                };
            }
        }

        protected override async Task<FFingerprintAuthenticationResult> NativeAuthenticateAsyncWithoutDialog(FAuthenticationRequestConfigurationBase authRequestConfig, CancellationToken cancellationToken)
        {
            try
            {
                var cryptoHelper = new FCryptoObjectHelper();
                var cancellationSignal = new AndroidX.Core.OS.CancellationSignal();
                var fingerprintManager = FingerprintManagerCompat.From(Platform.AppContext);
                var authenticationCallback = new FFingerprintManagerCompatHandler();

                await using (cancellationToken.Register(cancellationSignal.Cancel))
                {
                    authRequestConfig.BeforePromt?.Invoke();
                    fingerprintManager.Authenticate(cryptoHelper.BuildCryptoObject(), 0, cancellationSignal, authenticationCallback, null);
                    var result = await authenticationCallback.GetTask();
                    if (result.IsAuthenticated) authRequestConfig.AfterPromtWhenSuccess?.Invoke();
                    else authRequestConfig.AfterPromt?.Invoke();
                    return result;
                }
            }
            catch (Exception e)
            {
                return new FFingerprintAuthenticationResult
                {
                    Status = FFingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = e.Message
                };
            }
        }

        private static void TryReleaseLifecycleObserver(ILifecycleOwner lifecycleOwner, BiometricPrompt dialog)
        {
            var promptClass = Java.Lang.Class.FromType(dialog.GetType());
            var fields = promptClass.GetDeclaredFields();
            var lifecycleObserverField = fields?.FirstOrDefault(f => f.Name == "mLifecycleObserver");

            if (lifecycleObserverField is null)
                return;

            lifecycleObserverField.Accessible = true;
            var lastLifecycleObserver = lifecycleObserverField.Get(dialog).JavaCast<ILifecycleObserver>();
            var lifecycle = lifecycleOwner.Lifecycle;

            if (lastLifecycleObserver is null || lifecycle is null)
                return;

            lifecycle.RemoveObserver(lastLifecycleObserver);
        }
    }
}