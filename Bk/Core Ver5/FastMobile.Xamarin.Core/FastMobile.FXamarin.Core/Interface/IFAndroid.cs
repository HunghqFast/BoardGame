using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public interface IFAndroid
    {
        void SetCurentWindowBackground(Color start, Color end);

        void TransparentStatusBar();
    }
}