using Android.Content;
using System;
using Xamarin.Essentials;


namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FDownload : FDownloadBase, IFDownload
    {
        private Context CurrentContext => Platform.CurrentActivity;

        [Obsolete]
        public override string EnvironmentPath()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        }

        [Obsolete]
        public override void SaveImageToGallery(byte[] imageByte, string filename)
        {
            try
            {
                Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                string path = System.IO.Path.Combine(storagePath.ToString(), filename);
                System.IO.File.WriteAllBytes(path, imageByte);
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
                CurrentContext.SendBroadcast(mediaScanIntent);
            }
            catch (Exception ex)
            {
            }
        }
    }
}