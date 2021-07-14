namespace FastMobile.FXamarin.Core
{
    public class FAuthenticationRequestConfiguration : FAuthenticationRequestConfigurationBase
    {
        public string Title { get; set; }
        public string FontFamily { get; set; }
        public string Reason { get; set; }
        public string CancelTitle { get; set; }
        public FAuthenticationHelpTexts HelpTexts { get; } = new FAuthenticationHelpTexts();
        public bool ConfirmationRequired { get; set; } = true;
        public FAndroidOptions AndroidOptions { get; } = new FAndroidOptions();
        public FiOSOptions iOSOptions { get; } = new FiOSOptions();

        public FAuthenticationRequestConfiguration(string title, string reason, string cancelTitle)
        {
            Title = title;
            Reason = reason;
            CancelTitle = cancelTitle;
        }

        public class FAndroidOptions
        {
            public string SubTitle { get; set; } = string.Empty;

            public FAndroidOptions()
            {
            }

            public FAndroidOptions(string subTitle)
            {
                SubTitle = subTitle;
            }
        }

        public class FiOSOptions
        {
            public string FallbackTitle { get; set; } = string.Empty;

            public FiOSOptions()
            {
            }

            public FiOSOptions(string fallBackTitle)
            {
                FallbackTitle = fallBackTitle;
            }
        }
    }
}