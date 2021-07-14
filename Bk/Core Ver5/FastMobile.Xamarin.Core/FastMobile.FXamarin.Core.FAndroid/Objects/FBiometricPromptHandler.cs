using Android.Content;
using AndroidX.Biometric;
using Java.Lang;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FBiometricPromptHandler : BiometricPrompt.AuthenticationCallback, IDialogInterfaceOnClickListener
    {
        private readonly TaskCompletionSource<FFingerprintAuthenticationResult> taskCompletionSource;

        public FBiometricPromptHandler()
        {
            taskCompletionSource = new TaskCompletionSource<FFingerprintAuthenticationResult>();
        }

        public Task<FFingerprintAuthenticationResult> GetTask()
        {
            return taskCompletionSource.Task;
        }

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Succeeded });
        }

        public override void OnAuthenticationError(int errorCode, ICharSequence errString)
        {
            switch (errorCode)
            {
                case BiometricPrompt.ErrorCanceled:
                case BiometricPrompt.ErrorUserCanceled:
                case BiometricPrompt.ErrorNegativeButton:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Canceled, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;

                case BiometricPrompt.ErrorLockout:
                case BiometricPrompt.ErrorLockoutPermanent:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.TooManyAttempts, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;

                case BiometricPrompt.ErrorHwUnavailable:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.NotAvailable, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;

                default:
                    SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Unknown, ErrorMessage = errString != null ? errString.ToString() : string.Empty });
                    break;
            };
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            SetResultSafe(new FFingerprintAuthenticationResult { Status = FFingerprintAuthenticationResultStatus.Canceled });
        }

        private void SetResultSafe(FFingerprintAuthenticationResult result)
        {
            if (!(taskCompletionSource.Task.IsCanceled || taskCompletionSource.Task.IsCompleted || taskCompletionSource.Task.IsFaulted))
                taskCompletionSource.SetResult(result);
        }
    }
}