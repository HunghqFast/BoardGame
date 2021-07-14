using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FInterface
    {
        static FInterface()
        {
            IFAndroid = DependencyService.Get<IFAndroid>();
            IFDisplayAlert = DependencyService.Get<IFDisplayAlert>();
            IFDownloader = DependencyService.Get<IFDownloader>();
            IFEnvironment = DependencyService.Get<IFEnvironment>();
            IFFirebase = DependencyService.Get<IFFirebase>();
            IFLocalNotifications = DependencyService.Get<IFLocalNotifications>();
            IFVersion = DependencyService.Get<IFVersion>();
            IFDownload = DependencyService.Get<IFDownload>();
            IFFingerprint = DependencyService.Get<IFFingerprint>();
        }

        public static IFAndroid IFAndroid { get; }

        public static IFDisplayAlert IFDisplayAlert { get; }

        public static IFDownloader IFDownloader { get; }

        public static IFEnvironment IFEnvironment { get; }

        public static IFFirebase IFFirebase { get; }

        public static IFLocalNotifications IFLocalNotifications { get; }

        public static IFVersion IFVersion { get; }

        public static IFDownload IFDownload { get; }

        public static IFFingerprint IFFingerprint { get; }
    }
}