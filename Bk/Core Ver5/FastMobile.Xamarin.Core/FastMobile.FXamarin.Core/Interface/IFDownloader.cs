using System.IO;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFDownloader
    {
        Stream DownloadPdf(string url);

        Task<FMessage> SaveFile(string fileName);
    }
}