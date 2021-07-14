using System;

namespace FastMobile.FXamarin.Core
{
    public class FNotificationRequest
    {
        public FAndroidOptions Android { get; set; } = new FAndroidOptions();
        public int BadgeNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public int NotificationId { get; set; }
        public DateTime? NotifyTime { get; set; }
        public FNotificationRepeat Repeats { get; set; } = FNotificationRepeat.No;
        public string ReturningData { get; set; } = string.Empty;
        public string Sound { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}