using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.Text;
using Android.Text.Style;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using AApp = Android.App.Application;
using AColor = Android.Graphics.Color;
using AColorFilter = Android.Graphics.ColorFilter;
using ADrawable = Android.Graphics.Drawables.Drawable;
using ADrawableCompat = AndroidX.Core.Graphics.Drawable.DrawableCompat;
using AndroidLocation = Android.Locations.Location;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;
using LocationPower = Android.Locations.Power;
using Uri = Android.Net.Uri;
using Color = Xamarin.Forms.Color;
using Android.Views.InputMethods;

namespace FastMobile.FXamarin.Core.FAndroid
{
    public static class FUtility
    {
        public const long TwoMinutes = 120000;
        public const string LoadFromAssetsRegex = @"\w+\.((ttf)|(otf))\#\w*";
        public static void InitExtrasByNotify(Intent intent)
        {
            try
            {
                if (intent.Extras is null)
                    return;

                if (intent.HasExtra(FText.NotifyCodeKey) && intent.HasExtra(FText.NotifyGroupKey))
                {
                    FApplication.NotifyID = intent.GetStringExtra(FText.NotifyCodeKey);
                    FApplication.NotifyGroup = intent.GetStringExtra(FText.NotifyGroupKey);
                    FApplication.NotifyAction = intent.GetStringExtra(FText.NotifyActionKey);
                    return;
                }

                if (!intent.HasExtra(FChannel.EXTRA_RETURN_DATA))
                    return;

                if ((intent.GetStringExtra(FChannel.EXTRA_RETURN_DATA).ToObject<Dictionary<string, string>>() is Dictionary<string, string> param) && param.ContainsKey(FText.NotifyCodeKey) && param.ContainsKey(FText.NotifyGroupKey))
                {
                    FApplication.NotifyID = param[FText.NotifyCodeKey];
                    FApplication.NotifyGroup = param[FText.NotifyGroupKey];
                    FApplication.NotifyAction = param[FText.NotifyActionKey];
                }
            }
            catch { }
        }

        public static Dictionary<string, string> GetIntentNotify(Intent intent)
        {
            if (intent.Extras == null)
                return null;
            if (!intent.HasExtra(FText.NotifyCodeKey) || !intent.HasExtra(FText.NotifyGroupKey))
                return null;
            var result = new Dictionary<string, string>();
            result.Add(FText.NotifyCodeKey, intent.Extras.GetString(FText.NotifyCodeKey));
            result.Add(FText.NotifyGroupKey, intent.Extras.GetString(FText.NotifyGroupKey));
            result.Add(FText.NotifyActionKey, intent.Extras.GetString(FText.NotifyActionKey));
            return result;
        }

        public static Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
                return null;

            if (soundFileName.Contains("://", StringComparison.InvariantCulture) == false)
                soundFileName = $"{ContentResolver.SchemeAndroidResource}://{AApp.Context.PackageName}/raw/{soundFileName}";

            return Uri.Parse(soundFileName);
        }

        public static Typeface FindFont(string fontFamily, TypefaceStyle typefaceStyle)
        {
            if (!string.IsNullOrWhiteSpace(fontFamily))
                return Regex.IsMatch(fontFamily, LoadFromAssetsRegex) ? Typeface.CreateFromAsset(Android.App.Application.Context.Assets, FontNameToFontFile(fontFamily)) : Typeface.Create(fontFamily, typefaceStyle);

            return Typeface.Create(Typeface.Default, typefaceStyle);
        }

        public static string FontNameToFontFile(string fontFamily)
        {
            int hashtagIndex = fontFamily.IndexOf('#');
            if (hashtagIndex >= 0)
                return fontFamily.Substring(0, hashtagIndex);
            return string.Empty;
        }

        public static bool IsDisposed(this Java.Lang.Object obj)
        {
            return obj.Handle == IntPtr.Zero;
        }

        public static bool IsAlive(this Java.Lang.Object obj)
        {
            if (obj == null)
                return false;

            return !obj.IsDisposed();
        }

        public static bool IsDisposed(this global::Android.Runtime.IJavaObject obj)
        {
            return obj.Handle == IntPtr.Zero;
        }

        public static bool IsAlive(this global::Android.Runtime.IJavaObject obj)
        {
            if (obj == null)
                return false;

            return !obj.IsDisposed();
        }

        public static IEnumerable<T> GetChildrenOfType<T>(this AViewGroup self) where T : AView
        {
            for (var i = 0; i < self.ChildCount; i++)
            {
                AView child = self.GetChildAt(i);
                var typedChild = child as T;
                if (typedChild != null)
                    yield return typedChild;

                if (child is AViewGroup)
                {
                    IEnumerable<T> myChildren = (child as AViewGroup).GetChildrenOfType<T>();
                    foreach (T nextChild in myChildren)
                        yield return nextChild;
                }

            }
        }

