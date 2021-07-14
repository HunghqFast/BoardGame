using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static Xamarin.Essentials.Permissions;

namespace FastMobile.FXamarin.Core
{
    public static class FUtility
    {
        public static bool IsTimeOut
        {
            get
            {
                if (string.IsNullOrEmpty(FSetting.NetWorkKey))
                    return true;
                return DateTime.Now > FServices.LastAccess.AddMinutes(FString.TimeOut);
            }
        }

        public static bool HasNetwork => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public static bool CurrentIsUrl => FString.ServiceUrl.IsUrl();

        public static DataTable AddRowValue(this DataTable dt, string columnName, object value, Type columnType = null)
        {
            if (!dt.Columns.Contains(columnName))
            {
                if (columnType == null) dt.Columns.Add(columnName);
                else dt.Columns.Add(columnName, columnType);
            }
            DataRow dr;
            if (dt.Rows.Count == 0)
            {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
            }
            else dr = dt.Rows[0];
            dr[columnName] = value;
            return dt;
        }

        public static DataTable AddRowValue(this DataTable dt, int rowIndex, string columnName, object value, Type columnType = null)
        {
            if (!dt.Columns.Contains(columnName))
            {
                if (columnType == null) dt.Columns.Add(columnName);
                else dt.Columns.Add(columnName, columnType);
            }
            if (dt.Rows.Count == rowIndex)
                dt.Rows.Add(dt.NewRow());
            DataRow dr = dt.Rows[rowIndex];
            dr[columnName] = value;
            return dt;
        }

        public static DataSet AddTable(this DataSet ds, DataTable dt)
        {
            ds.Tables.Add(dt);
            return ds;
        }

        public static Task<bool> IsLastVersion()
        {
            if ("FastMobile.FXamarin.Core.IsUsingLastestVersionLastTimeCheck".GetCache().ToDate().AddMinutes(15) > DateTime.Now)
                return Task.FromResult(true);
            DateTime.Now.ToDate().SetCache("FastMobile.FXamarin.Core.IsUsingLastestVersionLastTimeCheck");
            return FInterface.IFVersion?.IsUsingLatestVersion();
        }

