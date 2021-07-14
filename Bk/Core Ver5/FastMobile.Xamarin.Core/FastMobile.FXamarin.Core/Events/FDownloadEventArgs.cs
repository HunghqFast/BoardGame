namespace FastMobile.FXamarin.Core
{
    public class FDownloadEventArgs
    {
        public bool FileSaved { get; }
        public string Path { get; }
        public FMessage Message { get; }

        public FDownloadEventArgs(bool saved, FMessage message, string path)
        {
            FileSaved = saved;
            Message = message;
            Path = path;
        }
    }
}