namespace FastMobile.FXamarin.Core
{
    public interface IFNotificationReceived
    {
        public const int DelayBeforeNotify = 1;

        void OnNotifyReceived(params object[] @params);

        void SendNotificationInfo(params string[] @params);

        void SendAction(string action);
    }
}