        public static DateTime ToDate(this string datetime)
        {
            if (string.IsNullOrEmpty(datetime))
                return DateTime.MinValue;
            DateTime.TryParseExact(datetime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return result;
        }

        public static DateTime ToDate(this string datetime, string format)
        {
            if (string.IsNullOrEmpty(datetime))
                return DateTime.MinValue;
            DateTime.TryParseExact(datetime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            return result;
        }

        public static string ToDate(this DateTime datetime, string format)
        {
            return datetime.ToString(format);
        }

        public static string ToDate(this DateTime datetime) => datetime.ToString("dd/MM/yyyy HH:mm:ss");

        public static bool IsUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            return (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp) || (Uri.TryCreate(url, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsContainUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            var urls = url.Split(";");
            if (urls.Length == 0)
                return false;
            foreach (string u in urls)
                if (u.IsUrl()) return true;
            return false;
        }

        public static bool IsUnicode(this string text)
        {
            return text.Any(c => c > 255);
        }

        public static bool IsEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                string DomainMapper(Match match)
                {
                    return match.Groups[1].Value + new IdnMapping().GetAscii(match.Groups[2].Value);
                }
            }
            catch
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsListEmail(this string emails)
        {
            if (string.IsNullOrWhiteSpace(emails))
                return false;
            foreach (string mail in FFunc.GetArrayString(emails))
                if (!mail.IsEmail()) return false;
            return true;
        }

        public static bool IsPrimaryKey(this string key, string pattern = "'`~!@#$%^&*(),;\"")
        {
            if (string.IsNullOrEmpty(key)) return true;
            if (key.IndexOf(' ') == 0) return false;
            key = key.Replace("“", "\"").Replace("’", "'");
            foreach (char k in key) if (pattern.Contains(k)) return false;
            return !key.IsUnicode();
        }

        public static bool IsTaxCode(this string taxCode)
        {
            if (taxCode.Trim().Length > 10 && taxCode.Trim().Length != 14)
                return false;
            if (taxCode.Trim().Length == 14)
                if (taxCode.Trim().Substring(10, 1) != "-")
                    return false;

            var taxCodeCheck = taxCode.Replace("-", "").Trim();

            if (taxCodeCheck.Trim().Length == 10 || taxCodeCheck.Trim().Length == 13)
            {
                if (!taxCodeCheck.IsNumeric())
                    return false;
                taxCodeCheck = taxCodeCheck.Substring(0, 10);

                var numberCheck = double.Parse(taxCodeCheck.Substring(0, 1)) * 31;
                numberCheck += double.Parse(taxCodeCheck.Substring(1, 1)) * 29;
                numberCheck += double.Parse(taxCodeCheck.Substring(2, 1)) * 23;
                numberCheck += double.Parse(taxCodeCheck.Substring(3, 1)) * 19;
                numberCheck += double.Parse(taxCodeCheck.Substring(4, 1)) * 17;
                numberCheck += double.Parse(taxCodeCheck.Substring(5, 1)) * 13;
                numberCheck += double.Parse(taxCodeCheck.Substring(6, 1)) * 7;
                numberCheck += double.Parse(taxCodeCheck.Substring(7, 1)) * 5;
                numberCheck += double.Parse(taxCodeCheck.Substring(8, 1)) * 3;
                numberCheck = 10 - (numberCheck % 11);

                return (double.Parse(taxCodeCheck.Substring(9, 1)) == numberCheck);
            }
            return false;
        }

        public static bool IsNumeric(this string value)
        {
            return value.All(char.IsDigit);
        }

        public static string CutString(this string text, int max, string plus = "...")
        {
            if (text.Length <= max || max < plus.Length) return text;
            return text.Remove(max - plus.Length, text.Length - max + plus.Length) + plus;
        }

        public static string Remove(this string text, string remove)
        {
            return text.Replace(remove, string.Empty);
        }

        public static Double ToDouble(this string input) => Double.Parse(input.Replace(",", ".").Replace(" ", ""), new CultureInfo("en-US"));

        public static bool TryToDouble(this string input, out Double result) => Double.TryParse(input.Replace(",", ".").Replace(" ", ""), NumberStyles.Number, new CultureInfo("en-US"), out result);

        public static string Hex(this Color color) => $"#{(int)(color.R * 255):X2}{(int)(color.G * 255):X2}{(int)(color.B * 255):X2}";

        public static Color ColorTrans(this Color color, string percent)
        {
            return Color.FromHex(color.Hex().Replace("#", $"#{percent}"));
        }

        public static ImageSource ToImageSource(this string path)
        {
            if (path.IsUrl())
                return new UriImageSource() { CachingEnabled = true, Uri = new Uri(path), CacheValidity = TimeSpan.FromDays(1) };
            return Xamarin.Forms.ImageSource.FromFile(path);
        }

        public static ImageSource ToImageSource(this string path, Color color)
        {
            var text = FIcons.Type.GetStaticFieldValue(path);
            if (text != null)
                return text.ToString().ToFontImageSource(color);
            return path.ToImageSource();
        }

        public static ImageSource ToImageSource(this string path, string color)
        {
            var text = FIcons.Type.GetStaticFieldValue(path);
            if (text != null)
                return text.ToString().ToFontImageSource(Color.FromHex(color));
            return path.ToImageSource();
        }

        public static ImageSource ToImageSource(this string path, Color color, double size)
        {
            var text = FIcons.Type.GetStaticFieldValue(path);
            if (text != null)
                return text.ToString().ToFontImageSource(color, size);
            return path.ToImageSource();
        }

        public static ImageSource ToImageSource(this string path, string color, double size)
        {
            var text = FIcons.Type.GetStaticFieldValue(path);
            if (text != null)
                return text.ToString().ToFontImageSource(Color.FromHex(color), size);
            return path.ToImageSource();
        }

        public static ImageSource ToImageSourceFromBase64(this string base64String)
        {
            return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64String)));
        }

        public static FontImageSource ToFontImageSource(this string glyph, Color color) => new FontImageSource { Glyph = glyph, Color = color, FontFamily = FSetting.FontIcon };

        public static FontImageSource ToFontImageSource(this string glyph, Color color, double size) => new FontImageSource { Glyph = glyph, Color = color, FontFamily = FSetting.FontIcon, Size = size };

        public static FontImageSource ToFontImageSource(this object binding, string path, Color color)
        {
            var result = new FontImageSource
            {
                BindingContext = binding,
                Color = color,
                FontFamily = FSetting.FontIcon
            };
            result.SetBinding(Xamarin.Forms.FontImageSource.GlyphProperty, path);
            return result;
        }

        public static FontImageSource ToFontImageSource(this object binding, string path, Color color, double size)
        {
            var result = new FontImageSource
            {
                BindingContext = binding,
                Color = color,
                Size = size,
                FontFamily = FSetting.FontIcon
            };
            result.SetBinding(FontImageSource.GlyphProperty, path);
            return result;
        }

        //Set
        public static void SetPropValue(this Type type, object source, string propName, object value)
        {
            type?.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.SetValue(source, value);
        }

