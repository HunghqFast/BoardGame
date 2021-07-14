using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FNotificationCenter : Core.FNotificationCenter
    {
        private static bool? AlertsAllowed;

        public static void Init()
        {
            try
            {
                Current = new FNotificationServiceImpl();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static async void AskPermission()
        {
            await AskPermissionAsync().ConfigureAwait(false);
        }

        public static async Task<bool> AskPermissionAsync()
        {
            try
            {
                if (AlertsAllowed.HasValue)
                {
                    return AlertsAllowed.Value;
                }

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    AlertsAllowed = true;
                    return true;
                }

                var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
                var allowed = settings.AlertSetting == UNNotificationSetting.Enabled;

                if (allowed)
                {
                    AlertsAllowed = true;
                    return true;
                }

                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound).ConfigureAwait(false);

                AlertsAllowed = alertsAllowed;
                Debug.WriteLine(error?.LocalizedDescription);

                return AlertsAllowed.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static void ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return;
            }

            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationList) =>
            {
                if (notificationList.Any())
                {
                    return;
                }

                uiApplication.InvokeOnMainThread(() =>
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                });
            });
        }
    }
}