        public static string GetText(this FAuthenticationHelpTexts texts, FFingerprintAuthenticationHelp help, string nativeText)
        {
            return help switch
            {
                FFingerprintAuthenticationHelp.MovedTooFast when !string.IsNullOrEmpty(texts.MovedTooFast) => texts.MovedTooFast,
                FFingerprintAuthenticationHelp.MovedTooSlow when !string.IsNullOrEmpty(texts.MovedTooSlow) => texts.MovedTooSlow,
                FFingerprintAuthenticationHelp.Partial when !string.IsNullOrEmpty(texts.Partial) => texts.Partial,
                FFingerprintAuthenticationHelp.Insufficient when !string.IsNullOrEmpty(texts.Insufficient) => texts.Insufficient,
                FFingerprintAuthenticationHelp.Dirty when !string.IsNullOrEmpty(texts.Dirty) => texts.Dirty,
                _ => nativeText,
            };
        }

        public static Typeface FindFontExpand(string fontFamily, TypefaceStyle typefaceStyle)
        {
            if (!string.IsNullOrWhiteSpace(fontFamily))
            {
                var (success, typeface) = TryGetFromAssets(fontFamily);
                return success ? typeface : Typeface.Create(Typeface.Default, typefaceStyle);
            }

            return Typeface.Create(Typeface.Default, typefaceStyle);
        }