        public static void SetFieldValue(this Type type, object source, string propName, object value)
        {
            type?.GetField(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.SetValue(source, value);
        }

        public static object SetStaticPropValue(this object source, string propName, object value)
        {
            source?.GetType()?.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.SetValue(source, value);
            return source;
        }

        public static void SetStaticFieldValue(this Type type, string propName, object value)
        {
            type?.GetField(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.SetValue(type, value);
        }

        //Get
        public static object GetPropValue(this Type type, object source, string propName)
        {
            return type?.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
        }

        public static object GetFieldValue(this Type type, object source, string propName)
        {
            return type?.GetField(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
        }

        public static object GetStaticPropValue(this object source, string propName)
        {
            return source?.GetType()?.GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
        }

        public static object GetStaticFieldValue(this Type type, string propName)
        {
            return type?.GetField(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance)?.GetValue(type);
        }

        public static Type TypeByAssemply(this Assembly assembly, string className)
        {
            return assembly?.GetTypes()?.First(x => x.Name == className);
        }

        public static Type TypeByAssemply(this Assembly assembly, string className, string nestedClassName)
        {
            return assembly?.GetTypes()?.First(x => x.Name == className)?.GetNestedType(nestedClassName);
        }

        public static Task OpenBrowser(this string url)
        {
            return Browser.OpenAsync(url, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.External,
                TitleMode = BrowserTitleMode.Default,
                PreferredToolbarColor = FSetting.PrimaryColor,
                PreferredControlColor = FSetting.PrimaryColor,
                Flags = BrowserLaunchFlags.PresentAsPageSheet
            });
        }

        public static async void DownloadCompleted(object sender, FDownloadEventArgs eventArgs)
        {
            try
            {
                if (!eventArgs.FileSaved)
                {
                    MessagingCenter.Send(eventArgs.Message, FChannel.ALERT_BY_MESSAGE);
                    return;
                }

                await Launcher.OpenAsync(new OpenFileRequest { Title = FText.ApplicationTitle, File = new ReadOnlyFile(eventArgs.Path) });
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
        }

        public static string ReplaceData(this DataTable table, string input, int rowIndex = 0) => ReplaceData(table.Rows[rowIndex], input);

        public static string ReplaceData(this DataRow row, string input)
        {
            row.Table.Columns.ForEach<DataColumn>((x) => input = input.Replace($"{{{x.ColumnName}}}", row[x.ColumnName].ToString()));
            return input;
        }

        public static DataSet ToDataSet(this FMessage message) => ToDataSet(message, FSetting.NetWorkKey);

        public static DataSet ToDataSet(this FMessage message, string key) => message.Message.AESDecrypt(key).ToDataSet();

        public static DataTable Last(this DataSet ds) => ds.Tables[ds.Tables.Count - 1];

        public static DataTable First(this DataSet ds) => ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();

        public static bool IsNullOrEmpty(this object owner, params string[] inputs)
        {
            foreach (var t in inputs)
                if (string.IsNullOrEmpty(t)) return true;
            return false;
        }

        public static string ClearString(this object obj)
        {
            return obj.ToString().TrimEnd().ToLower();
        }

        public static string ClearString(this string str)
        {
            return str.TrimEnd().ToLower();
        }

        public static string ToEmbeddedPath(this string fileName, string assemly = "FastMobile.FXamarin.Core.Resources")
        {
            var f = fileName.DeleteEmbeddedCache();
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemly + "." + fileName))
            using (var file = new FileStream(f, FileMode.Create, FileAccess.ReadWrite))
            {
                resource.CopyTo(file);
            }
            return f;
        }

        public static string DeleteEmbeddedCache(this string path)
        {
            var f = Path.Combine(DependencyService.Get<IFEnvironment>()?.PersionalPath, path);
            if (File.Exists(f))
                File.Delete(f);
            return f;
        }

        public static string ReplaceSpamJson(this string json)
        {
            return json?.Replace("&comma;", ",");
        }

        public static DataTable JsonToTable(this string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return null;
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                var tb = new DataTable();
                foreach (var key in obj)
                    tb.AddRowValue(0, key.Key, key.Value);
                return tb;
            }
            catch { return null; }
        }

