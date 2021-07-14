using Xamarin.Forms.Internals;
using DeviceInfo = Xamarin.Essentials.DeviceInfo;

namespace FastMobile.FXamarin.Core
{
    [Preserve(AllMembers = true)]
    public class FDeviceInformation
    {
        public FDeviceInformation(bool getToken, string token = "", string sender = "")
        {
            DeviceType = DeviceInfo.DeviceType.ToString();
            DeviceModel = DeviceInfo.Model;
            DeviceName = DeviceInfo.Name;
            Idiom = DeviceInfo.Idiom.ToString();
            Version = DeviceInfo.VersionString;
            Manufacturer = DeviceInfo.Manufacturer;
            Platform = DeviceInfo.Platform.ToString();
            DeviceId = FSetting.DeviceID;
            NotifyToken = getToken ? string.IsNullOrEmpty(sender) ? FInterface.IFFirebase?.GetToken() : FInterface.IFFirebase?.GetToken(sender) : token;
        }

        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public string Manufacturer { get; set; }
        public string DeviceName { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceType { get; set; }
        public string NotifyToken { get; set; }
    }
}