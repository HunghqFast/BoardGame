using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FastMobile.FXamarin.Core.FAndroid
{
    [Service]
    public class FLocalNotifications : IFLocalNotifications
    {
        public static Type TypeDrawable { get; }
        public static int NotificationIconId { get; set; }

        public static readonly string PackageName = Application.Context.PackageName;
        private NotificationManager Manager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

        static FLocalNotifications()
        {
            TypeDrawable = FNotificationCenter.MainActivityType.Assembly?.TypeByAssemply("Resource", "Drawable");
        }

        public void Show(string title, string body, int id = 0)
        {
            var channel = new NotificationChannel(PackageName, PackageName, NotificationImportance.High);
            Manager.CreateNotificationChannel(channel);

            var resultIntent = GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask).AddFlags(ActivityFlags.ClearTop);
            resultIntent.PutExtras(new Bundle());

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            var icon = TypeDrawable?.GetStaticFieldValue("iconNoti");

            var builder = new NotificationCompat.Builder(Application.Context)
            .SetChannelId(PackageName)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetAutoCancel(true)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(body))
            .SetColor(43693)
            .SetDefaults((int)NotificationDefaults.All)
            .SetSmallIcon(Convert.ToInt32(icon))
            .SetContentIntent(resultPendingIntent);

            Manager.Notify(id, builder.Build());
        }

        public static Intent GetLauncherActivity()
        {
            return Application.Context.PackageManager.GetLaunchIntentForPackage(PackageName);
        }

        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            var intent = CreateIntent(id);
            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Id = id;
            localNotification.NotifyTime = notifyTime;
            if (NotificationIconId != 0)
            {
                localNotification.IconId = NotificationIconId;
            }
            else
            {
                localNotification.IconId = Convert.ToInt32(TypeDrawable?.GetStaticFieldValue("iconNoti"));
            }

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
            var triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime);
            var alarmManager = GetAlarmManager();

            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        public void Cancel(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }

        private Intent CreateIntent(int id)
        {
            return new Intent(Application.Context, typeof(ScheduledAlarmHandler)).SetAction("LocalNotifierIntent" + id);
        }

        private AlarmManager GetAlarmManager()
        {
            return (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
        }

        private string SerializeNotification(LocalNotification notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }

        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }

        private class LocalNotification
        {
            public string Title;
            public string Body;
            public int Id;
            public int IconId;
            public DateTime NotifyTime;
        }

        [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
        public class ScheduledAlarmHandler : BroadcastReceiver
        {
            public const string LocalNotificationKey = "LocalNotification";

            public override void OnReceive(Context context, Intent intent)
            {
                var extra = intent.GetStringExtra(LocalNotificationKey);
                var notification = DeserializeNotification(extra);
                FInterface.IFLocalNotifications?.Show(notification.Title, notification.Body, notification.Id);
            }

            private LocalNotification DeserializeNotification(string notificationString)
            {
                var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
                using (var stringReader = new StringReader(notificationString))
                {
                    var notification = (LocalNotification)xmlSerializer.Deserialize(stringReader);
                    return notification;
                }
            }
        }
    }
}