using Foundation;
using System;
using UIKit;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FDownload : FDownloadBase, IFDownload
    {
        public override string EnvironmentPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public override void SaveImageToGallery(byte[] imageByte, string filename)
        {
            var imageData = new UIImage(NSData.FromArray(imageByte));
            imageData.SaveToPhotosAlbum((image, error) =>
            {
                if (error != null)
                    Console.WriteLine(error);
            });
        }
    }
}