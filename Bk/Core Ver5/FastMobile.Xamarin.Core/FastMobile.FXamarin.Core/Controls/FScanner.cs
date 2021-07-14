using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace FastMobile.FXamarin.Core
{
    public class FScanner : SfEffectsView
    {
        public event EventHandler<Result> OnFscannerCommpleted;

        public FScanner(double width, double height, FontImageSource icon)
        {
            var s = new StackLayout();
            var i = new Image();
            var l = false;

            i.HorizontalOptions = i.VerticalOptions = LayoutOptions.Center;
            i.HeightRequest = i.WidthRequest = icon.Size;
            i.Margin = new Thickness(0, (height - icon.Size) * 0.5, 0, 0);
            i.Source = icon;

            s.Margin = 0;
            s.Padding = new Thickness(5, 5);
            s.Spacing = 5;
            s.WidthRequest = width;
            s.HeightRequest = height;
            s.HorizontalOptions = s.VerticalOptions = LayoutOptions.CenterAndExpand;
            s.Children.Add(i);

            Content = s;
            AutoResetEffect = AutoResetEffects.Ripple;
            SelectionColor = HighlightColor = Color.Transparent;
            TouchUp += async (s, e) =>
            {
                lock (s) { if (l) return; l = true; }
                OnFscannerCommpleted?.Invoke(s, await Scanning());
                l = false;
            };
            WidthRequest = width;
            LongPressEffects = TouchUpEffects = TouchDownEffects = SfEffects.Ripple;
            VerticalOptions = HorizontalOptions = LayoutOptions.CenterAndExpand;
        }

        private async Task<Result> Scanning()
        {
            if (!await FPermissions.HasPermission<Permissions.Camera>(true) || !await FPermissions.HasPermission<Permissions.Flashlight>(true))
            {
                FPermissions.ShowMessage();
                return null;
            }

            var c = FText.Close;
            var f = FText.Flash;
            var e = FText.DeviceUnSupport;
            var s = new MobileBarcodeScanner { CancelButtonText = c, FlashButtonText = f, CameraUnsupportedMessage = e, UseCustomOverlay = true };

            if (FSetting.IsAndroid)
            {
                return await s.Scan();
            }
            else
            {
                var o = new MobileBarcodeScanningOptions
                {
                    CameraResolutionSelector = Camera
                };
                return await s.Scan(o);
            }
        }

        private CameraResolution Camera(List<CameraResolution> o)
        {
            return o[^1];
        }
    }
}