using System.IO;

namespace FastMobile.FXamarin.Core
{
    public class FFileInfo
    {
        public FFileInfo(string filePath, string line_nbr_mode, byte[] data, string fileName)
        {
            Data = data;
            Info = string.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
            LineNbrMode = line_nbr_mode;
            FileName = fileName;
        }

        public FileInfo Info { get; }
        public byte[] Data { get; }
        public string LineNbrMode { get; }
        public string FileName { get; }
    }
}