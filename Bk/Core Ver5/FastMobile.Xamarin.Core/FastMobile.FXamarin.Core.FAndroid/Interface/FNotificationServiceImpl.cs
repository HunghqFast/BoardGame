using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Java.Util.Concurrent;
using System;
using System.Globalization;
using Application = Android.App.Application;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FNotificationServiceImpl : IFNotificationService
    {
        private readonly NotificationManager notificationManager;
        private readonly WorkManager workManager;

        public event FNotificationTappedEventHandler NotificationTapped;

        public void OnNotificationTapped(FNotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        public FNotificationServiceImpl()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                    return;

                notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager ?? throw new Exception("AndroidNotificationServiceNotFound");
                workManager = WorkManager.GetInstance(Application.Context);
            }
            catch { }
        }

        public void Cancel(int notificationId)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                    return;

                workManager?.CancelAllWorkByTag(notificationId.ToString(CultureInfo.CurrentCulture));
                notificationManager?.Cancel(notificationId);
            }
            catch { }
        }

        public void CancelAll()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                    return;

                workManager?.CancelAllWork();
                notificationManager?.CancelAll();
            }
            catch { }
        }

        public void Show(FNotificationRequest notificationRequest)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                    return;

                if (notificationRequest is null)
                    return;

                if (notificationRequest.NotifyTime.HasValue)
                    ShowLater(notificationRequest);
                else
                    ShowNow(notificationRequest);
            }
            catch { }
        }

        private void ShowLater(FNotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime is null || notificationRequest.NotifyTime.Value <= DateTime.Now)
                return;

            Cancel(notificationRequest.NotificationId);

            using var dataBuilder = new Data.Builder();
            dataBuilder.PutString(FChannel.EXTRA_RETURN_NOTIFICATION, FObjectSerializer.SerializeObject(notificationRequest));
            dataBuilder.PutString($"{FChannel.EXTRA_RETURN_NOTIFICATION}_Android", FObjectSerializer.SerializeObject(notificationRequest.Android));

            var requestBuilder = OneTimeWorkRequest.Builder.From<FScheduledNotificationWorker>();
            requestBuilder.AddTag(notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture));
            requestBuilder.SetInputData(dataBuilder.Build());
            var diff = (long)(notificationRequest.NotifyTime.Value - DateTime.Now).TotalMilliseconds;
            requestBuilder.SetInitialDelay(diff, TimeUnit.Milliseconds);

            var workRequest = requestBuilder.Build();
            workManager?.Enqueue(workRequest);
        }

        [Obsolete]
        private void ShowNow(FNotificationRequest request)
        {
            Cancel(request.NotificationId);

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
                request.Android.ChannelId = FChannel.DEFAULT_CHANNEL_ID;

            using var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);
            builder.SetContentTitle(request.Title);
            builder.SetContentText(request.Description);
            using (var bigTextStyle = new NotificationCompat.BigTextStyle())
            {
                var bigText = bigTextStyle.BigText(request.Description);
                builder.SetStyle(bigText);
            }
            builder.SetNumber(request.BadgeNumber);
            builder.SetAutoCancel(request.Android.AutoCancel);
            builder.SetOngoing(request.Android.Ongoing);

            if (string.IsNullOrWhiteSpace(request.Android.Group) == false)
            {
                builder.SetGroup(request.Android.Group);
                if (request.Android.IsGroupSummary)
                    builder.SetGroupSummary(true);
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)request.Android.Priority);

                var soundUri = FUtility.GetSoundUri(request.Sound);
                if (soundUri != null)
                    builder.SetSound(soundUri);
            }

            if (request.Android.VibrationPattern != null)
                builder.SetVibrate(request.Android.VibrationPattern);

            if (request.Android.ProgressBarMax.HasValue && request.Android.ProgressBarProgress.HasValue && request.Android.ProgressBarIndeterminate.HasValue)
                builder.SetProgress(request.Android.ProgressBarMax.Value, request.Android.ProgressBarProgress.Value, request.Android.ProgressBarIndeterminate.Value);

            if (request.Android.Color.HasValue)
                builder.SetColor(request.Android.Color.Value);

            builder.SetSmallIcon(GetIcon(request.Android.IconName));
            if (request.Android.TimeoutAfter.HasValue)
                builder.SetTimeoutAfter((long)request.Android.TimeoutAfter.Value.TotalMilliseconds);

            var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(string.IsNullOrEmpty(request.Android.PackageName) ? Application.Context.PackageName : request.Android.PackageName);
            //var notificationIntent = new Intent(Application.Context, FNotificationCenter.MainActivityType);
            notificationIntent.SetFlags(ActivityFlags.ClearTop);
            notificationIntent.PutExtra(FChannel.EXTRA_RETURN_DATA, request.ReturningData);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId, notificationIntent, PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(pendingIntent);

            var notification = builder.Build();
            if (Build.VERSION.SdkInt < BuildVersionCodes.O && request.Android.LedColor.HasValue)
                notification.LedARGB = request.Android.LedColor.Value;

            if (Build.VERSION.SdkInt < BuildVersionCodes.O && string.IsNullOrWhiteSpace(request.Sound))
                notification.Defaults = NotificationDefaults.All;

            notificationManager.CreateNotificationChannel(new NotificationChannel(request.Android.ChannelId, request.Android.ChannelId, NotificationImportance.High));
            notificationManager?.Notify(request.NotificationId, notification);
        }

        private static int GetIcon(string iconName)
        {
            var iconId = 0;
            if (string.IsNullOrWhiteSpace(iconName) == false)
                iconId = Application.Context.Resources.GetIdentifier(iconName, "drawable", Application.Context.PackageName);

            if (iconId != 0)
                return iconId;

            iconId = Application.Context.ApplicationInfo.Icon;
            if (iconId == 0)
                iconId = Application.Context.Resources.GetIdentifier("icon", "drawable", Application.Context.PackageName);

            return iconId;
        }
    }
}