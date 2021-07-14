using System.Threading;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFFingerprint
    {
        bool IsAvailable(bool allowAlternativeAuthentication = false);

        Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false);

        FAuthenticationType GetAuthenticationType();

        Task<FAuthenticationType> GetAuthenticationTypeAsync();

        FFingerprintAvailability GetAvailability(bool allowAlternativeAuthentication = false);

        Task<FFingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);

        Task<FFingerprintAuthenticationResult> AuthenticateAsync(FAuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default);

        Task<FFingerprintAuthenticationResult> AuthenticateAsyncWithoutDialog(FAuthenticationRequestConfigurationBase authRequestConfig, CancellationToken cancellationToken = default);
    }
}