        public static string GetContentType(string exts)
        {
            return (exts.Replace(".", "").ToLower().Trim()) switch
            {
                "*" or "323" or "acx" or "ai" or "aif" or "aifc" or "aiff" or "asf" or "asr" or "asx" or "au" or "avi" or "axs" or "bas" or "bcpio" or "bin" or "cpio" or "c" or "cat" or "cdf" or "cer" or "class" or "clp" or "crd" or "crl" or "crt" or "csh" or "dcr" or "der" or "dir" or "dll" or "dms" or "dvi" or "dxr" or "eps" or "etx" or "evy" or "exe" or "fif" or "gtar" or "gz" or "h" or "hdf" or "hlp" or "hqx" or "hta" or "htc" or "iii" or "ins" or "isp" or "lha" or "lsf" or "lsx" or "lzh" or "m13" or "m14" or "m3u" or "man" or "mdb" or "me" or "mht" or "mhtml" or "mid" or "mny" or "mov" or "movie" or "mp2" or "mp3" or "mpa" or "mpe" or "mpeg" or "mpg" or "mpp" or "mpv2" or "ms" or "msg" or "mvb" or "nc" or "nws" or "oda" or "p10" or "p12" or "p7b" or "p7c" or "p7m" or "p7r" or "p7s" or "pbm" or "pfx" or "pko" or "pma" or "pmc" or "pml" or "pmr" or "pmw" or "pnm" or "pot" or "pps" or "ppt" or "pptx" or "prf" or "ps" or "pub" or "qt" or "ra" or "ram" or "rmi" or "roff" or "rtf" or "rtx" or "scd" or "sct" or "setpay" or "setreg" or "sh" or "shar" or "sit" or "snd" or "spc" or "spl" or "src" or "sst" or "stl" or "sv4cpio" or "sv4crc" or "swf" or "t" or "tar" or "tcl" or "tex" or "texi" or "texinfo" or "tgz" or "tr" or "trm" or "tsv" or "uls" or "ustar" or "vcf" or "vrml" or "wav" or "wcm" or "wdb" or "wks" or "wmf" or "wps" or "wri" or "z" or "zip" => "FILE",
                "bmp" or "cmx" or "cod" or "gif" or "ico" or "ief" or "jfif" or "jpe" or "jpeg" or "jpg" or "pgm" or "ppm" or "ras" or "rgb" or "svg" or "tif" or "tiff" or "xbm" or "xpm" or "xwd" => "IMG",
                "doc" or "docx" or "dot" or "flr" or "wrl" or "wrz" or "xaf" or "xof" or "wpd" => "WORD",
                "htm" or "html" or "css" or "htt" or "js" or "latex" or "stm" or "xml" => "CODE",
                "pdf" => "PDF",
                "xla" or "xlc" or "xlm" or "xls" or "xlsx" or "xlt" or "xlw" => "EXCEL",
                "txt" or "tex" => "TEXT",
                _ => "FILE",
            };
        }

        public static async Task<byte[]> GetFileData(FileResult file)
        {
            byte[] result = null;
            using (var ms = new MemoryStream())
            {
                using var stream = await file.OpenReadAsync();
                stream.CopyTo(ms);
                result = ms.ToArray();
                ms.Dispose();
            }
            return result;
        }

