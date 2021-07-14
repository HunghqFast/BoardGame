using Foundation;
using System;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FNotificationServiceImpl : IFNotificationService
    {
        public event FNotificationTappedEventHandler NotificationTapped;

        public void OnNotificationTapped(FNotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        public void Cancel(int notificationId)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                    return;

                var itemList = new[]
                {
                    notificationId.ToString(CultureInfo.CurrentCulture)
                };

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
            }
            catch { }
        }

        public void CancelAll()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                    return;

                UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            }
            catch { }
        }

        public async void Show(FNotificationRequest notificationRequest)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                    return;

                if (notificationRequest is null)
                    return;

                var allowed = await FNotificationCenter.AskPermissionAsync().ConfigureAwait(false);
                if (allowed == false)
                    return;

                var userInfoDictionary = new NSMutableDictionary();

                if (string.IsNullOrWhiteSpace(notificationRequest.ReturningData) == false)
                {
                    using var returningData = new NSString(notificationRequest.ReturningData);
                    userInfoDictionary.SetValueForKey(string.IsNullOrWhiteSpace(notificationRequest.ReturningData) ? NSString.Empty : returningData, new NSString(FChannel.EXTRA_RETURN_DATA));
                }

                using var content = new UNMutableNotificationContent
                {
                    Title = notificationRequest.Title,
                    Body = notificationRequest.Description,
                    Badge = notificationRequest.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default
                };

                if (string.IsNullOrWhiteSpace(notificationRequest.Sound) == false)
                    content.Sound = UNNotificationSound.GetSound(notificationRequest.Sound);

                var repeats = notificationRequest.Repeats != FNotificationRepeat.No;
                using var notifyTime = GetNsDateComponentsFromDateTime(notificationRequest);
                using var trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, repeats);

                var request = UNNotificationRequest.FromIdentifier(notificationRequest.NotificationId.ToString(), content, trigger);

                await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request).ConfigureAwait(false);
            }
            catch { }
        }

        private static NSDateComponents GetNsDateComponentsFromDateTime(FNotificationRequest notificationRequest)
        {
            var dateTime = notificationRequest.NotifyTime ?? DateTime.Now.AddSeconds(1);
            return notificationRequest.Repeats switch
            {
                FNotificationRepeat.Daily => new NSDateComponents
                {
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                FNotificationRepeat.Weekly => new NSDateComponents
                {
                    Weekday = (int)dateTime.DayOfWeek + 1,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                FNotificationRepeat.No => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                _ => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                }
            };
        }
    }
}