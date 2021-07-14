using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using System;
using Application = Android.App.Application;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FNotificationCenter : Core.FNotificationCenter
    {
        public static Type MainActivityType { get; private set; }

        public static void Init(Type mainType)
        {
            try
            {
                Current = new FNotificationServiceImpl();
                MainActivityType = mainType;
            }
            catch { }
        }

        public static void NotifyNotificationTapped(Intent intent, IFNotificationService service = null)
        {
            try
            {
                if (intent is null)
                    return;

                if (intent.HasExtra(FChannel.EXTRA_RETURN_DATA))
                {
                    if (service is IFNotificationService)
                    {
                        service?.OnNotificationTapped(new FNotificationTappedEventArgs(intent?.GetStringExtra(FChannel.EXTRA_RETURN_DATA)));
                        return;
                    }
                    Current?.OnNotificationTapped(new FNotificationTappedEventArgs(intent?.GetStringExtra(FChannel.EXTRA_RETURN_DATA)));
                    return;
                }

                var dic = FUtility.GetIntentNotify(intent);
                if (dic == null)
                    return;
                if (service is IFNotificationService)
                {
                    service?.OnNotificationTapped(new FNotificationTappedEventArgs(dic.ToJson()));
                    return;
                }
                Current?.OnNotificationTapped(new FNotificationTappedEventArgs(dic.ToJson()));
            }
            catch { }
        }

        public static void CreateNotificationChannelGroup(FNotificationChannelGroupRequest request)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager notificationManager)
                return;

            if (request is null || string.IsNullOrWhiteSpace(request.Group) || string.IsNullOrWhiteSpace(request.Name))
                return;

            using (var channelGroup = new NotificationChannelGroup(request.Group, request.Name))
                notificationManager.CreateNotificationChannelGroup(channelGroup);
        }

        public static void CreateNotificationChannel(FNotificationChannelRequest request = null)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager notificationManager)
                return;

            if (request is null)
                request = new FNotificationChannelRequest();

            if (string.IsNullOrWhiteSpace(request.Name))
                request.Name = "General";

            if (string.IsNullOrWhiteSpace(request.Id))
                request.Id = FChannel.DEFAULT_CHANNEL_ID;

            using var channel = new NotificationChannel(request.Id, request.Name, request.Importance)
            {
                Description = request.Description,
                Group = request.Group,
                LightColor = request.LightColor,
                LockscreenVisibility = request.LockscreenVisibility,
            };
            var soundUri = FUtility.GetSoundUri(request.Sound);
            if (soundUri != null)
            {
                using var audioAttributesBuilder = new AudioAttributes.Builder();
                var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification).SetContentType(AudioContentType.Music).Build();

                channel.SetSound(soundUri, audioAttributes);
            }

            if (request.VibrationPattern != null)
                channel.SetVibrationPattern(request.VibrationPattern);

            channel.SetShowBadge(request.ShowBadge);
            channel.EnableLights(request.EnableLights);
            channel.EnableVibration(request.EnableVibration);

            notificationManager.CreateNotificationChannel(channel);
        }
    }
}