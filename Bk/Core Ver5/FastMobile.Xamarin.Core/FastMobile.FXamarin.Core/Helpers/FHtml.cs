using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FHtml
    {
        public static string DataSetToHtml(DataSet data)
        {
            if (data.Tables.Count == 0)
                return "";
            try
            {
                string header = data.Tables[data.Tables.Count - 1].Rows[0]["header"].ToString();
                string footer = data.Tables[data.Tables.Count - 1].Rows[0]["footer"].ToString();
                string detail = data.Tables[data.Tables.Count - 1].Rows[0]["detail"].ToString();
                data.Tables.ForEach<DataTable>((x) => ReplaceHeaderAndFooter(x, ref header, ref footer));
                detail = ReplaceDetail(detail, data.Tables[Convert.ToInt32(data.Tables[data.Tables.Count - 1].Rows[0]["table"])]);
                string result = header.Replace("@$footer", footer);
                result = result.Replace("@$detail", detail);
                return ReplaceHtml(result);
            }
            catch
            {
                MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE);
                return string.Empty;
            }
        }

        public static string ReplaceEnter(string val)
        {
            return val.Replace("</br>", Environment.NewLine)
                .Replace("<br/>", Environment.NewLine)
                .Replace("<br>", Environment.NewLine);
        }

        public static string ReplaceTab(string val)
        {
            return val.Replace("<tab>", "    ")
                .Replace("</tab>", "    ")
                .Replace("<tab/>", "    ");
        }

        public static string ReplaceHtml(string input)
        {
            return ReplacePlatform(input)
                 .Replace("@@deviceHeightRowGrid", FSetting.HeightRowGrid.ToString())
                 .Replace("@@deviceHeightMenu", "50")
                 .Replace("@@deviceHeight", (FSetting.ScreenHeight - 80).ToString())
                 .Replace("@@deviceWidth", (FSetting.ScreenWidth - 16).ToString())
                 .Replace("@@deviceFontSizeContent", FSetting.FontSizeLabelContent.ToString())
                 .Replace("@@deviceFontSizeTitle", FSetting.FontSizeLabelTitle.ToString())
                 .Replace("@@deviceFontSizeButton", FSetting.FontSizeButton.ToString())
                 .Replace("@@deviceVersion", FInterface.IFVersion?.InstalledVersionNumber)
                 .Replace("@@deviceFontIconName", FSetting.FontIconName)
                 .Replace("@@deviceFontIcon", Path.Combine(FInterface.IFEnvironment.BaseUrl, FSetting.FontIconFileName))
                 .Replace("@@deviceFontName", FSetting.FontTextName)
                 .Replace("@@deviceFont", Path.Combine(FInterface.IFEnvironment.BaseUrl, FSetting.FontTextFileName))
                 .Replace("@@deviceColorContent", FSetting.TextColorContent.Hex())
                 .Replace("@@deviceColorTitle", FSetting.TextColorTitle.Hex())
                 .Replace("@@deviceColorPrimary", FSetting.PrimaryColor.Hex())
                 .Replace("@@deviceColorInfo", FSetting.InfoColor.Hex())
                 .Replace("@@deviceColorSecondary", FSetting.SecondaryColor.Hex())
                 .Replace("@@deviceColorSuccess", FSetting.SuccessColor.Hex())
                 .Replace("@@deviceColorWarning", FSetting.WarningColor.Hex())
                 .Replace("@@deviceColorDanger", FSetting.DangerColor.Hex())
                 .Replace("@@deviceColorLight", FSetting.LightColor.Hex())
                 .Replace("@@deviceColorDark", FSetting.DarkColor.Hex())
                 .Replace("@@deviceColorTime", FSetting.ColorTime.Hex())
                 .Replace("@@deviceColorDisable", FSetting.DisableColor.Hex())
                 .Replace("@@deviceColorBackgroundMain", FSetting.BackgroundMain.Hex())
                 .Replace("@@deviceColorLine", FSetting.LineBoxReportColor.Hex())
                 .Replace("@@deviceBackgroundAlternatingRow", FSetting.BackgroundAlternatingRow.Hex())
                 .Replace("@@deviceLineBoxReportColor", FSetting.LineBoxReportColor.Hex())
                 .Replace("@@deviceBackgroundMain", FSetting.BackgroundMain.Hex())
                 .Replace("@@deviceStyleFast", Path.Combine(FInterface.IFEnvironment?.BaseUrl, "Fast.min.css"));
        }

        private static void ReplaceHeaderAndFooter(DataTable table, ref string header, ref string footer)
        {
            header = ReplaceAll(header, table);
            footer = ReplaceAll(footer, table);
        }

        private static string ReplaceAll(string input, DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
                return input;
            var result = input;
            data.Rows.ForEach<DataRow>((x) => data.Columns.ForEach<DataColumn>((z) => result = result.Replace($"@@{z.ColumnName}", x[z.ColumnName].ToString())));
            return result;
        }

        private static string ReplaceDetail(string detail, DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
                return "";
            string result = "";
            data.Rows.ForEach<DataRow>((x) =>
            {
                var temp = detail;
                data.Columns.ForEach<DataColumn>((z) => temp = temp.Replace($"@@{z.ColumnName}", x[z.ColumnName].ToString()));
                result += temp;
            });
            return result;
        }

        private static string ReplacePlatform(string input)
        {
            input = input.Replace("\n", " ");
            string currentPlatform = Device.RuntimePlatform.ToLower();
            string notUsing = FSetting.IsAndroid ? Device.iOS.ToLower() : Device.Android.ToLower();
            foreach (Match match in new Regex($@"<{currentPlatform}>(.*?)</{currentPlatform}>").Matches(input))
                input = input.Replace($"<{currentPlatform}>{match.Groups[1].Value}</{currentPlatform}>", match.Groups[1].Value);
            foreach (Match match in new Regex($@"<{notUsing}>(.*?)</{notUsing}>").Matches(input))
                input = input.Replace($"<{notUsing}>{match.Groups[1].Value}</{notUsing}>", "");
            return input;
        }
    }
}