        public static async Task<string> GetFilePath(FileResult file)
        {
            try
            {
                if (File.Exists(file.FullPath)) return file.FullPath;
                var stream = await file.OpenReadAsync().ConfigureAwait(false);
                var fileResultPath = string.Empty;
                if (stream != null && stream != Stream.Null)
                {
                    var tempFile = Path.Combine(FileSystem.CacheDirectory, NewFileName(Path.GetExtension(file.FileName)));
                    using (var fileStream = new FileStream(tempFile, FileMode.CreateNew))
                    {
                        await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                        await stream.FlushAsync().ConfigureAwait(false);
                    }

                    fileResultPath = tempFile;

                    await stream.DisposeAsync().ConfigureAwait(false);
                }
                return fileResultPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string> GetFileBase64(FileResult file)
        {
            var bytes = await GetFileData(file);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public static byte[] GetFileData(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return null;
            byte[] result = null;
            using (var ms = new MemoryStream())
            {
                using var stream = fileInfo.OpenRead();
                stream.CopyTo(ms);
                result = ms.ToArray();
                ms.Dispose();
            }
            return result;
        }

        public static byte[] GetFileData(Stream stream)
        {
            byte[] result = null;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                result = ms.ToArray();
                ms.Dispose();
            }
            return result;
        }

        public static string GetFileBase64(FileInfo fileInfo)
        {
            var bytes = GetFileData(fileInfo);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public static async Task<Stream> GetFileStream(FileResult file)
        {
            var ms = new MemoryStream();
            using var stream = await file.OpenReadAsync();
            stream.CopyTo(ms);
            return ms;
        }

        public static Stream GetFileStream(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms;
        }

        public static string GetFileSize(double size, int precision)
        {
            if (size < 0) return "";
            if (size == 0) return "0 KB";
            if (size <= 1024) return $"1 KB";

            string[] a = { "B", "KB", "MB", "GB", "TB" };
            var i = 0;
            while (i < 4 && size >= 1024)
            {
                size = (size / 1024).ToFixed(precision);
                i++;
            }
            return $"{size} {a[i]}";
        }

        public static string GetContentTypeByExt(string ext)
        {
            return ext.Replace(".", "").Trim().ToLower() switch
            {
                "*" => "application/octet-stream",
                "323" => "text/h323",
                "acx" => "application/internet-property-stream",
                "ai" => "application/postscript",
                "aif" => "audio/x-aiff",
                "aifc" => "audio/x-aiff",
                "aiff" => "audio/x-aiff",
                "asf" => "video/x-ms-asf",
                "asr" => "video/x-ms-asf",
                "asx" => "video/x-ms-asf",
                "au" => "audio/basic",
                "avi" => "video/x-msvideo",
                "axs" => "application/olescript",
                "bas" => "text/plain",
                "bcpio" => "application/x-bcpio",
                "bin" => "application/octet-stream",
                "bmp" => "image/bmp",
                "c" => "text/plain",
                "cat" => "application/vnd.ms-pkiseccat",
                "cdf" => "application/x-cdf",
                "cer" => "application/x-x509-ca-cert",
                "class" => "application/octet-stream",
                "clp" => "application/x-msclip",
                "cmx" => "image/x-cmx",
                "cod" => "image/cis-cod",
                "cpio" => "application/x-cpio",
                "crd" => "application/x-mscardfile",
                "crl" => "application/pkix-crl",
                "crt" => "application/x-x509-ca-cert",
                "csh" => "application/x-csh",
                "css" => "text/css",
                "dcr" => "application/x-director",
                "der" => "application/x-x509-ca-cert",
                "dir" => "application/x-director",
                "dll" => "application/x-msdownload",
                "dms" => "application/octet-stream",
                "doc" => "application/msword",
                "docx" => "application/msword",
                "dot" => "application/msword",
                "dvi" => "application/x-dvi",
                "dxr" => "application/x-director",
                "eps" => "application/postscript",
                "etx" => "text/x-setext",
                "evy" => "application/envoy",
                "exe" => "application/octet-stream",
                "fif" => "application/fractals",
                "flr" => "x-world/x-vrml",
                "gif" => "image/gif",
                "gtar" => "application/x-gtar",
                "gz" => "application/x-gzip",
                "h" => "text/plain",
                "hdf" => "application/x-hdf",
                "hlp" => "application/winhlp",
                "hqx" => "application/mac-binhex40",
                "hta" => "application/hta",
                "htc" => "text/x-component",
                "htm" => "text/html",
                "html" => "text/html",
                "htt" => "text/webviewhtml",
                "ico" => "image/x-icon",
                "ief" => "image/ief",
                "iii" => "application/x-iphone",
                "ins" => "application/x-internet-signup",
                "isp" => "application/x-internet-signup",
                "jfif" => "image/pipeg",
                "jpe" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "jpg" => "image/jpeg",
                "js" => "application/x-javascript",
                "latex" => "application/x-latex",
                "lha" => "application/octet-stream",
                "lsf" => "video/x-la-asf",
                "lsx" => "video/x-la-asf",
                "lzh" => "application/octet-stream",
                "m13" => "application/x-msmediaview",
                "m14" => "application/x-msmediaview",
                "m3u" => "audio/x-mpegurl",
                "man" => "application/x-troff-man",
                "mdb" => "application/x-msaccess",
                "me" => "application/x-troff-me",
                "mht" => "message/rfc822",
                "mhtml" => "message/rfc822",
                "mid" => "audio/mid",
                "mny" => "application/x-msmoney",
                "mov" => "video/quicktime",
                "movie" => "video/x-sgi-movie",
                "mp2" => "video/mpeg",
                "mp3" => "audio/mpeg",
                "mp4" => "video/mp4",
                "mpa" => "video/mpeg",
                "mpe" => "video/mpeg",
                "mpeg" => "video/mpeg",
                "mpg" => "video/mpeg",
                "mpp" => "application/vnd.ms-project",
                "mpv2" => "video/mpeg",
                "ms" => "application/x-troff-ms",
                "msg" => "application/vnd.ms-outlook",
                "mvb" => "application/x-msmediaview",
                "nc" => "application/x-netcdf",
                "nws" => "message/rfc822",
                "oda" => "application/oda",
                "p10" => "application/pkcs10",
                "p12" => "application/x-pkcs12",
                "p7b" => "application/x-pkcs7-certificates",
                "p7c" => "application/x-pkcs7-mime",
                "p7m" => "application/x-pkcs7-mime",
                "p7r" => "application/x-pkcs7-certreqresp",
                "p7s" => "application/x-pkcs7-signature",
                "pbm" => "image/x-portable-bitmap",
                "pdf" => "application/pdf",
                "pfx" => "application/x-pkcs12",
                "pgm" => "image/x-portable-graymap",
                "pko" => "application/ynd.ms-pkipko",
                "pma" => "application/x-perfmon",
                "pmc" => "application/x-perfmon",
                "pml" => "application/x-perfmon",
                "pmr" => "application/x-perfmon",
                "pmw" => "application/x-perfmon",
                "pnm" => "image/x-portable-anymap",
                "pot" => "application/vnd.ms-powerpoint",
                "ppm" => "image/x-portable-pixmap",
                "pps" => "application/vnd.ms-powerpoint",
                "ppt" => "application/vnd.ms-powerpoint",
                "pptx" => "application/vnd.ms-powerpoint",
                "prf" => "application/pics-rules",
                "ps" => "application/postscript",
                "pub" => "application/x-mspublisher",
                "qt" => "video/quicktime",
                "ra" => "audio/x-pn-realaudio",
                "ram" => "audio/x-pn-realaudio",
                "ras" => "image/x-cmu-raster",
                "rgb" => "image/x-rgb",
                "rmi" => "audio/mid",
                "roff" => "application/x-troff",
                "rtf" => "application/rtf",
                "rtx" => "text/richtext",
                "scd" => "application/x-msschedule",
                "sct" => "text/scriptlet",
                "setpay" => "application/set-payment-initiation",
                "setreg" => "application/set-registration-initiation",
                "sh" => "application/x-sh",
                "shar" => "application/x-shar",
                "sit" => "application/x-stuffit",
                "snd" => "audio/basic",
                "spc" => "application/x-pkcs7-certificates",
                "spl" => "application/futuresplash",
                "src" => "application/x-wais-source",
                "sst" => "application/vnd.ms-pkicertstore",
                "stl" => "application/vnd.ms-pkistl",
                "stm" => "text/html",
                "sv4cpio" => "application/x-sv4cpio",
                "sv4crc" => "application/x-sv4crc",
                "svg" => "image/svg+xml",
                "swf" => "application/x-shockwave-flash",
                "t" => "application/x-troff",
                "tar" => "application/x-tar",
                "tcl" => "application/x-tcl",
                "tex" => "application/x-tex",
                "texi" => "application/x-texinfo",
                "texinfo" => "application/x-texinfo",
                "tgz" => "application/x-compressed",
                "tif" => "image/tiff",
                "tiff" => "image/tiff",
                "tr" => "application/x-troff",
                "trm" => "application/x-msterminal",
                "tsv" => "text/tab-separated-values",
                "txt" => "text/plain",
                "uls" => "text/iuls",
                "ustar" => "application/x-ustar",
                "vcf" => "text/x-vcard",
                "vrml" => "x-world/x-vrml",
                "wav" => "audio/x-wav",
                "wcm" => "application/vnd.ms-works",
                "wdb" => "application/vnd.ms-works",
                "wks" => "application/vnd.ms-works",
                "wmf" => "application/x-msmetafile",
                "wps" => "application/vnd.ms-works",
                "wri" => "application/x-mswrite",
                "wrl" => "x-world/x-vrml",
                "wrz" => "x-world/x-vrml",
                "xaf" => "x-world/x-vrml",
                "xbm" => "image/x-xbitmap",
                "xla" => "application/vnd.ms-excel",
                "xlc" => "application/vnd.ms-excel",
                "xlm" => "application/vnd.ms-excel",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.ms-excel",
                "xlt" => "application/vnd.ms-excel",
                "xlw" => "application/vnd.ms-excel",
                "xof" => "x-world/x-vrml",
                "xpm" => "image/x-xpixmap",
                "xwd" => "image/x-xwindowdump",
                "z" => "application/x-compress",
                "zip" => "application/zip",
                _ => "application/octet-stream",
            };
        }

        public static double ToFixed(this double number, int decimals)
        {
            return Convert.ToDouble(number.ToString("N" + decimals));
        }

        public static string TextFinger(FAuthenticationType type, bool v)
        {
            return type switch
            {
                FAuthenticationType.Face => FText.AttributeString(v ? "V" : "E", "BioFace"),
                _ => FText.AttributeString(v ? "V" : "E", "BioFingerprint"),
            };
        }

        public static string TextFingerType(FAuthenticationType type, bool v)
        {
            return type switch
            {
                FAuthenticationType.Face => FText.AttributeString(v ? "V" : "E", "BioTypeFace"),
                _ => FText.AttributeString(v ? "V" : "E", "BioTypeFingerprint"),
            };
        }

        public static CancellationToken TimeoutToken(CancellationToken cancellationToken, TimeSpan timeout)
        {
            var cancelTokenSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout > TimeSpan.Zero)
                cancelTokenSrc.CancelAfter(timeout);
            return cancelTokenSrc.Token;
        }

        public static string ToString(this Position position) => $"{position.Latitude},{position.Longitude}";

        public static Position ToPosition(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;
            var arr = value.Split(",");
            if (arr.Length < 2)
                return default;
            return new Position(Convert.ToDouble(arr[0].Trim()), Convert.ToDouble(arr[1].Trim()));
        }

        public static async void RunAfter(this Action action, TimeSpan span)
        {
            await Task.Delay(span);
            action?.Invoke();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }

        public static string GetParamFromUrl(string url, string paramName)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            return HttpUtility.ParseQueryString(new Uri(url).Query).Get(paramName);
        }

        public static string ToQuery(this string input)
        {
            return input?.Replace("+", "%2B");
        }

        public static string NewFileName(string extension)
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff") + extension;
        }

