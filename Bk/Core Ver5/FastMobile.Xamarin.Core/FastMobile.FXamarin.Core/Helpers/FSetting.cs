using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FSetting
    {
        public static FResourceManager Manager { get; set; }

        public static event EventHandler<EventArgs> LanguageChanged;

        static FSetting()
        {
#if DEBUG
            RunMode = FRuntimeMode.Debug;
            IsDebug = true;
#else
            RunMode = FRuntimeMode.Release;
            IsDebug = false;
#endif
            InternalLanguage = InternalLanguage == "E" ? "E" : "V";
            IsAndroid = DeviceInfo.Platform == DevicePlatform.Android;
            var avaiable = FInterface.IFFingerprint.GetAvailability();
            CanUseMachineSecurity = avaiable != FFingerprintAvailability.NoSensor && avaiable != FFingerprintAvailability.NoApi;
            DeviceID = string.IsNullOrEmpty(DeviceID) ? FInterface.IFEnvironment.DeviceID : DeviceID;
            Manager = new FResourceManager(@"FastMobile.FXamarin.Core.Resources.Config.xml", "Setting");
            ScreenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
            ScreenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            Base();
        }

        public static void Init(FAppMode appMode, FConfigMode configMode)
        {
            AppMode = appMode;
            ConfigMode = configMode;
        }

        private static void Base()
        {
            FontIcon = GetValue("FontIcon", "FontIcons");
            FontIconName = GetValue("FontIconName", "Material Design Icons");
            FontIconFileName = GetValue("FontIconFileName", "Icons455.ttf");
            FontText = GetValue("FontText", "Roboto-Regular");
            FontTextMedium = GetValue("FontTextMedium", "Roboto-Medium");
            FontTextBold = GetValue("FontTextBold", "Roboto-Bold");
            FontTextItalic = GetValue("FontTextItalic", "Roboto-Italic");
            FontTextName = GetValue("FontTextName", "Roboto");
            FontTextFileName = GetValue("FontTextFileName", "Roboto-Regular.ttf");

            SpacingButtons = Convert.ToInt32(GetValue("SpacingButtons", "5"));
            FontSizePercent = GetResultWithScreenWidth(GetValue("FontSizePercent", "-1,3,-1,0,1,2,3"));
            FontSizeLabelContent = GetResultWithScreenWidth(GetValue("FontSizeLabelContent", "12,13,14,15,16"));
            FontSizeLabelHint = GetResultWithScreenWidth(GetValue("FontSizeLabelHint", "11,12,13,14,15"));
            FontSizeLabelTitle = GetResultWithScreenWidth(GetValue("FontSizeLabelTitle", "13,14,15,16,17"));
            FontSizeButton = GetResultWithScreenWidth(GetValue("FontSizeButton", "13,14,15,16,17"));
            SizeButtonIcon = GetResultWithScreenWidth(GetValue("SizeButtonIcon", "26,28,30,32"));
            SizeIconToolbar = GetResultWithScreenWidth(GetValue("SizeIconToolbar", "18,20,22,24"));
            SizeIconButton = GetResultWithScreenWidth(GetValue("SizeIconButton", "22,24,26,28,30"));
            SizeIconShow = GetResultWithScreenWidth(GetValue("SizeIconShow", "18,20,22,24"));
            SizeIconLegend = GetResultWithScreenWidth(GetValue("SizeIconLegend", "16,17,18,19,20"));
            SizeIconMenu = GetResultWithScreenWidth(GetValue("SizeIconMenu", "22,24,26,28,30"));
            FontSizeBadge = GetResultWithScreenWidth(GetValue("FontSizeBadge", "8,8,9,9,10"));
            HeightChart = GetResultWithScreenHeight(GetValue("HeightChart", "430,490,550,600"));
            FontSizeLabelSelectedDate = GetResultWithScreenWidth(GetValue("FontSizeLabelSelectedDate", "20,20,20,20,21,22,23,24,25,26,27,28,29,30"));
            RadiusPopup = GetResultWithScreenWidth(GetValue("RadiusPopup", "30,32,34,36,38,40"));
            FilterDatePickerWidth = GetValueDouble("FilterDatePickerWidth", "330");
            BusySize = GetValueDouble("BusySize", "40");
            HeightRowView = GetValueDouble("HeightRowView", "60");
            HeightRowGrid = GetValueInt("HeightRowGrid", "40");
            RadiusButton = GetValueInt("RadiusButton", "5");
            FilterInputHeight = GetValueInt("FilterInputHeight", "45");
            StartColor = Color.FromHex(GetValue("StartColor", "#1d9ea1"));
            EndColor = Color.FromHex(GetValue("EndColor", "#1f80b5"));
            PrimaryColor = Color.FromHex(GetValue("PrimaryColor", "#1470F0"));
            SecondaryColor = Color.FromHex(GetValue("SecondaryColor", "#6c757d"));
            SuccessColor = Color.FromHex(GetValue("SuccessColor", "#28a745"));
            InfoColor = Color.FromHex(GetValue("InfoColor", "#32b9bb"));
            WarningColor = Color.FromHex(GetValue("WarningColor", "#ffa500"));
            DangerColor = Color.FromHex(GetValue("DangerColor", "#f3425f"));
            LightColor = Color.FromHex(GetValue("LightColor", "#ffffff"));
            DarkColor = Color.FromHex(GetValue("DarkColor", "#343a40"));
            CheckColor = Color.FromHex(GetValue("CheckColor", "#0c6e93"));
            ColorTime = Color.FromHex(GetValue("ColorTime", "#4790f5"));
            BackgroundMain = Color.FromHex(GetValue("BackgroundMain", "#ffffff"));
            BackgroundSpacing = Color.FromHex(GetValue("BackgroundSpacing", "#F4F4F4"));
            BackgroundAlternatingRow = Color.FromHex(GetValue("BackgroundAlternatingRow", "#f7fcff"));
            LineBoxReportColor = Color.FromHex(GetValue("LineBoxReportColor", "#DDDDDD"));
            TextColorContent = Color.FromHex(GetValue("TextColorContent", "#212121"));
            TextColorTitle = Color.FromHex(GetValue("TextColorTitle", "#212121"));
            DisableColor = Color.FromHex(GetValue("DisableColor", "#808080"));
            BusyColor = IsAndroid ? Color.FromHex(GetValue("BusyColor" + "Android", "#548ff0")) : Color.FromHex(GetValue("BusyColor" + "iOS", "#000000"));
        }

        #region Methods

        public static int GetResultPercentWithScreenWidth(string listResult)
        {
            var arr = listResult.Split(",");
            int s = 325;

            if (arr.Length == 0)
                return -1;
            if (arr.Length == 1)
                return Convert.ToInt32(arr[0]);
            if (ScreenWidth <= s)
                return Convert.ToInt32(ScreenWidth / Double.Parse(arr[0]));
            foreach (var val in arr)
            {
                if (ScreenWidth > s && ScreenWidth <= (s + 50))
                    return Convert.ToInt32(ScreenWidth / Double.Parse(val));
                s += 50;
            }
            return Convert.ToInt32(ScreenWidth / Double.Parse(arr[^1]));
        }

        public static double ArrayToDoubleByPercentAndScreenWidth(double[] arr, FSizeType type)
        {
            int s = 275;
            if (arr == null || arr.Length == 0)
                return -1;
            if (arr.Length == 1)
                return arr[0] == -1 ? -1 : type == FSizeType.Ratio ? ScreenWidth * arr[0] : arr[0];
            if (ScreenWidth <= s)
                return type == FSizeType.Ratio ? ScreenWidth * arr[0] : arr[0];
            foreach (var val in arr)
            {
                if (ScreenWidth > s && ScreenWidth <= (s + 50))
                    return type == FSizeType.Ratio ? ScreenWidth * val : val;
                s += 50;
            }
            return type == FSizeType.Ratio ? ScreenWidth * arr[^1] : arr[^1];
        }

        public static int GetResultWithScreenWidth(string listResult)
        {
            var arr = listResult.Split(",");
            int s = 325;

            if (arr.Length == 0)
                return -1;
            if (arr.Length == 1)
                return Convert.ToInt32(arr[0]);
            if (ScreenWidth <= s)
                return Convert.ToInt32(arr[0]);
            foreach (var val in arr)
            {
                if (ScreenWidth > s && ScreenWidth <= (s + 50))
                    return Convert.ToInt32(val);
                s += 50;
            }
            return Convert.ToInt32(arr[^1]);
        }

        public static int GetResultWithScreenHeight(string listResult)
        {
            var arr = listResult.Split(",");
            int s = 500;

            if (arr.Length == 0)
                return -1;
            if (arr.Length == 1)
                return Convert.ToInt32(arr[0]);
            if (ScreenHeight <= s)
                return Convert.ToInt32(arr[0]);
            foreach (var val in arr)
            {
                if (ScreenHeight > s && ScreenHeight <= (s + 100))
                    return Convert.ToInt32(val);
                s += 100;
            }
            return Convert.ToInt32(arr[^1]);
        }

        public static int GetResultPercentWithScreenHeight(string listResult)
        {
            var arr = listResult.Split(",");
            int s = 500;

            if (arr.Length == 0)
                return -1;
            if (arr.Length == 1)
                return Convert.ToInt32(arr[0]);
            if (ScreenHeight <= s)
                return Convert.ToInt32(ScreenHeight / Double.Parse(arr[0]));
            foreach (var val in arr)
            {
                if (ScreenHeight > s && ScreenHeight <= (s + 100))
                    return Convert.ToInt32(ScreenHeight / Double.Parse(val));
                s += 100;
            }
            return Convert.ToInt32(ScreenHeight / Double.Parse(arr[^1]));
        }

        public static string GetValue(string name, string defaultValue = "")
        {
            return Manager.GetString(name, defaultValue, "value");
        }

        public static int GetValueInt(string name, string defaultValue = "")
        {
            return Convert.ToInt32(GetValue(name, defaultValue));
        }

        public static double GetValueDouble(string name, string defaultValue = "")
        {
            return Convert.ToDouble(GetValue(name, defaultValue));
        }

        public static void UpdateUseLocalAuthen(bool value)
        {
            UseLocalAuthen = value;
        }

        #endregion Methods

        #region Publish

        public static string NetWorkKey
        {
            get => "FastMobile.FXamarin.Core.FSetting.NetworkKey".GetCache();
            internal set => value.SetCache("FastMobile.FXamarin.Core.FSetting.NetworkKey");
        }

        public static string DeviceID
        {
            get => "FastMobile.FXamarin.Core.FSetting.DeviceID".GetCache();
            internal set => value.SetCache("FastMobile.FXamarin.Core.FSetting.DeviceID");
        }

        public static string InternalLanguage
        {
            get => "FastMobile.FXamarin.Core.FSetting.Language".GetCache("V");
            internal set
            {
                Language = value;
                Language.SetCache("FastMobile.FXamarin.Core.FSetting.Language");
                LanguageChanged?.Invoke(new object(), EventArgs.Empty);
            }
        }

        public static bool UseLocalAuthen
        {
            get => CanUseMachineSecurity && Convert.ToBoolean($"FastMobile.FXamarin.Core.FSetting.UseLocalAuthen.{FString.ServiceID}".GetCache(bool.FalseString));
            private set => value.SetCache($"FastMobile.FXamarin.Core.FSetting.UseLocalAuthen.{FString.ServiceID}");
        }

        public static FConfigMode ConfigMode { get; set; }
        public static FAppMode AppMode { get; private set; }
        public static FRuntimeMode RunMode { get; private set; }

        public const int MaximumDisableAlert = 2;
        public static string Language { get; private set; }
        public static string SystemLanguage => SystemLanguageIsV ? "V" : "E";

        public static bool V => Language == "V";
        public static bool SystemLanguageIsV => CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "vi";
        public static bool IsAndroid { get; }
        public static bool HasLocaltionPermission { get; set; }
        public static bool CanUseMachineSecurity { get; }
        public static bool IsDebug { get; }

        public static int FontSizePercent { get; private set; }
        public static int FontSizeLabelContent { get; private set; }
        public static int FontSizeLabelHint { get; private set; }
        public static int FontSizeLabelTitle { get; private set; }
        public static int FontSizeButton { get; private set; }
        public static int FontSizeBadge { get; private set; }
        public static int SizeIconToolbar { get; private set; }
        public static int SizeButtonIcon { get; private set; }
        public static int SizeIconButton { get; private set; }
        public static int SizeIconShow { get; private set; }
        public static int SizeIconLegend { get; private set; }
        public static int SizeIconMenu { get; private set; }
        public static int RadiusButton { get; private set; }
        public static int FilterInputHeight { get; private set; }
        public static int FontSizeLabelSelectedDate { get; private set; }
        public static int HeightRowGrid { get; private set; }
        public static int RadiusPopup { get; private set; }

        public static double ScreenHeight { get; set; }
        public static double ScreenWidth { get; set; }
        public static double HeightChart { get; private set; }
        public static double FilterDatePickerWidth { get; private set; }
        public static double BusySize { get; private set; }
        public static double HeightRowView { get; private set; }
        public static double SpacingButtons { get; private set; }

        public static string FontIconName { get; private set; }
        public static string FontIconFileName { get; private set; }
        public static string FontIcon { get; private set; }
        public static string FontTextName { get; private set; }
        public static string FontTextFileName { get; private set; }
        public static string FontText { get; private set; }
        public static string FontTextMedium { get; private set; }
        public static string FontTextBold { get; private set; }
        public static string FontTextItalic { get; private set; }

        public static Color StartColor { get; private set; }
        public static Color EndColor { get; private set; }
        public static Color BackgroundAlternatingRow { get; private set; }
        public static Color PrimaryColor { get; private set; }
        public static Color SecondaryColor { get; private set; }
        public static Color SuccessColor { get; private set; }
        public static Color InfoColor { get; private set; }
        public static Color WarningColor { get; private set; }
        public static Color DangerColor { get; private set; }
        public static Color LightColor { get; private set; }
        public static Color DarkColor { get; private set; }
        public static Color CheckColor { get; private set; }
        public static Color ColorTime { get; private set; }
        public static Color BackgroundMain { get; private set; }
        public static Color BackgroundSpacing { get; private set; }
        public static Color LineBoxReportColor { get; private set; }
        public static Color TextColorContent { get; private set; }
        public static Color TextColorTitle { get; private set; }
        public static Color DisableColor { get; private set; }
        public static Color BusyColor { get; private set; }

        public static void ClearKey()
        {
            NetWorkKey = "";
            FServices.TicketExpireTime = DateTime.MinValue;
            FOptions.TicketMinute = 60;
            FString.SenderID = string.Empty;
        }

        #endregion Publish
    }
}