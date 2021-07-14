using AndroidX.Biometric;
using AndroidX.Core.Hardware.Fingerprint;
using Java.Lang;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FAndroid
{
    [System.Obsolete]
    public class FFingerprintManagerCompatHandler : FingerprintManagerCompat.AuthenticationCallback
    {
        private readonly TaskCompletionSource<FFingerprintAuthenticationResult> taskCompletionSource;

        public FFingerprintManagerCompatHandler()
        {
            taskCompletionSource = new TaskCompletionSource<FFingerprintAuthenticationResult>();
        }

        public Task<FFingerprintAuthenticationResult> GetTask()
        {
            return taskCompletionSource.Task;
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Succeeded });
        }

        public override void OnAuthenticationError(int errorCode, ICharSequence errString)
        {
            switch (errorCode)
            {
                case BiometricPrompt.ErrorCanceled:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Canceled, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;

                case BiometricPrompt.ErrorLockout:
                case BiometricPrompt.ErrorLockoutPermanent:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.TooManyAttempts, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;

                default:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Failed, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;
            };
        }

        public override void OnAuthenticationFailed()
        {
            SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Failed, ErrorMessage = string.Empty });
        }

        private void SetResultSafe(FFingerprintAuthenticationResult result)
        {
            if (!(taskCompletionSource.Task.IsCanceled || taskCompletionSource.Task.IsCompleted || taskCompletionSource.Task.IsFaulted))
                taskCompletionSource.SetResult(result);
        }
    }
}