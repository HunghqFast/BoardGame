using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public abstract class FFingerprintBase : IFFingerprint
    {
        public async Task<FFingerprintAuthenticationResult> AuthenticateAsync(FAuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default)
        {
            if (authRequestConfig is null)
                throw new ArgumentNullException(nameof(authRequestConfig));

            var availability = await GetAvailabilityAsync(authRequestConfig.AllowAlternativeAuthentication);
            if (availability != FFingerprintAvailability.Available)
            {
                var status = availability == FFingerprintAvailability.Denied ?
                    FFingerprintAuthenticationResultStatus.Denied :
                    FFingerprintAuthenticationResultStatus.NotAvailable;

                return new FFingerprintAuthenticationResult { Status = status, ErrorMessage = availability.ToString() };
            }

            return await NativeAuthenticateAsync(authRequestConfig, cancellationToken);
        }

        public async Task<FFingerprintAuthenticationResult> AuthenticateAsyncWithoutDialog(FAuthenticationRequestConfigurationBase authRequestConfig, CancellationToken cancellationToken = default)
        {
            if (authRequestConfig is null)
                throw new ArgumentNullException(nameof(authRequestConfig));
            var availability = await GetAvailabilityAsync(authRequestConfig.AllowAlternativeAuthentication);
            if (availability != FFingerprintAvailability.Available)
            {
                var status = availability == FFingerprintAvailability.Denied ? FFingerprintAuthenticationResultStatus.Denied : FFingerprintAuthenticationResultStatus.NotAvailable;
                return new FFingerprintAuthenticationResult { Status = status, ErrorMessage = availability.ToString() };
            }

            return await NativeAuthenticateAsyncWithoutDialog(authRequestConfig, cancellationToken);
        }

        public bool IsAvailable(bool allowAlternativeAuthentication = false)
        {
            return GetAvailability(allowAlternativeAuthentication) == FFingerprintAvailability.Available;
        }

        public async Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false)
        {
            return await GetAvailabilityAsync(allowAlternativeAuthentication) == FFingerprintAvailability.Available;
        }

        public abstract Task<FFingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);

        public abstract Task<FAuthenticationType> GetAuthenticationTypeAsync();

        public abstract FAuthenticationType GetAuthenticationType();

        protected abstract Task<FFingerprintAuthenticationResult> NativeAuthenticateAsync(FAuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken);

        protected abstract Task<FFingerprintAuthenticationResult> NativeAuthenticateAsyncWithoutDialog(FAuthenticationRequestConfigurationBase authRequestConfig, CancellationToken cancellationToken);

        public abstract FFingerprintAvailability GetAvailability(bool allowAlternativeAuthentication = false);
    }
}