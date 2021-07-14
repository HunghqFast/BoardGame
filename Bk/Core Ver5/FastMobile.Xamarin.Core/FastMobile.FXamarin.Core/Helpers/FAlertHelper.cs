using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FAlertHelper
    {
        public static FResourceManager Manager { get; set; }

        static FAlertHelper()
        {
            Manager = new FResourceManager("FastMobile.FXamarin.Core.Resources.Config.xml", "Message");
        }

        public static void Init(object subscriber)
        {
            MessagingCenter.Subscribe<FMessage>(subscriber, FChannel.ALERT_BY_MESSAGE, (s) => Device.BeginInvokeOnMainThread(() => Show(s)));
            MessagingCenter.Subscribe<FMessageToast>(subscriber, FChannel.ALERT_BY_MESSAGE, (s) => Device.BeginInvokeOnMainThread(() => Show(s)));
            MessagingCenter.Subscribe<FMessageOptions>(subscriber, FChannel.ALERT_BY_MESSAGE, (s) => Device.BeginInvokeOnMainThread(() => Show(s)));
            MessagingCenter.Subscribe<FAlertArguments>(subscriber, FChannel.ALERT_BY_MESSAGE, (s) => Device.BeginInvokeOnMainThread(() => Show(s)));
            MessagingCenter.Subscribe<FMessageConfirm>(subscriber, FChannel.ALERT_BY_MESSAGE, (s) => Device.BeginInvokeOnMainThread(() => Show(s)));
        }

        public static string MessageByCode(string code)
        {
            return Manager.GetString(code);
        }

        public static async void Show(string code)
        {
            await new FAlertBase().Show(Manager.GetString(code), FText.Accept);
        }

        public static async void Toast(string code, int miliseconds)
        {
            await new FAlertBase().Toast(Manager.GetString(code), FText.Accept, miliseconds);
        }

        public static Task<bool> Confirm(string code)
        {
            return new FAlertBase().Confirm(Manager.GetString(code));
        }

        public static Task<bool> Confirm(string code, string message)
        {
            return new FAlertBase().Confirm(string.Format(Manager.GetString(code), message.Split((char)254)));
        }

        public static async Task<bool> Confirm(string code, string acceptCode, string cancelCode)
        {
            return await new FAlertBase().Confirm(Manager.GetString(code), Manager.GetString(acceptCode), Manager.GetString(cancelCode));
        }

        public static async Task<bool> Confirm(string code, string message, string acceptCode, string cancelCode)
        {
            return await new FAlertBase().Confirm(string.Format(Manager.GetString(code), message.Split((char)254)), Manager.GetString(acceptCode), Manager.GetString(cancelCode));
        }

        public static async Task<string> ShowOptions(string code, IEnumerable<FItemCustom> dataSource)
        {
            return await new FAlertOptions().ShowOptions(Manager.GetString(code), FText.Accept, FText.Cancel, dataSource);
        }

        private static async void Show(FAlertArguments fAlert)
        {
            await new FAlertBase().Show(fAlert.Title, fAlert.Message, fAlert.Accept);
        }

        private static async void Show(FMessage sender)
        {
            if (FSetting.IsDebug)
            {
                if (!Out(sender))
                {
                    var message = Manager.GetString(sender.Code.ToString());
                    if (message == FText.ErrorDefault)
                        message += $"{Environment.NewLine} $${{0}}$$";
                    message = string.Format(message, sender.Message.Split((char)254));
                    await new FAlertBase().Show(message, FText.Accept);
                }
                return;
            }

            if (!Out(sender))
                await new FAlertBase().Show(string.Format(Manager.GetString(sender.Code.ToString()), sender.Message.Split((char)254)), FText.Accept);
        }

        private static async void Show(FMessageToast sender)
        {
            if (FSetting.IsDebug)
            {
                if (!Out(sender))
                {
                    var message = Manager.GetString(sender.Code.ToString());
                    if (message == FText.ErrorDefault)
                        message += $"{Environment.NewLine} $${{0}}$$";
                    message = string.Format(message, sender.Message.Split((char)254));
                    await new FAlertBase().Toast(message, FText.Accept, sender.Milisecond);
                }
                return;
            }

            if (!Out(sender))
                await new FAlertBase().Toast(string.Format(Manager.GetString(sender.Code.ToString()), sender.Message.Split((char)254)), FText.Accept, sender.Milisecond);
        }

        private static async void Show(FMessageOptions sender)
        {
            if (FSetting.IsDebug)
            {
                if (!Out(sender))
                {
                    var message = Manager.GetString(sender.Code.ToString());
                    if (message == FText.ErrorDefault)
                        message += $"{Environment.NewLine} $${{0}}$$";
                    message = string.Format(message, sender.Message.Split((char)254));
                    sender.Action?.Invoke(await new FAlertOptions().ShowOptions(message, sender.Source));
                }
                return;
            }

            if (!Out(sender))
                sender.Action?.Invoke(await new FAlertOptions().ShowOptions(string.Format(Manager.GetString(sender.Code.ToString()), sender.Message.Split((char)254)), sender.Source));
        }

        private static async void Show(FMessageConfirm sender)
        {
            var alert = new FAlertBase();
            alert.Confirmed += (s, e) => sender.Completed?.Invoke(e.Value);
            await alert.Confirm(string.Format(Manager.GetString(sender.Code.ToString()), sender.Message?.Split((char)254)), FText.Yes, FText.No);
        }

        private static bool Out(FMessage sender)
        {
            switch (sender.Code)
            {
                case 404:
                    MessagingCenter.Send(sender, FChannel.TIMEOUT);
                    return true;

                case 202:
                    MessagingCenter.Send(sender, FChannel.TIMEOUT);
                    return true;

                case 303:
                    MessagingCenter.Send(sender, FChannel.TIMEOUT);
                    return true;

                case 204:
                    MessagingCenter.Send(sender, FChannel.NOT_MATCH_VERSION);
                    return true;

                case 205:
                    MessagingCenter.Send(sender, FChannel.NOT_MATCH_VERSION);
                    return true;

                case 501:
                    MessagingCenter.Send(sender, FChannel.OLD_VERSIONS);
                    return true;

                default:
                    return false;
            }
        }
    }
}