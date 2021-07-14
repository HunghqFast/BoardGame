using Foundation;
using LocalAuthentication;
using ObjCRuntime;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FFingerprint : FFingerprintBase
    {
        private LAContext context;

        public FFingerprint()
        {
            CreateLaContext();
        }

        protected override async Task<FFingerprintAuthenticationResult> NativeAuthenticateAsync(FAuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken)
        {
            var result = new FFingerprintAuthenticationResult();
            SetupContextProperties(authRequestConfig);

            Tuple<bool, NSError> resTuple;
            using (cancellationToken.Register(CancelAuthentication))
            {
                var policy = GetPolicy(authRequestConfig.AllowAlternativeAuthentication);
                resTuple = await context.EvaluatePolicyAsync(policy, authRequestConfig.Reason);
            }

            if (resTuple.Item1)
            {
                result.Status = FFingerprintAuthenticationResultStatus.Succeeded;
            }
            else
            {
                if (resTuple.Item2 == null)
                {
                    result.Status = FFingerprintAuthenticationResultStatus.UnknownError;
                    result.ErrorMessage = "";
                }
                else result = GetResultFromError(resTuple.Item2);
            }

            CreateNewContext();
            return result;
        }

        protected override Task<FFingerprintAuthenticationResult> NativeAuthenticateAsyncWithoutDialog(FAuthenticationRequestConfigurationBase authRequestConfig, CancellationToken cancellationToken)
        {
            throw new Exception("IOS can not Biometric authenticate without dialog");
        }

        public override FFingerprintAvailability GetAvailability(bool allowAlternativeAuthentication = false)
        {
            if (context == null) return FFingerprintAvailability.NoApi;

            var policy = GetPolicy(allowAlternativeAuthentication);
            if (context.CanEvaluatePolicy(policy, out var error)) return FFingerprintAvailability.Available;

            return (LAStatus)(int)error.Code switch
            {
                LAStatus.BiometryNotAvailable => IsDeniedError(error) ? FFingerprintAvailability.Denied : FFingerprintAvailability.NoSensor,
                LAStatus.BiometryNotEnrolled => FFingerprintAvailability.NoFingerprint,
                LAStatus.PasscodeNotSet => FFingerprintAvailability.NoFallback,
                _ => FFingerprintAvailability.Unknown,
            };
        }

        public override async Task<FFingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            await Task.Delay(1);
            return GetAvailability(allowAlternativeAuthentication);
        }

        public override async Task<FAuthenticationType> GetAuthenticationTypeAsync()
        {
            if (context == null) return FAuthenticationType.None;
            return AuthenticationType(await GetAvailabilityAsync(false));
        }

        public override FAuthenticationType GetAuthenticationType()
        {
            if (context == null) return FAuthenticationType.None;
            return AuthenticationType(GetAvailability(false));
        }

        private void SetupContextProperties(FAuthenticationRequestConfiguration authRequestConfig)
        {
            if (context.RespondsToSelector(new Selector("localizedFallbackTitle")))
                context.LocalizedFallbackTitle = authRequestConfig.iOSOptions.FallbackTitle;

            if (context.RespondsToSelector(new Selector("localizedCancelTitle")))
                context.LocalizedCancelTitle = authRequestConfig.CancelTitle;
        }

        private LAPolicy GetPolicy(bool allowAlternativeAuthentication)
        {
            return allowAlternativeAuthentication ? LAPolicy.DeviceOwnerAuthentication : LAPolicy.DeviceOwnerAuthenticationWithBiometrics;
        }

        private FFingerprintAuthenticationResult GetResultFromError(NSError error)
        {
            var result = new FFingerprintAuthenticationResult();

            switch ((LAStatus)(int)error.Code)
            {
                case LAStatus.AuthenticationFailed:
                    var description = error.Description;
                    if (description != null && description.Contains("retry limit exceeded"))
                        result.Status = FFingerprintAuthenticationResultStatus.TooManyAttempts;
                    else result.Status = FFingerprintAuthenticationResultStatus.Failed;
                    break;

                case LAStatus.UserCancel:
                case LAStatus.AppCancel:
                    result.Status = FFingerprintAuthenticationResultStatus.Canceled;
                    break;

                case LAStatus.UserFallback:
                    result.Status = FFingerprintAuthenticationResultStatus.FallbackRequested;
                    break;

                case LAStatus.TouchIDLockout:
                    result.Status = FFingerprintAuthenticationResultStatus.TooManyAttempts;
                    break;

                case LAStatus.BiometryNotAvailable:
                    result.Status = IsDeniedError(error) ? FFingerprintAuthenticationResultStatus.Denied : FFingerprintAuthenticationResultStatus.NotAvailable;
                    break;

                default:
                    result.Status = FFingerprintAuthenticationResultStatus.UnknownError;
                    break;
            }

            result.ErrorMessage = error.LocalizedDescription;
            return result;
        }

        private void CancelAuthentication()
        {
            CreateNewContext();
        }

        private void CreateNewContext()
        {
            if (context != null)
            {
                if (context.RespondsToSelector(new Selector("invalidate")))
                    context.Invalidate();
                context.Dispose();
            }

            CreateLaContext();
        }

        private void CreateLaContext()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                return;
            if (Class.GetHandle(typeof(LAContext)) == IntPtr.Zero)
                return;

            context = new LAContext();
        }

        private bool IsDeniedError(NSError error)
        {
            return !string.IsNullOrEmpty(error.Description) && error.Description.ToLower().Contains("denied");
        }

        private FAuthenticationType AuthenticationType(FFingerprintAvailability availability)
        {
            if (context.RespondsToSelector(new Selector("biometryType")))
            {
                return context.BiometryType switch
                {
                    LABiometryType.None => FAuthenticationType.None,
                    LABiometryType.TouchId => FAuthenticationType.Fingerprint,
                    LABiometryType.FaceId => FAuthenticationType.Face,
                    _ => FAuthenticationType.None,
                };
            }

            if (availability == FFingerprintAvailability.NoApi || availability == FFingerprintAvailability.NoSensor || availability == FFingerprintAvailability.Unknown)
                return FAuthenticationType.None;
            return FAuthenticationType.Fingerprint;
        }
    }
}