using System;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FUtcTimestamper : IFGetTimestamp
    {
        private readonly DateTime startTime;

        public FUtcTimestamper()
        {
            startTime = DateTime.UtcNow;
        }

        public string GetFormattedTimestamp()
        {
            TimeSpan duration = DateTime.UtcNow.Subtract(startTime);
            return $"Service started at {startTime} ({duration:c} ago).";
        }
    }
}