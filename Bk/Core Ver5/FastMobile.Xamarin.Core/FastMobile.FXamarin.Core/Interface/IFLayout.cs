using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFLayout
    {
        Task OnChanged();

        Task OnLoaded();
    }
}