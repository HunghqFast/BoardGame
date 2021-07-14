using System;

namespace FastMobile.FXamarin.Core
{
    public static class FString
    {
        //static FString()
        //{
        //    if (string.IsNullOrEmpty(UserID))
        //        RemoveCurrentProrfile();
        //}

        public static void SetCurrentProfile(FItemProfile profile)
        {
            ServiceUrl = profile.Link;
            ServiceName = profile.Name;
            ServiceDatabase = profile.DatabaseName;
            ServiceInternal = profile.IsInternal;
            ServiceID = profile.ID.ToString();
            FServices.SetUrl(profile.Link);
        }

        public static void RemoveCurrentProrfile()
        {
            UserID = "";
            ServiceUrl = "";
            ServiceName = "";
            ServiceInternal = "";
            ServiceDatabase = "";
            ServiceID = "1";
            FServices.SetUrl("");
        }

        public static void UpdatePassword(string pass)
        {
            Password = pass;
        }

        public static string ServiceUrl
        {
            get => "FastMobile.FXamarin.Core.FString.ServiceUrl".GetCache();
            private set => value.SetCache("FastMobile.FXamarin.Core.FString.ServiceUrl");
        }

        public static string ServiceName
        {
            get => "FastMobile.FXamarin.Core.FString.ServiceName".GetCache();
            private set => value.SetCache("FastMobile.FXamarin.Core.FString.ServiceName");
        }

        public static string ServiceDatabase
        {
            get => "FastMobile.FXamarin.Core.FString.ServiceDatabase".GetCache();
            private set => value.SetCache("FastMobile.FXamarin.Core.FString.ServiceDatabase");
        }

        public static string ServiceInternal
        {
            get => "FastMobile.FXamarin.Core.FString.ServiceInternal".GetCache();
            private set => value.SetCache("FastMobile.FXamarin.Core.FString.ServiceInternal");
        }

        public static string ServiceID
        {
            get => "FastMobile.FXamarin.Core.FString.ServiceID".GetCache();
            private set => value.SetCache("FastMobile.FXamarin.Core.FString.ServiceID");
        }

        public static string Username
        {
            get => Get("FastMobile.FXamarin.Core.FString.Username");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.Username.{ServiceID}");
        }

        public static string Password
        {
            get => Get("FastMobile.FXamarin.Core.FString.Password");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.Password.{ServiceID}");
        }

        public static string UserID
        {
            get => Get("FastMobile.FXamarin.Core.FString.UserID");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.UserID.{ServiceID}");
        }

        public static bool Admin
        {
            get => Convert.ToBoolean(Get("FastMobile.FXamarin.Core.FString.Admin", bool.FalseString));
            internal set => value.SetCache("FastMobile.FXamarin.Core.FString.Admin");
        }

        public static string Comment
        {
            get => Get("FastMobile.FXamarin.Core.FString.Comment");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.Comment.{ServiceID}");
        }

        public static string Comment2
        {
            get => Get("FastMobile.FXamarin.Core.FString.Comment2");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.Comment2.{ServiceID}");
        }

        public static string DLanguage
        {
            get => Get("FastMobile.FXamarin.Core.FString.DLanguage");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.DLanguage.{ServiceID}");
        }

        public static string Status
        {
            get => Get("FastMobile.FXamarin.Core.FString.Status");
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.Status.{ServiceID}");
        }

        public static int TimeOut
        {
            get => Convert.ToInt32(Get("FastMobile.FXamarin.Core.FString.TimeOut", "43200"));
            internal set => value.SetCache($"FastMobile.FXamarin.Core.FString.TimeOut.{ServiceID}");
        }

        public static string SenderID
        {
            get => Get("FastMobile.FXamarin.Core.FString.SenderID");
            set => value.SetCache($"FastMobile.FXamarin.Core.FString.SenderID.{ServiceID}");
        }

        private static string Get(string key, string defaultValue = "")
        {
            var cur = $"{key}.{ServiceID}".GetCache(defaultValue);
            if (!string.IsNullOrEmpty(cur))
                return cur;
            return key.GetCache(defaultValue);
        }
    }
}