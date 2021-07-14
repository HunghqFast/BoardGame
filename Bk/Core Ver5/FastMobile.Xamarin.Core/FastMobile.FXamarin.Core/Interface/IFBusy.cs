using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFBusy
    {
        //bool IsBusy { get; set; }
        Task SetBusy(bool value);
    }
}