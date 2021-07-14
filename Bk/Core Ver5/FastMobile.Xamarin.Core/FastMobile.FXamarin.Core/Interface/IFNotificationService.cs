namespace FastMobile.FXamarin.Core
{
    public interface IFNotificationService
    {
        void Cancel(int notificationId);

        void CancelAll();

        void OnNotificationTapped(FNotificationTappedEventArgs e);

        void Show(FNotificationRequest notificationRequest);

        event FNotificationTappedEventHandler NotificationTapped;
    }
}