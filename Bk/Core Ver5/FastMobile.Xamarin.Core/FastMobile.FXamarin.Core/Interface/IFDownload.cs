using System;
using System.IO;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public interface IFDownload
    {
        public const string DownloadKey = "FastMobile.FXamarin.Core.IFDownload.Downloaded";

        event EventHandler<FDownloadEventArgs> OnFileDownloaded;

        string EnvironmentPath();

        (bool OK, byte[] Data, string Path) SaveImage(Stream stream, string fileName);

        void SaveImageToGallery(byte[] imageByte, string filename);

        Task DownloadFile(string url, string saveFileName = "", string saveToFolder = "");

        Task DownloadFileWithText(string url, string saveFileName = "", string saveToFolder = "");

        Task DeleteDownload(string filePath);
    }
}