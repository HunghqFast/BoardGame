using Android.Content;
using Android.Runtime;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application = Android.App.Application;
using Net = Android.Net;

namespace FastMobile.FXamarin.Core.FAndroid
{
    [Preserve(AllMembers = true)]
    public class FVersion : IFVersion
    {
        string PackageName => Application.Context.PackageName;
        string VersionName => Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0).VersionName;

        public string InstalledVersionNumber => VersionName;

        public async Task<bool> IsUsingLatestVersion()
        {
            try
            {
                string latestVersion = await GetLatestVersionNumber();
                if (string.IsNullOrEmpty(latestVersion))
                    return true;
                return Version.Parse(latestVersion).CompareTo(Version.Parse(VersionName)) <= 0;
            }
            catch
            {
                return true;
            }
        }

        public async Task<string> GetLatestVersionNumber()
        {
            if (string.IsNullOrEmpty(PackageName))
            {
                return ".";
            }
            var version = string.Empty;
            var url = $"https://play.google.com/store/apps/details?id={PackageName}&hl=en";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using var handler = new HttpClientHandler();
                using var client = new HttpClient(handler);
                using var responseMsg = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                if (responseMsg == null)
                    return version;
                if (!responseMsg.IsSuccessStatusCode)
                    return version;

                try
                {
                    var content = responseMsg.Content == null ? null : await responseMsg.Content.ReadAsStringAsync();

                    var versionMatch = Regex.Match(content, "<div[^>]*>Current Version</div><span[^>]*><div[^>]*><span[^>]*>(.*?)<").Groups[1];

                    if (versionMatch.Success)
                    {
                        version = versionMatch.Value.Trim();
                    }
                }
                catch
                {
                    return version;
                }
            }
            return version;
        }

        public Task OpenAppInStore()
        {
            try
            {
                var intent = new Intent(Intent.ActionView, Net.Uri.Parse($"market://details?id={PackageName}"));
                intent.SetPackage("com.android.vending");
                intent.AddFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                var intent = new Intent(Intent.ActionView, Net.Uri.Parse($"https://play.google.com/store/apps/details?id={PackageName}"));
                intent.AddFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
            }

            return Task.FromResult(true);
        }
    }
}