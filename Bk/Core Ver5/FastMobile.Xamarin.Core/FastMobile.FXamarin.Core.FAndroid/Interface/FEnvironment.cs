using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Views;
using Java.IO;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Bitmap = Android.Graphics.Bitmap;
using Color = Xamarin.Forms.Color;
using Environment = System.Environment;
using Orientation = Android.Content.Res.Orientation;
using Platform = Xamarin.Essentials.Platform;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public class FEnvironment : IFEnvironment
    {
        public string BaseUrl { get; }

        public string DeviceID { get; }

        public string PersionalPath { get; }

        public Thickness SafeArea { get => default; }

        public FEnvironment()
        {
            BaseUrl = "file:///android_asset/";
            DeviceID = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId).Replace("'", "");
            PersionalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public void Close()
        {
            Platform.CurrentActivity.Finish();
        }

        [System.Obsolete]
        public void SetStatusBarColor(Color color, bool darkStatusBarTint)
        {
            var activity = Platform.CurrentActivity;
            activity.Window.DecorView.SystemUiVisibility = darkStatusBarTint ? (StatusBarVisibility)SystemUiFlags.LightStatusBar : 0;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                SetWindowFlag(WindowManagerFlags.TranslucentStatus, true);
                return;
            }
            SetWindowFlag(WindowManagerFlags.TranslucentStatus, false);
            activity.Window.SetStatusBarColor(color.ToAndroid());
        }

        private void SetWindowFlag(WindowManagerFlags bits, bool on)
        {
            if (on)
            {
                Platform.CurrentActivity.Window.Attributes.Flags |= bits;
                return;
            }
            Platform.CurrentActivity.Window.Attributes.Flags &= ~bits;
        }

        public Task OpenUrl(string url)
        {
            Android.App.Application.Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse(url)).AddFlags(ActivityFlags.NewTask));
            return Task.CompletedTask;
        }

        public void SetAllowRotation(FOrientation orientation)
        {
            Platform.CurrentActivity?.OnConfigurationChanged(new Configuration { Orientation = GetOrientation(orientation) });
        }

        public void OpenAppSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.SetData(Uri.FromParts("package", AppInfo.PackageName, null));
            Android.App.Application.Context.StartActivity(intent);
        }

        private Orientation GetOrientation(FOrientation orientation)
        {
            Orientation result = default;
            if (orientation == FOrientation.Landscape)
                result = result != default ? result | Orientation.Landscape : Orientation.Landscape;
            if (orientation == FOrientation.Portrait)
                result = result != default ? result | Orientation.Portrait : Orientation.Portrait;
            if (orientation == FOrientation.Square)
                result = result != default ? result | Orientation.Square : Orientation.Square;
            if (orientation == FOrientation.Undefined)
                result = result != default ? result | Orientation.Undefined : Orientation.Undefined;
            return result;
        }

        public ImageSource GetThumbImage(string videoPath, long second)
        {
            try
            {
                if (string.IsNullOrEmpty(videoPath))
                    return null;
                var retriever = new MediaMetadataRetriever();
                retriever.SetDataSource(new FileInputStream(videoPath).FD);
                var bitmap = retriever.GetFrameAtTime(second);
                if (bitmap != null)
                {
                    var stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    byte[] bitmapData = stream.ToArray();
                    return ImageSource.FromStream(() => new MemoryStream(bitmapData));
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}