using Android.App;
using Android.Graphics;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FNotificationChannelRequest
    {
        public NotificationImportance Importance { get; set; } = NotificationImportance.Default;

        public string Id { get; set; } = FChannel.DEFAULT_CHANNEL_ID;

        public string Name { get; set; } = "General";

        public string Description { get; set; }

        public string Group { get; set; }

        public Color LightColor { get; set; }

        public string Sound { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        public long[] VibrationPattern { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public NotificationVisibility LockscreenVisibility { get; set; } = NotificationVisibility.Secret;

        public bool ShowBadge { get; set; } = true;

        public bool EnableLights { get; set; } = true;

        public bool EnableVibration { get; set; } = true;
    }
}