using Android.App;
using Android.Content;

using Firebase.Messaging;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core.FAndroid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FNotifyMessagingServices : FirebaseMessagingService, IFNotificationReceived
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            OnNotifyReceived(message);
        }

        #region Interface

        public virtual async void OnNotifyReceived(params object[] @params)
        {
            if (string.IsNullOrEmpty(FSetting.NetWorkKey) || @params[0] is not RemoteMessage message || message.Data == null)
                return;
            try
            {
                string messageBody = "", notifyCode = "", action = "", group = "";

                if (message.Data.ContainsKey(FText.NotifyBodyKey))
                    messageBody = message.Data[FText.NotifyBodyKey];
                if (message.Data.ContainsKey(FText.NotifyCodeKey))
                    notifyCode = message.Data[FText.NotifyCodeKey];
                if (message.Data.ContainsKey(FText.NotifyActionKey))
                    action = message.Data[FText.NotifyActionKey];
                if (message.Data.ContainsKey(FText.NotifyGroupKey))
                    group = message.Data[FText.NotifyGroupKey];
                SendAction(action);
                if (!string.IsNullOrEmpty(notifyCode))
                {
                    await Task.Delay(IFNotificationReceived.DelayBeforeNotify);
                    SendNotificationInfo(notifyCode, group, FChannel.NOTIFYRECEIVED + group);
                }
                FNotificationCenter.Current?.Show(new FNotificationRequest
                {
                    Title = FText.ApplicationTitle,
                    Description = messageBody,
                    ReturningData = new Dictionary<string, string> { { FText.NotifyBodyKey, messageBody }, { FText.NotifyCodeKey, notifyCode }, { FText.NotifyActionKey, action }, { FText.NotifyGroupKey, group } }.ToJson(),
                    NotificationId = new Random().Next()
                });
            }
            catch { }
        }

        public virtual async void SendAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                return;

            switch (action)
            {
                case "0":
                case "10":
                case "90":
                    break;
                case "100":
                    FSetting.ClearKey();
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet && !await new FVersion().IsUsingLatestVersion())
                        MessagingCenter.Send(new FMessage(), FChannel.OLD_VERSIONS);
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
    }
}