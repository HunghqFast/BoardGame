using FastMobile.FXamarin.Core;
using System;

namespace FastMobile.Core
{
    public static class CConfigure
    {
        public static bool AutocompleteLookup
        {
            get => FPageLookup.AutoComplete;
            set => FPageLookup.AutoComplete = value;
        }

        public static bool UseLocalAuthen
        {
            get => FSetting.UseLocalAuthen;
        }

        public static DateTime TimeSleep
        {
            get => DateTime.Parse("FastMobile.Core.CConfigure.TimeSleep".GetCache());
            private set => value.SetCache("FastMobile.Core.CConfigure.TimeSleep");
        }

        public static void UpdateUseLocalAuthen(bool value)
        {
            FSetting.UpdateUseLocalAuthen(value);
        }

        public static void UpdateTimeSleep()
        {
            TimeSleep = DateTime.Now;
        }
    }
}