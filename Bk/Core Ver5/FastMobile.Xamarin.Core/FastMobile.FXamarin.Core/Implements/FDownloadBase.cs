using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public abstract class FDownloadBase : IFDownload
    {
        public event EventHandler<FDownloadEventArgs> OnFileDownloaded;

        public Task DeleteDownload(string filePath)
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        [Obsolete]
        public async Task DownloadFile(string url, string saveFileName = "", string saveToFolder = "")
        {
            try
            {
                var fileName = string.IsNullOrEmpty(saveFileName) ? Path.GetFileName(url) : saveFileName;
                var filePath = Path.Combine(EnvironmentPath(), FText.ApplicationTitle, saveToFolder);
                var webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler((s, e) =>
                {
                    if (e.Error != null)
                    {
                        if (e.Error is WebException ex)
                        {
                            OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(false, GetMessage(ex.Response), string.Empty));
                            return;
                        }

                        OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(false, FMessage.FromFail(1103, "101"), string.Empty));
                        return;
                    }
                    OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(true, FMessage.FromSuccess(), Path.Combine(filePath, fileName)));
                });
                Directory.CreateDirectory(filePath);
                await webClient.DownloadFileTaskAsync(new Uri(url), Path.Combine(filePath, fileName));
            }
            catch
            {
                OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(false, FMessage.FromFail(1103, "100"), string.Empty));
            }
        }

        public abstract string EnvironmentPath();

        public abstract void SaveImageToGallery(byte[] imageByte, string filename);

        [Obsolete]
        public async Task DownloadFileWithText(string url, string saveFileName = "", string saveToFolder = "")
        {
            try
            {
                var content = await new HttpClient().GetStringAsync(url);
                if (string.IsNullOrEmpty(content))
                {
                    OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(false, FMessage.FromFail(1103, "101"), string.Empty));
                    return;
                }
                var fileName = string.IsNullOrEmpty(saveFileName) ? Path.GetFileName(url) : saveFileName;
                var filePath = Path.Combine(EnvironmentPath(), FText.ApplicationTitle, saveToFolder);
                Directory.CreateDirectory(filePath);
                File.WriteAllText(Path.Combine(filePath, fileName), content);
                OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(true, FMessage.FromSuccess(), Path.Combine(filePath, fileName)));
            }
            catch
            {
                OnFileDownloaded?.Invoke(this, new FDownloadEventArgs(false, FMessage.FromFail(1103, "100"), string.Empty));
                return;
            }
        }

        private FMessage GetMessage(WebResponse response)
        {
            try
            {
                if (response is HttpWebResponse httpWeb)
                    return httpWeb.StatusDescription.ToObject<FMessage>();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd().ToObject<FMessage>();
                }
            }
            catch (Exception ex)
            {
                return new FMessage(ex.Message);
            }
        }

        public (bool OK, byte[] Data, string Path) SaveImage(Stream stream, string saveFileName)
        {
            try
            {
                var filePath = Path.Combine(EnvironmentPath(), FText.ApplicationTitle);
                var fileName = Path.Combine(filePath, saveFileName);
                var bytes = FUtility.GetFileData(stream);
                Directory.CreateDirectory(filePath);
                File.WriteAllBytes(fileName, bytes);
                return (true, bytes, fileName);
            }
            catch
            {
                return (false, null, null);
            }
        }
    }
}