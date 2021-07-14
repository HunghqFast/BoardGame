using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FDownloader : IFDownloader
    {
        private static readonly WebClient webClient;

        static FDownloader()
        {
            webClient = new WebClient();
        }

        public Stream DownloadPdf(string url)
        {
            var documentStream = webClient.OpenRead(new Uri(url));
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(path, "tempPdf.pdf");

            try
            {
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                documentStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch { }

            if (File.Exists(filePath))
                return new MemoryStream(File.ReadAllBytes(filePath));
            return documentStream;
        }

        [Obsolete]
        public async Task<FMessage> SaveFile(string fileName)
        {
            try
            {
                var pathW = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                var pathR = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePathW = Path.Combine(pathW, fileName);
                var filePathR = Path.Combine(pathR, "tempPdf.pdf");
                var fileRead = File.Open(filePathR, FileMode.Open);
                var fileSave = File.Create(filePathW);

                fileRead.Seek(0, SeekOrigin.Begin);
                await fileRead.CopyToAsync(fileSave);
                fileSave.Close();
                fileRead.Flush();
                fileRead.Close();
                return new FMessage(1, 901, "");
            }
            catch (Exception e)
            {
                return new FMessage(0, 310, e.Message);
            }
        }
    }
}