        public static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }

        #region For

        public static void ForEach<T>(this IEnumerable items, Action<T> action)
        {
            foreach (T item in items)
                action?.Invoke(item);
        }

        public static void ForIndex<T>(this IEnumerable items, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in items)
            {
                action?.Invoke(item, index);
                index++;
            }
        }

        //--

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action?.Invoke(item);
        }

        public static void ForIndex<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in items)
            {
                action?.Invoke(item, index);
                index++;
            }
        }

        //--

        public static async Task ForEach<T>(this IEnumerable<T> items, Func<T, Task> invoke)
        {
            foreach (T item in items) await invoke?.Invoke(item);
        }

        public static async Task ForIndex<T>(this IEnumerable<T> items, Func<T, int, Task> invoke)
        {
            int index = 0;
            foreach (T item in items)
            {
                await invoke?.Invoke(item, index);
                index++;
            }
        }

        //--

        public static async void ForEachAsync<T>(this IEnumerable items, Action<T> invoke)
        {
            var task = new List<Task>();
            foreach (T item in items) task.Add(Task.Run(() => invoke?.Invoke(item)));
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        public static async void ForIndexAsync<T>(this IEnumerable items, Action<T, int> invoke)
        {
            int index = 0;
            var task = new List<Task>();
            foreach (T item in items)
            {
                task.Add(Task.Run(() => invoke?.Invoke(item, index)));
                index++;
            }
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        //--

        public static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> invoke)
        {
            foreach (T item in items) await invoke?.Invoke(item);
        }

        public static async Task ForIndexAsync<T>(this IEnumerable<T> items, Func<T, int, Task> invoke)
        {
            int index = 0;
            foreach (T item in items)
            {
                await invoke?.Invoke(item, index);
                index++;
            }
        }

        //--

        public static async void ForAllAsync<T>(this IEnumerable<T> items, Action<T> invoke)
        {
            var task = new List<Task>();
            foreach (T item in items) task.Add(Task.Run(() => invoke?.Invoke(item)));
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        public static async void ForAllAsync<T>(this IEnumerable<T> items, Action<T, int> invoke)
        {
            int index = 0;
            var task = new List<Task>();
            foreach (T item in items)
            {
                task.Add(Task.Run(() => invoke?.Invoke(item, index)));
                index++;
            }
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        //--

        public static async Task ForAllAsync<T>(this IEnumerable<T> items, Func<T, Task> invoke)
        {
            var task = new List<Task>();
            foreach (T item in items) task.Add(invoke?.Invoke(item));
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        public static async Task ForAllAsync<T>(this IEnumerable<T> items, Func<T, int, Task> invoke)
        {
            int index = 0;
            var task = new List<Task>();
            foreach (T item in items)
            {
                task.Add(invoke?.Invoke(item, index));
                index++;
            }
            await Task.WhenAll(task).ConfigureAwait(true);
        }

        //--

        public static async void RunBackground(params Task[] action)
        {
            await Task.WhenAll(action).ConfigureAwait(false);
        }

        #endregion For

        #region Invoke

        public static async Task<string> InvokeAsync(string servicePath, string namesapceUrl, string methodName, int timeOut, IEnumerable<FParam> paramValues, CancellationTokenSource cancellation)
        {
            using (HttpClient httpClient = new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }) { Timeout = TimeSpan.FromMilliseconds(timeOut) })
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var request = new HttpRequestMessage(HttpMethod.Post, servicePath);
                request.Content = new StringContent(CreateSOAPRequestXML(namesapceUrl, methodName, paramValues), Encoding.UTF8, "text/xml");
                request.Headers.Clear();
                request.Headers.Add("SOAPAction", $"{namesapceUrl}{methodName}");

                var response = await httpClient.SendAsync(request, cancellation.Token);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"), true))
                    {
                        return GetXMLResult(methodName, reader.ReadToEnd());
                    }
                }
            }
        }

        public static async Task<string> InvokeAsyncAttachment(string servicePath, int timeOut, string authen, CancellationTokenSource cancellation, IEnumerable<FFileInfo> files)
        {
            if (files == null || files.Count() == 0)
                throw new Exception("File can not null");

            using (HttpClient httpClient = new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }) { Timeout = TimeSpan.FromMilliseconds(timeOut) })
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("FM", authen);
                var request = new HttpRequestMessage(HttpMethod.Post, servicePath);
                request.Headers.ExpectContinue = false;

                using (var filesContent = new MultipartFormDataContent())
                {
                    if (files != null)
                    {
                        files.ForEach(x =>
                        {
                            var f = new ByteArrayContent(x.Data);
                            f.Headers.ContentLength = x.Data.Length;
                            f.Headers.Add("Content-Type", GetContentTypeByExt(Path.GetExtension(x.Info == null ? x.FileName : x.Info.Name)));
                            filesContent.Add(f, x.LineNbrMode, x.Info == null ? x.FileName : x.Info.Name);
                        });
                    }

                    request.Content = filesContent;
                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.ReasonPhrase);

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"), true))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }
        }

        public static async Task<string> GetResult(HttpClient httpClient, HttpRequestMessage message, MultipartFormDataContent filesData, CancellationTokenSource token, string url, string method)
        {
            var response = filesData == null ? await httpClient.SendAsync(message, token.Token) : await httpClient.PostAsync(url, filesData, token.Token);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"), true))
                {
                    return GetXMLResult(method, reader.ReadToEnd());
                }
            }
        }

        public static string Invoke(string servicePath, string namesapceUrl, string methodName, int timeOut, IEnumerable<FParam> paramValues)
        {
            using (HttpClient httpClient = new())
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeOut);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

                var request = new HttpRequestMessage(HttpMethod.Post, servicePath);
                request.Content = new StringContent(CreateSOAPRequestXML(namesapceUrl, methodName, paramValues), Encoding.UTF8, "text/xml");
                request.Headers.Clear();
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                request.Headers.Add("SOAPAction", $"{namesapceUrl}{methodName}");
                var send = httpClient.SendAsync(request);
                send.Wait();
                var response = send.Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);

                using var stream = response.Content.ReadAsStreamAsync();
                stream.Wait();
                return GetXMLResult(methodName, new StreamReader(stream.Result, Encoding.GetEncoding("utf-8"), true).ReadToEnd());
            }
        }

        public static string Encode(this DataSet ds)
        {
            var memoryStream = new MemoryStream();
            var zipStream = new GZipStream(memoryStream, CompressionMode.Compress);
            ds.WriteXml(zipStream, XmlWriteMode.WriteSchema);
            zipStream.Close();
            var data = memoryStream.ToArray();
            memoryStream.Close();
            return Convert.ToBase64String(data);
        }

        public static DataSet ToDataSet(this string data)
        {
            if (string.IsNullOrEmpty(data)) return new DataSet();
            try
            {
                var memoryStream = new MemoryStream(Convert.FromBase64String(data));
                var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                var ds = new DataSet();
                ds.ReadXml(zipStream, XmlReadMode.ReadSchema);
                zipStream.Close();
                memoryStream.Close();
                return ds;
            }
            catch
            {
                return new DataSet();
            }
        }

        public static string GetRandomString()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        #endregion Invoke

        #region Json

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(this string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

        #endregion Json

        #region Device

        public static void SetCache(this object value, string key)
        {
            Preferences.Set(key, value?.ToString());
        }

        public static void SetCache(this object value, object key)
        {
            if (key?.ToString() is not string k)
                return;
            Preferences.Set(k, value?.ToString());
        }

        public static string GetCache(this object key, string defaultValue = "")
        {
            if (key?.ToString() is not string k)
                return string.Empty;
            return Preferences.Get(k, defaultValue);
        }

        public static void RemoveCache(this string key)
        {
            Preferences.Remove(key);
        }

        public static void RemoveCache(this object key)
        {
            if (key?.ToString() is not string k)
                return;
            Preferences.Remove(k);
        }

        public static bool ContainsCache(this object key)
        {
            return Preferences.ContainsKey(key?.ToString());
        }

        public static void ClearCache()
        {
            Preferences.Clear();
        }

        #endregion Device

        #region Private methods

        private static string CreateSOAPRequestXML(string namesapceUrl, string methodName, IEnumerable<FParam> paramValues)
        {
            var s = "";
            paramValues?.ForEach((x) => s += $"<{x.Name}>{HttpUtility.HtmlEncode(x.ParamValue)}</{x.Name}>");
            return @$"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" ><soap:Body><{methodName} xmlns=""{namesapceUrl}"">{s}</{methodName}></soap:Body></soap:Envelope>";
        }

        private static string GetXMLResult(string methodName, string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(s);
                var node = xml.DocumentElement.GetElementsByTagName($"{methodName.Trim()}Result");
                return node.Count == 0 ? "" : node[0].InnerText;
            }
            catch
            {
                return "";
            }
        }

        #endregion Private methods
    }
}