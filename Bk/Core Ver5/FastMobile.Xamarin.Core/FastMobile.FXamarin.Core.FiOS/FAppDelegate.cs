using CoreFoundation;
using Firebase.CloudMessaging;
using Firebase.Core;
using Foundation;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfAutoComplete.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.SfDataGrid.XForms.iOS;
using Syncfusion.SfImageEditor.XForms.iOS;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using Syncfusion.SfPdfViewer.XForms.iOS;
using Syncfusion.SfPicker.XForms.iOS;
using Syncfusion.SfPullToRefresh.XForms.iOS;
using Syncfusion.SfRangeSlider.XForms.iOS;
using Syncfusion.XForms.iOS.Backdrop;
using Syncfusion.XForms.iOS.BadgeView;
using Syncfusion.XForms.iOS.Border;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.ComboBox;
using Syncfusion.XForms.iOS.Core;
using Syncfusion.XForms.iOS.EffectsView;
using Syncfusion.XForms.iOS.Expander;
using Syncfusion.XForms.iOS.Graphics;
using Syncfusion.XForms.iOS.MaskedEdit;
using Syncfusion.XForms.iOS.PopupLayout;
using Syncfusion.XForms.iOS.TabView;
using Syncfusion.XForms.iOS.TextInputLayout;
using System.Linq;
using UIKit;
using UserNotifications;
using Xamarin;
using Xamarin.Forms.Platform.iOS;
using XamIQKeyboardManager;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FAppDelegate : FormsApplicationDelegate
    {
        public virtual FLocalNotificationDelegate Delegate { get; set; } = new FLocalNotificationDelegate();

        protected virtual FApplication FormsApp { get; set; }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
#if DEBUG
            Messaging.SharedInstance.SetApnsToken(deviceToken, ApnsTokenType.Sandbox);
#else
            Messaging.SharedInstance.SetApnsToken(deviceToken, ApnsTokenType.Production);
#endif
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return;
            }

            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationList) =>
            {
                if (notificationList.Any())
                {
                    return;
                }

                uiApplication.InvokeOnMainThread(() =>
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                });
            });
            base.OnActivated(uiApplication);
        }

        public void InitAll(UIApplication app, NSDictionary options)
        {
            FormsMaps.Init();
            RegisterNotify();
            InitFirebase();
            InitSyncfusionIOS();
            InitNotificationCenter();
            Delegate.SetAction(options, app.ApplicationState);
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            InitIQKeyboardManager();
        }

        public void InitSyncfusionIOS()
        {
            SfTabViewRenderer.Init();
            SfAutoCompleteRenderer.Init();
            SfComboBoxRenderer.Init();
            SfTextInputLayoutRenderer.Init();
            SfPopupLayoutRenderer.Init();
            SfBusyIndicatorRenderer.Init();
            SfChartRenderer.Init();
            SfPickerRenderer.Init();
            SfNumericTextBoxRenderer.Init();
            SfPdfDocumentViewRenderer.Init();
            SfRangeSliderRenderer.Init();
            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();
            SfPullToRefreshRenderer.Init();
            SfBorderRenderer.Init();
            SfButtonRenderer.Init();
            SfMaskedEditRenderer.Init();
            SfRadioButtonRenderer.Init();
            SfAvatarViewRenderer.Init();
            SfDataGridRenderer.Init();
            SfGradientViewRenderer.Init();
            SfExpanderRenderer.Init();
            SfBackdropPageRenderer.Init();
            SfImageEditorRenderer.Init();
            SfBadgeViewRenderer.Init();
            SfCheckBoxRenderer.Init();
        }

        public void InitFirebase()
        {
            Configuration Config = Configuration.SharedInstance;
            App.Configure();
            Messaging.SharedInstance.Delegate = Delegate;
        }

        public void InitNotificationCenter()
        {
            FNotificationCenter.Init();
        }

        public void RegisterNotify()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.Delegate = Delegate;
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, err) =>
                {
                    if (approved)
                        DispatchQueue.MainQueue.DispatchAsync(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                });
            }
            else
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            FormsApp?.OnFinish();
            base.WillTerminate(uiApplication);
        }

        public void InitIQKeyboardManager()
        {
            var manager = IQKeyboardManager.SharedManager();
            manager.Enable = true;
            manager.EnableAutoToolbar = false;
            manager.LayoutIfNeededOnUpdate = true;
        }
    }
}