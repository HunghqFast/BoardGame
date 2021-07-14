using Syncfusion.SfImageEditor.XForms;
using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FApplication : Application
    {
        public FPage Dashboard { get; protected set; }
        public FPageMenu Report { get; protected set; }
        public FPageMenu Entry { get; protected set; }
        public FPageMenu Others { get; protected set; }
        public FPageNotify Notify { get; protected set; }

        public static string NotifyID
        {
            get => FUtility.GetCache("FastMobile.FXamarin.Core.FApplication.NotifyID");
            set => value.SetCache("FastMobile.FXamarin.Core.FApplication.NotifyID");
        }

        public static string NotifyGroup
        {
            get => FUtility.GetCache("FastMobile.FXamarin.Core.FApplication.NotifyNotifyGroup");
            set => value.SetCache("FastMobile.FXamarin.Core.FApplication.NotifyNotifyGroup");
        }

        public static string NotifyAction
        {
            get => FUtility.GetCache("FastMobile.FXamarin.Core.FApplication.NotifyNotifyAction");
            set => value.SetCache("FastMobile.FXamarin.Core.FApplication.NotifyNotifyAction");
        }

        public FApplication()
        {
            FLicense.RegisterLicense();
            Device.SetFlags(new string[] { "FastRenderers_Experimental", "UseLegacyRenderers", "Expander_Experimental", "Brush_Experimental", "Markup_Experimental", "Shapes_Experimental" });
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            ImageEditorResourceManager.Manager = new ResourceManager($"FastMobile.FXamarin.Core.Resources.FImageEditor{FSetting.Language}", typeof(FApplication).Assembly);

            FInterface.IFAndroid.TransparentStatusBar();
            FNotificationCenter.Current.NotificationTapped -= OnLocalNotificationTapped;
            FNotificationCenter.Current.NotificationTapped += OnLocalNotificationTapped;
            FSetting.LanguageChanged -= OnLanguageChanged;
            FSetting.LanguageChanged += OnLanguageChanged;
        }

        public virtual void OnFinish()
        {
            FNotificationCenter.Current.NotificationTapped -= OnLocalNotificationTapped;
        }

        protected virtual void OnLocalNotificationTapped(FNotificationTappedEventArgs e)
        {
        }

        protected override void OnStart()
        {
            FAlert.UpdateCanAlert();
            base.OnStart();
        }

        protected override void OnResume()
        {
            FAlert.UpdateCanAlert();
            base.OnResume();
        }

        protected virtual void OnLanguageChanged(object sender, EventArgs e)
        {
            ImageEditorResourceManager.Manager = new ResourceManager($"FastMobile.FXamarin.Core.Resources.FImageEditor{FSetting.Language}", typeof(FApplication).Assembly);
        }
    }
}