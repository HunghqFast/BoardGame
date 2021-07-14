using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFVersion
    {
        string InstalledVersionNumber { get; }

        Task<bool> IsUsingLatestVersion();

        Task<string> GetLatestVersionNumber();

        Task OpenAppInStore();
    }
}