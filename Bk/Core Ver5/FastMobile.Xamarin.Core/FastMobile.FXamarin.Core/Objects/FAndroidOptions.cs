using System;

namespace FastMobile.FXamarin.Core
{
    public class FAndroidOptions
    {
        public bool AutoCancel { get; set; } = true;

        public string ChannelId { get; set; } = FChannel.DEFAULT_CHANNEL_ID;

        public string Group { get; set; }

        public bool IsGroupSummary { get; set; }

        public int? Color { get; set; }

        public string IconName { get; set; } = "iconNoti";

        public int? LedColor { get; set; }

        public bool Ongoing { get; set; } = false;

        public FNotificationPriority Priority { get; set; } = FNotificationPriority.High;

        public bool? ProgressBarIndeterminate { get; set; }

        public int? ProgressBarMax { get; set; }

        public int? ProgressBarProgress { get; set; }

        public TimeSpan? TimeoutAfter { get; set; }

        public string PackageName { get; set; } = string.Empty;

#pragma warning disable CA1819
        public long[] VibrationPattern { get; set; }
#pragma warning restore CA1819
    }
}