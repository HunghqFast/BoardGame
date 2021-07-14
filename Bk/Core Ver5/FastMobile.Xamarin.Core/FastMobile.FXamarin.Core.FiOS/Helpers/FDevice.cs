using UIKit;

namespace FastMobile.FXamarin.Core.FiOS
{
    public static class FDevice
    {
        private static bool? s_isiOS9OrNewer;
        private static bool? s_isiOS10OrNewer;
        private static bool? s_isiOS11OrNewer;
        private static bool? s_isiOS12OrNewer;
        private static bool? s_isiOS13OrNewer;
        private static bool? s_isiOS14OrNewer;
        private static bool? s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden;

        internal static bool IsiOS9OrNewer
        {
            get
            {
                if (!s_isiOS9OrNewer.HasValue)
                    s_isiOS9OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(9, 0);
                return s_isiOS9OrNewer.Value;
            }
        }

        internal static bool IsiOS10OrNewer
        {
            get
            {
                if (!s_isiOS10OrNewer.HasValue)
                    s_isiOS10OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
                return s_isiOS10OrNewer.Value;
            }
        }

        internal static bool IsiOS11OrNewer
        {
            get
            {
                if (!s_isiOS11OrNewer.HasValue)
                    s_isiOS11OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
                return s_isiOS11OrNewer.Value;
            }
        }

        internal static bool IsiOS12OrNewer
        {
            get
            {
                if (!s_isiOS12OrNewer.HasValue)
                    s_isiOS12OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(12, 0);
                return s_isiOS12OrNewer.Value;
            }
        }

        internal static bool IsiOS13OrNewer
        {
            get
            {
                if (!s_isiOS13OrNewer.HasValue)
                    s_isiOS13OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
                return s_isiOS13OrNewer.Value;
            }
        }

        internal static bool IsiOS14OrNewer
        {
            get
            {
                if (!s_isiOS14OrNewer.HasValue)
                    s_isiOS14OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(14, 0);
                return s_isiOS14OrNewer.Value;
            }
        }

        internal static bool RespondsToSetNeedsUpdateOfHomeIndicatorAutoHidden
        {
            get
            {
                if (!s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.HasValue)
                    s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden = new UIViewController().RespondsToSelector(new ObjCRuntime.Selector("setNeedsUpdateOfHomeIndicatorAutoHidden"));
                return s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.Value;
            }
        }
    }
}