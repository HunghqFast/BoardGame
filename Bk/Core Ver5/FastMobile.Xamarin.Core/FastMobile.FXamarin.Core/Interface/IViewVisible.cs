using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IViewVisible<T> where T : View
    {
        T View { get; }
        bool Show { get; set; }
    }
}