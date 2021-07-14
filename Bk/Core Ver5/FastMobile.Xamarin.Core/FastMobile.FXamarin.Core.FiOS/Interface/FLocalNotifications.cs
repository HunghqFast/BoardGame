using Foundation;
using System;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FLocalNotifications : IFLocalNotifications
    {
        private const string NotificationKey = "LocalNotificationKey";

        public void Show(string title, string body, int id = 0)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(.1, false);
                ShowUserNotification(title, body, id, trigger);
            }
            else
            {
                var notification = new UILocalNotification
                {
                    FireDate = NSDate.FromTimeIntervalSinceNow(1),
                    AlertTitle = title,
                    AlertBody = body,
                    AlertAction = title,
                    SoundName = UILocalNotification.DefaultSoundName,
                    UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
                };
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                UIApplication.SharedApplication.ApplicationIconBadgeNumber += 1;
            }
        }

        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponentsFromDateTime(notifyTime), false);
                ShowUserNotification(title, body, id, trigger);
            }
            else
            {
                var notification = new UILocalNotification
                {
                    FireDate = (NSDate)notifyTime,
                    AlertTitle = title,
                    AlertBody = body,
                    AlertAction = title,
                    SoundName = UILocalNotification.DefaultSoundName,
                    UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
                };
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                UIApplication.SharedApplication.ApplicationIconBadgeNumber += 1;
            }
        }

        public void Cancel(int id)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                    return;

                var itemList = new[]
                {
                    id.ToString(CultureInfo.CurrentCulture)
                };

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
            }
            catch { }
        }

        private void ShowUserNotification(string title, string body, int id, UNNotificationTrigger trigger)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                return;
            }
            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Body = body
            };

            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
            });
            UIApplication.SharedApplication.ApplicationIconBadgeNumber += 1;
        }

        private NSDateComponents GetNSDateComponentsFromDateTime(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }
    }
}