        public static (bool success, Typeface typeface) TryGetFromAssets(string fontName)
        {
            var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
            if (hasFontAlias)
                return (true, Typeface.CreateFromFile(fontPostScriptName));

            var isAssetFont = IsAssetFontFamily(fontName);
            if (isAssetFont)
            {
                return LoadTypefaceFromAsset(fontName);
            }

            var folders = new[]
            {
                "",
                "Fonts/",
                "fonts/",
            };


            var fontFile = FontFile.FromString(fontName);
            if (!string.IsNullOrWhiteSpace(fontFile.Extension))
            {
                var (hasFont, fontPath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
                if (hasFont)
                {
                    return (true, Typeface.CreateFromFile(fontPath));
                }
            }
            else
            {
                foreach (var ext in FontFile.Extensions)
                {
                    var formated = fontFile.FileNameWithExtension(ext);
                    var (hasFont, fontPath) = FontRegistrar.HasFont(formated);
                    if (hasFont)
                    {
                        return (true, Typeface.CreateFromFile(fontPath));
                    }

                    foreach (var folder in folders)
                    {
                        formated = $"{folder}{fontFile.FileNameWithExtension()}#{fontFile.PostScriptName}";
                        var result = LoadTypefaceFromAsset(formated);
                        if (result.success)
                            return result;
                    }

                }
            }

            return (false, null);
        }

        public static (bool success, Typeface typeface) LoadTypefaceFromAsset(string fontfamily)
        {
            try
            {
                var result = Typeface.CreateFromAsset(AApp.Context.Assets, FontNameToFontFile(fontfamily));
                return (true, result);
            }
            catch
            {
                return (false, null);
            }
        }

        public static bool IsAssetFontFamily(string name)
        {
            return Regex.IsMatch(name, LoadFromAssetsRegex);
        }

        public static ICharSequence CreateCharSequence(string text, int size, string family, TypefaceStyle style)
        {
            var span = new SpannableString(text);
            span.SetSpan(new AbsoluteSizeSpan(size, true), 0, span.Length(), SpanTypes.InclusiveExclusive);
            span.SetSpan(new FTypefaceSpan(family, FindFontExpand(family, style)), 0, span.Length(), SpanTypes.InclusiveExclusive);
            return span;
        }

        public static async Task<Bitmap> ToImageFromImageSource(this ImageSource imageSource, Context context)
        {
            IImageSourceHandler handler;
            if (imageSource is FileImageSource)
                handler = new FileImageSourceHandler();
            else if (imageSource is StreamImageSource)
                handler = new StreamImagesourceHandler();
            else if (imageSource is UriImageSource)
                handler = new ImageLoaderSourceHandler();
            else handler = new FontImageSourceHandler();

            return await handler.LoadImageAsync(imageSource, context);
        }

        public static (string Provider, float Accuracy) GetBestProvider(LocationManager locationManager, FLocationAccuracy accuracy)
        {
            var criteria = new Criteria
            {
                BearingRequired = false,
                AltitudeRequired = false,
                SpeedRequired = false
            };
            var accuracyDistance = 100;
            switch (accuracy)
            {
                case FLocationAccuracy.Lowest:
                    criteria.Accuracy = Accuracy.NoRequirement;
                    criteria.HorizontalAccuracy = Accuracy.NoRequirement;
                    criteria.PowerRequirement = LocationPower.NoRequirement;
                    accuracyDistance = 500;
                    break;
                case FLocationAccuracy.Low:
                    criteria.Accuracy = Accuracy.Coarse;
                    criteria.HorizontalAccuracy = Accuracy.Low;
                    criteria.PowerRequirement = LocationPower.Low;
                    accuracyDistance = 500;
                    break;
                case FLocationAccuracy.Default:
                case FLocationAccuracy.Medium:
                    criteria.Accuracy = Accuracy.Coarse;
                    criteria.HorizontalAccuracy = Accuracy.Medium;
                    criteria.PowerRequirement = LocationPower.Medium;
                    accuracyDistance = 250;
                    break;
                case FLocationAccuracy.High:
                    criteria.Accuracy = Accuracy.Fine;
                    criteria.HorizontalAccuracy = Accuracy.High;
                    criteria.PowerRequirement = LocationPower.High;
                    accuracyDistance = 100;
                    break;
                case FLocationAccuracy.Best:
                    criteria.Accuracy = Accuracy.Fine;
                    criteria.HorizontalAccuracy = Accuracy.High;
                    criteria.PowerRequirement = LocationPower.High;
                    accuracyDistance = 50;
                    break;
            }

            var provider = locationManager.GetBestProvider(criteria, true) ?? locationManager.GetProviders(true).FirstOrDefault();
            return (provider, accuracyDistance);
        }

        public static bool IsBetterLocation(this AndroidLocation location, AndroidLocation bestLocation)
        {
            if (bestLocation == null)
                return true;

            var timeDelta = location.Time - bestLocation.Time;

            var isSignificantlyNewer = timeDelta > TwoMinutes;
            var isSignificantlyOlder = timeDelta < -TwoMinutes;
            var isNewer = timeDelta > 0;

            if (isSignificantlyNewer)
                return true;

            if (isSignificantlyOlder)
                return false;

            var accuracyDelta = (int)(location.Accuracy - bestLocation.Accuracy);
            var isLessAccurate = accuracyDelta > 0;
            var isMoreAccurate = accuracyDelta < 0;
            var isSignificantlyLessAccurage = accuracyDelta > 200;

            var isFromSameProvider = location?.Provider?.Equals(bestLocation?.Provider, StringComparison.OrdinalIgnoreCase) ?? false;

            if (isMoreAccurate)
                return true;

            if (isNewer && !isLessAccurate)
                return true;

            if (isNewer && !isSignificantlyLessAccurage && isFromSameProvider)
                return true;

            return false;
        }

        public static BlendMode GetFilterMode(FilterMode mode)
        {
            return mode switch
            {
                FilterMode.SrcIn => BlendMode.SrcIn,
                FilterMode.Multiply => BlendMode.Multiply,
                _ => BlendMode.SrcAtop,
            };
        }

        [Obsolete]
        static PorterDuff.Mode GetFilterModePre29(FilterMode mode)
        {
            return mode switch
            {
                FilterMode.SrcIn => PorterDuff.Mode.SrcIn,
                FilterMode.Multiply => PorterDuff.Mode.Multiply,
                _ => PorterDuff.Mode.SrcAtop,
            };
        }

        public static AColorFilter GetColorFilter(this ADrawable drawable)
        {
            if (drawable == null)
                return null;

            return ADrawableCompat.GetColorFilter(drawable);
        }

        public static void SetColorFilter(this ADrawable drawable, AColorFilter colorFilter)
        {
            if (drawable == null)
                return;

            if (colorFilter == null)
                ADrawableCompat.ClearColorFilter(drawable);

            drawable.SetColorFilter(colorFilter);
        }


        public static void SetColorFilter(this ADrawable drawable, Color color, AColorFilter defaultColorFilter, FilterMode mode)
        {
            if (drawable == null)
                return;

            if (color == Color.Default)
            {
                SetColorFilter(drawable, defaultColorFilter);
                return;
            }

            drawable.SetColorFilter(color.ToAndroid(), mode);
        }

        public static void SetColorFilter(this ADrawable drawable, Color color, FilterMode mode)
        {
            drawable.SetColorFilter(color.ToAndroid(), mode);
        }

        public static void SetColorFilter(this ADrawable drawable, AColor color, FilterMode mode)
        {
            if ((int)global::Android.OS.Build.VERSION.SdkInt >= 29)
                drawable.SetColorFilter(new BlendModeColorFilter(color, GetFilterMode(mode)));
            else
                drawable.SetColorFilter(color, GetFilterModePre29(mode));
        }

        public static ImeAction ToAndroidImeAction(this ReturnType returnType)
        {
            return returnType switch
            {
                ReturnType.Go => ImeAction.Go,
                ReturnType.Next => ImeAction.Next,
                ReturnType.Send => ImeAction.Send,
                ReturnType.Search => ImeAction.Search,
                ReturnType.Done => ImeAction.Done,
                ReturnType.Default => ImeAction.Done,
                _ => ImeAction.Done,
            };
        }
    }
}