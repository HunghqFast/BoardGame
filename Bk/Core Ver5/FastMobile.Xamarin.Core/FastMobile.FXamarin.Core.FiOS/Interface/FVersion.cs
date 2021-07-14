using Foundation;
using System;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core.FiOS
{
    public class FVersion : IFVersion
    {
        private class App
        {
            public string Version { get; set; }
            public string Url { get; set; }
        }

        private App app;
        private string bundleIdentifier => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleIdentifier").ToString();
        private string bundleVersion => NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        public string InstalledVersionNumber => bundleVersion;

        public async Task<bool> IsUsingLatestVersion()
        {
            try
            {
                string latestVersion = await GetLatestVersionNumber();
                if (String.IsNullOrEmpty(latestVersion))
                    return true;
                return Version.Parse(latestVersion).CompareTo(Version.Parse(bundleVersion)) <= 0;
            }
            catch
            {
                return true;
            }
        }

        public async Task<string> GetLatestVersionNumber()
        {
            app = await LookupApp();
            return app?.Version;
        }

        public async Task OpenAppInStore()
        {
            if (app == null)
                app = await LookupApp();
            if (app == null || string.IsNullOrEmpty(app.Url))
                return;
            UIKit.UIApplication.SharedApplication.OpenUrl(new NSUrl($"{app.Url}"));
        }

        private async Task<App> LookupApp()
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMddHHmmss");
                var respone = await new HttpClient().GetStringAsync($"http://itunes.apple.com/lookup?bundleId={bundleIdentifier}&t={now}");
                if (string.IsNullOrEmpty(respone))
                    return null;

                var appLookup = JsonValue.Parse(respone);
                if (appLookup["resultCount"] == 0)
                {
                    return new App
                    {
                        Version = bundleVersion,
                        Url = ""
                    };
                }
                return new App
                {
                    Version = appLookup["results"][0]["version"],
                    Url = appLookup["results"][0]["trackViewUrl"]
                };
            }
            catch
            {
                return null;
            }
        }
    }
}