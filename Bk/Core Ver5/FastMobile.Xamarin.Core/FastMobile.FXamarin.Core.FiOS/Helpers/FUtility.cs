using CoreAnimation;
using CoreGraphics;
using CoreLocation;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

namespace FastMobile.FXamarin.Core.FiOS
{
    public static class FUtility
    {
        public static async Task<UIImage> ToImageFromImageSource(this ImageSource imageSource)
        {
            IImageSourceHandler handler;
            if (imageSource is FileImageSource)
                handler = new FileImageSourceHandler();
            else if (imageSource is StreamImageSource)
                handler = new StreamImagesourceHandler();
            else if (imageSource is UriImageSource)
                handler = new ImageLoaderSourceHandler();
            else handler = new FontImageSourceHandler();

            return await handler.LoadImageAsync(imageSource);
        }

        public static string CleanseFontName(string fontName)
        {
            var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
            if (hasFontAlias)
                return fontPostScriptName;

            var fontFile = FontFile.FromString(fontName);

            if (!string.IsNullOrWhiteSpace(fontFile.Extension))
            {
                var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
                if (hasFont)
                    return filePath ?? fontFile.PostScriptName;
            }
            else
            {
                foreach (var ext in FontFile.Extensions)
                {
                    var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension(ext));
                    if (hasFont) return filePath;
                }
            }
            return fontFile.PostScriptName;
        }

        public static UIFont FindFont(string fontFamily, float fontSize)
        {
            if (fontFamily != null)
                return UIFont.FromName(CleanseFontName(fontFamily), fontSize);
            return UIFont.SystemFontOfSize(fontSize);
        }

        public static double ToPlatformDesiredAccuracy(this FLocationAccuracy desiredAccuracy)
        {
            return desiredAccuracy switch
            {
                FLocationAccuracy.Lowest => CLLocation.AccuracyThreeKilometers,
                FLocationAccuracy.Low => CLLocation.AccuracyKilometer,
                FLocationAccuracy.Default or FLocationAccuracy.Medium => CLLocation.AccuracyHundredMeters,
                FLocationAccuracy.High => CLLocation.AccuracyNearestTenMeters,
                FLocationAccuracy.Best => CLLocation.AccurracyBestForNavigation,
                _ => CLLocation.AccuracyHundredMeters,
            };
        }

        public static UIImage ToUIImage(this Color color, CGContext context)
        {
            var gradientLayer = new CALayer
            {
                Frame = new CGRect(0, 0, double.MaxValue, double.MaxValue),
                BackgroundColor = color.ToCGColor(),
            };

            UIGraphics.BeginImageContext(gradientLayer.Bounds.Size);
            gradientLayer.RenderInContext(context);
            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }

        public static UIReturnKeyType ToUIReturnKeyType(this ReturnType returnType)
        {
            return returnType switch
            {
                ReturnType.Go => UIReturnKeyType.Go,
                ReturnType.Next => UIReturnKeyType.Next,
                ReturnType.Send => UIReturnKeyType.Send,
                ReturnType.Search => UIReturnKeyType.Search,
                ReturnType.Done => UIReturnKeyType.Done,
                ReturnType.Default => UIReturnKeyType.Default,
                _ => UIReturnKeyType.Default,
            };
        }
    }
}