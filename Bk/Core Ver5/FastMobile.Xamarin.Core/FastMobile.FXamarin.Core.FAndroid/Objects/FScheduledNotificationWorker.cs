using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.Work;
using System;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FScheduledNotificationWorker : Worker
#pragma warning restore CA1812
    {
        public FScheduledNotificationWorker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            var serializedNotification = InputData.GetString(FChannel.EXTRA_RETURN_NOTIFICATION);

            if (string.IsNullOrWhiteSpace(serializedNotification))
                return Result.InvokeFailure();

            var serializedNotificationAndroid = InputData.GetString($"{FChannel.EXTRA_RETURN_NOTIFICATION}_Android");

            Task.Run(() =>
            {
                try
                {
                    Log.Info(Application.Context.PackageName, $"ScheduledNotificationWorker.DoWork: SerializedNotification [{serializedNotification}]");
                    var notification = FObjectSerializer.DeserializeObject<FNotificationRequest>(serializedNotification);
                    if (string.IsNullOrWhiteSpace(serializedNotificationAndroid) == false && FObjectSerializer.DeserializeObject<FAndroidOptions>(serializedNotificationAndroid) is FAndroidOptions options)
                        notification.Android = options;

                    if (notification.NotifyTime.HasValue && notification.Repeats != FNotificationRepeat.No)
                    {
                        switch (notification.Repeats)
                        {
                            case FNotificationRepeat.Daily:
                                notification.NotifyTime = notification.NotifyTime.Value.AddDays(1);
                                while (notification.NotifyTime <= DateTime.Now)
                                    notification.NotifyTime = notification.NotifyTime.Value.AddDays(1);
                                break;

                            case FNotificationRepeat.Weekly:
                                notification.NotifyTime = notification.NotifyTime.Value.AddDays(7);
                                while (notification.NotifyTime <= DateTime.Now)
                                    notification.NotifyTime = notification.NotifyTime.Value.AddDays(7);
                                break;
                        }
                        new FNotificationServiceImpl().Show(notification);
                    }

                    if (notification.NotifyTime != null && notification.NotifyTime.Value <= DateTime.Now.AddMinutes(-1))
                        return;

                    notification.NotifyTime = null;
                    new FNotificationServiceImpl().Show(notification);
                }
                catch { }
            });

            return Result.InvokeSuccess();
        }
    }
}