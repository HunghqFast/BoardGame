using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FDownloader : IFDownloader
    {
        private static readonly WebClient webClient;

        static FDownloader()
        {
            webClient = new WebClient();
        }

        public FDownloader()
        {
        }

        public Stream DownloadPdf(string url)
        {
            var documentStream = webClient.OpenRead(new Uri(url));
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
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

        public async Task<FMessage> SaveFile(string fileName)
        {
            try
            {
                var pathW = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var pathR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
                var filePathW = Path.Combine(pathW, fileName);
                var filePathR = Path.Combine(pathR, "tempPdf.pdf");
                var fileSave = File.Create(filePathW);
                var fileRead = File.Open(filePathR, FileMode.Open);

                fileRead.Seek(0, SeekOrigin.Begin);
                await fileRead.CopyToAsync(fileSave);
                fileSave.Close();
                fileRead.Flush();
                fileRead.Close();
                return new FMessage(0, 901, "");
            }
            catch (Exception e)
            {
                return new FMessage(0, 310, e.Message);
            }
        }
    }
}