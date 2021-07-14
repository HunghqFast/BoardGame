using Firebase.CloudMessaging;
using Foundation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FLocalNotificationDelegate : UNUserNotificationCenterDelegate, IFNotificationReceived, IMessagingDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            try
            {
                OnReceived(false, response.Notification);
            }
            catch { }
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification response, Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                OnReceived(true, response, completionHandler);
            }
            catch { }
        }

        public void SetAction(NSDictionary options, UIApplicationState state)
        {
            OnNotifyReceived(options, state);
        }

        #region Interface

        public virtual async void OnNotifyReceived(params object[] @params)
        {
            if (string.IsNullOrEmpty(FSetting.NetWorkKey) || @params[0] is not NSDictionary data)
                return;

            if (data == null)
                return;
            if (data.ContainsKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")))
                data = (NSDictionary)data[new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")];
            if (data.ContainsKey(new NSString("ApnsPayLoad")))
                data = (NSDictionary)data[new NSString("ApnsPayLoad")];

            string messageBody = "", notifyCode = "", action = "", group = "";
            if (data.ContainsKey(new NSString(FText.NotifyBodyKey)))
                messageBody = data[new NSString(FText.NotifyBodyKey)].ToString();
            if (data.ContainsKey(new NSString(FText.NotifyCodeKey)))
                notifyCode = data[new NSString(FText.NotifyCodeKey)].ToString();
            if (data.ContainsKey(new NSString(FText.NotifyActionKey)))
                action = data[new NSString(FText.NotifyActionKey)].ToString();
            if (data.ContainsKey(new NSString(FText.NotifyGroupKey)))
                group = data[new NSString(FText.NotifyGroupKey)].ToString();
            SendAction(action);
            if (!string.IsNullOrEmpty(notifyCode))
            {
                await Task.Delay(IFNotificationReceived.DelayBeforeNotify);
                SendNotificationInfo(notifyCode, group, FChannel.NOTIFYRECEIVED + group);
            }
            if ((UIApplicationState)@params[1] != UIApplicationState.Active)
            {
                FApplication.NotifyID = notifyCode;
                FApplication.NotifyGroup = group;
                FApplication.NotifyAction = action;
            }
            else
            {
                FNotificationCenter.Current?.Show(new FNotificationRequest
                {
                    Title = FText.ApplicationTitle,
                    Description = messageBody,
                    ReturningData = new Dictionary<string, string> { { FText.NotifyBodyKey, messageBody }, { FText.NotifyCodeKey, notifyCode }, { FText.NotifyActionKey, action }, { FText.NotifyGroupKey, group } }.ToJson(),
                    NotificationId = new Random().Next(),
                });
            }
        }

        public virtual async void SendAction(string action)
        {
            switch (action)
            {
                case "0":
                case "10":
                case "90":
                    break;

                case "100":
                    FSetting.ClearKey();
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet && !await new FVersion().IsUsingLatestVersion())
                        MessagingCenter.Send(new FMessage(), Core.FChannel.OLD_VERSIONS);
                    break;

                default:
                    break;
            }
        }

        public virtual void SendNotificationInfo(params string[] @params)
        {
            MessagingCenter.Send(new FNotifyInformation { ID = @params[0], Group = @params[1] }, @params[2]);
        }

        #endregion Interface

        private void OnReceived(bool isActive, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler = null)
        {
            if (notification is null || notification.Request is null || notification.Request.Content is null)
                return;

            if (!notification.Request.Content.UserInfo.ContainsKey(new NSString(FChannel.EXTRA_RETURN_DATA)))
            {
                OnNotifyReceived(notification.Request.Content.UserInfo, UIApplication.SharedApplication.ApplicationState);
                return;
            }
            completionHandler?.Invoke(UNNotificationPresentationOptions.Alert);
            if (isActive)
                return;
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                try
                {
                    FNotificationCenter.Current.OnNotificationTapped(new FNotificationTappedEventArgs(notification.Request.Content.UserInfo[FChannel.EXTRA_RETURN_DATA].ToString()));
                    Int32.TryParse(notification.Request.Content.Badge.ToString(), out var badge);
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
                }
                catch { }
            });
        }
    }
}