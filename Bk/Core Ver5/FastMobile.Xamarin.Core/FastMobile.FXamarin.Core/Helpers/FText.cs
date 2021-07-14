using System.Runtime.CompilerServices;

namespace FastMobile.FXamarin.Core
{
    public static class FText
    {
        public const string NotifyActionCharacter = "þ", NotifyGroupKey = "group", NotifyCodeKey = "notifyCode", NotifyBodyKey = "body", NotifyActionKey = "action", CommandAccept = "OK", CommandCancel = "Cancel", CommandYes = "Yes", CommandNo = "No", CommandUpdate = "Update";
        public static FResourceManager Manager { get; set; }
        public static string ApplicationTitle => AttributeString(FSetting.AppMode.ToString().ToLower());
        public static string Accept => String();
        public static string Cancel => String();
        public static string Yes => String();
        public static string No => String();
        public static string Update => String();

        public static string Downloading => String();

        public static string PickDay => String();
        public static string PickHour => String();
        public static string Day => String();
        public static string Month => String();
        public static string Year => String();
        public static string Hour => String();
        public static string Minute => String();

        public static string NewRecord => String();
        public static string SelectFile => String();
        public static string ClearAll => String();
        public static string Download => String();
        public static string Edit => String();
        public static string Approve => String();
        public static string Close => String();
        public static string Done => String();
        public static string Flash => String();
        public static string Save => String();
        public static string NewAction => String();
        public static string Detail => String();
        public static string Select => String();
        public static string SelectRecord => String();

        public static string CaptureImage => String();
        public static string CaptureVideo => String();
        public static string SelectImage => String();
        public static string SelectVideo => String();
        public static string UnCheck => String();
        public static string EditImage => String();

        public static string LocationAnnotation => String();
        public static string LocationCurrent => String();
        public static string LocationUnknown => String();
        public static string LocationSelect => String();

        public static string ViewMore => String();
        public static string DeviceUnSupport => String();
        public static string AuthenConfirmAppend => String();

        public static string ReportTotal => String();
        public static string ReportRecord => String();

        public static string Delete => String();
        public static string MarkUnread => String();
        public static string MarkRead => String();

        public static string NoData => String();
        public static string NoDataNoAccess => String();
        public static string Developing => String();

        public static string Version => String();
        public static string Copyright => String();

        public static string ChangePassword => String();
        public static string ConfirmPassword => String();
        public static string PasswordHint => String();
        public static string ReNewPasswordHint => String();

        public static string ProfileDeclare => String();
        public static string Language => String();
        public static string UserNameHint => String();
        public static string Login => String();
        public static string Logout => String();
        public static string ForgotPassword => String();

        public static string FilterCondition => String();
        public static string QuickFind => String();
        public static string Link => String();
        public static string Alias => String();
        public static string CompanyCode => String();

        public static string ResetPassword => String();
        public static string Name => String();
        public static string Email => String();
        public static string Code => String();

        public static string Dashboard => String();
        public static string Report => String();
        public static string Entry => String();
        public static string Notify => String();
        public static string Others => String();

        public static string AboutProduct => String();
        public static string Settings => String();
        public static string ReportSetting => String();
        public static string ApplicationSetting => String();
        public static string CheckUpdate => String();
        public static string CloseScreen => String();

        public static string General => String();
        public static string Approval => String();
        public static string System => String();
        public static string News => String();
        public static string Promotion => String();

        public static string ErrorDefault => String();
        public static string ReloadThisPage => String();
        public static string NoInternet => String();
        public static string TryConnectInternet => String();

        public static string Continue => String();

        static FText()
        {
            Manager = new FResourceManager("FastMobile.FXamarin.Core.Resources.Config.xml", "Text");
        }

        public static string String([CallerMemberName] string name = "")
        {
            return Manager.GetString(name);
        }

        public static string AttributeString(string attribute, [CallerMemberName] string name = "")
        {
            return Manager.GetString(name, "", attribute);
        }
    }
}