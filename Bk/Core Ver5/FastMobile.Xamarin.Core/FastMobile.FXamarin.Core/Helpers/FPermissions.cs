using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace FastMobile.FXamarin.Core
{
    public static class FPermissions
    {
        public static void ShowMessage()
        {
            MessagingCenter.Send(new FMessageConfirm(ShowSetting, 0, 908, FText.ApplicationTitle), FChannel.ALERT_BY_MESSAGE);
        }

        private static void ShowSetting(bool value)
        {
            if (value) FInterface.IFEnvironment.OpenAppSettings();
        }


        public static async Task<bool> HasPermission<T>(bool required = true) where T : BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (!required)
                return status == PermissionStatus.Granted;
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<T>();
            return status == PermissionStatus.Granted;
        }
    }
}
