using AVFoundation;
using CoreMedia;
using Foundation;
using Security;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FEnvironment : IFEnvironment
    {
        public string BaseUrl { get; }

        public string DeviceID { get; }

        public string PersionalPath { get; }

        public Thickness SafeArea { get => new Thickness(UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Left, UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Top, UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Right, UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom); }

        public FEnvironment()
        {
            BaseUrl = NSBundle.MainBundle?.BundlePath;
            DeviceID = GetUniqueID();
            PersionalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
        }

        public void Close()
        {
            Thread.CurrentThread.Abort();
        }

        public Task OpenUrl(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
            return Task.CompletedTask;
        }

        public void SetStatusBarColor(Color color, bool darkStatusBarTint)
        {
            UIApplication.SharedApplication.SetStatusBarStyle(darkStatusBarTint ? UIStatusBarStyle.DarkContent : UIStatusBarStyle.LightContent, true);
            Xamarin.Essentials.Platform.GetCurrentUIViewController()?.SetNeedsStatusBarAppearanceUpdate();
        }

        private string GetUniqueID()
        {
            var query = new SecRecord(SecKind.GenericPassword) { Service = NSBundle.MainBundle.BundleIdentifier, Account = "UniqueID" };
            if (SecKeyChain.QueryAsData(query) is NSData uniqueId)
                return uniqueId.ToString().Replace("-", "").Replace("'", "");

            query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
            var err = SecKeyChain.Add(query);
            if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
                return UIDevice.CurrentDevice.IdentifierForVendor.ToString().Replace("-", "").Replace("'", "");
            return query.ValueData.ToString().Replace("-", "").Replace("'", "");
        }

        public void SetAllowRotation(FOrientation orientation)
        {

        }

        public void OpenAppSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }

        public ImageSource GetThumbImage(string videoPath, long second)
        {
            var imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl(new NSUrl(videoPath)));
            imageGenerator.AppliesPreferredTrackTransform = true;
            var cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(second, 1000000), out var actualTime, out var error);
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }
    }
}