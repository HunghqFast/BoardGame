using System;

namespace FastMobile.FXamarin.Core
{
    public class FAuthenticationRequestConfigurationBase
    {
        public bool AllowAlternativeAuthentication { get; set; } = false;
        public Action AfterPromt { get; set; }
        public Action AfterPromtWhenSuccess { get; set; }
        public Action BeforePromt { get; set; }
    }
}