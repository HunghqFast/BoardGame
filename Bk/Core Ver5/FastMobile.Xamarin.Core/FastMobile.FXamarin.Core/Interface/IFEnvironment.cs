using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFEnvironment
    {
        string BaseUrl { get; }

        string DeviceID { get; }

        string PersionalPath { get; }

        Thickness SafeArea { get; }

        void Close();

        void SetStatusBarColor(Color color, bool darkStatusBarTint);

        void SetAllowRotation(FOrientation orientation);

        void OpenAppSettings();

        Task OpenUrl(string url);

        ImageSource GetThumbImage(string videoPath, long second);
    }
}