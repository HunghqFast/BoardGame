namespace FastMobile.FXamarin.Core
{
    public class FFingerprintAuthenticationResult
    {
        public bool IsAuthenticated => Status == FFingerprintAuthenticationResultStatus.Succeeded;
        public FFingerprintAuthenticationResultStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}