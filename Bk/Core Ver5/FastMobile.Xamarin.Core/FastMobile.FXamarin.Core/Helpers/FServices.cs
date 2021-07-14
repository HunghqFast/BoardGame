using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FServices
    {
        private const string NamespaceUrl = "http://fast.com.vn/";
        private const int TimeOut = 60000, ServerErrorCode = 407, TimeOutCode = 404;
        public static bool TicketUpdating { get; private set; }

        public static string AppMode { get; private set; }
        public static string ServiceUrl { get; private set; }
        public static string Version { get; private set; }

        public static DateTime LastAccess
        {
            get => "FastMobile.FXamarin.Core.FService.LastAccess".GetCache().ToDate();
            internal set => value.ToDate().SetCache("FastMobile.FXamarin.Core.FService.LastAccess");
        }

        public static DateTime TicketExpireTime
        {
            get => "FastMobile.FXamarin.Core.FService.TicketExpireTime".GetCache().ToDate();
            internal set => value.ToDate().SetCache("FastMobile.FXamarin.Core.FService.TicketExpireTime");
        }

        static FServices()
        {
            ServiceUrl = FString.ServiceUrl;
        }

        public static void Init(string version, string mode)
        {
            Version = version;
            AppMode = mode;
        }

        internal static void SetUrl(string url)
        {
            ServiceUrl = url;
        }

        public static async Task<FMessage> CheckVersion()
        {
            var mess = await GetVersion();
            var outVersion = System.Version.Parse(Version).CompareTo(System.Version.Parse(mess.Message));
            if (outVersion != 0)
                return FMessage.FromFail(outVersion > 0 ? 204 : 205);
            return FMessage.FromSuccess();
        }

        public static async Task<FMessage> GetVersion()
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            return await GetMessage(new List<FParam>(), "GetVersion", FCrypto.Key);
        }

        public static async Task<FMessage> Capcha()
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            return await GetMessage(new List<FParam> { new FParam("deviceID", FSetting.DeviceID.AESEncrypt(FCrypto.Key)) }, "Captcha", FCrypto.Key);
        }

        public static async Task<FMessage> CaptchaByKey(string keyword)
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            var par = new List<FParam>();
            par.Add(new FParam("deviceID", FSetting.DeviceID.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("keyword", keyword.AESEncrypt(FCrypto.Key)));
            return await GetMessage(par, "CaptchaByKey", FCrypto.Key);
        }

        public static async Task<FMessage> ValidateCaptchaByKey(string keyword, string value, string del)
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            var par = new List<FParam>();

            par.Add(new FParam("deviceID", FSetting.DeviceID.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("keyword", keyword.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("value", value.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("del", del.AESEncrypt(FCrypto.Key)));
            return await GetMessage(par, "ValidateCaptchaByKey", FCrypto.Key);
        }

        public static async Task<FMessage> Recovery(string capcha, string email, string name = "")
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            var par = new List<FParam>
            {
                new FParam("deviceID", FSetting.DeviceID.AESEncrypt(FCrypto.Key)),
                new FParam("name", name.AESEncrypt(FCrypto.Key)),
                new FParam("email", email.AESEncrypt(FCrypto.Key)),
                new FParam("appMode", AppMode.AESEncrypt(FCrypto.Key)),
                new FParam("language", FSetting.Language.AESEncrypt(FCrypto.Key)),
                new FParam("captcha", capcha.AESEncrypt(FCrypto.Key))
            };
            return await GetMessage(par, "PasswordRecovery", FCrypto.Key);
        }

        public static async Task<FMessage> GetKeys(string Username)
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            var par = new List<FParam>();
            var random = FUtility.GetRandomString().Substring(0, 8);
            par.Add(new FParam("salt", random.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("deviceId", FSetting.DeviceID.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("data", Username.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("version", Version.AESEncrypt(FCrypto.Key)));
            return await GetMessage(par, "GetKeys", FCrypto.Key, random + (char)254);
        }

        public static Task<FMessage> CheckHash(string username, string password, string salt, string token = "", string type = "1")
        {
            if (!FUtility.CurrentIsUrl)
                return Task.FromResult(FMessage.FromFail(ServerErrorCode, ""));
            var par = new List<FParam>();
            par.Add(new FParam("user", username.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("hash", (password.MD5() + salt).MD5().AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("deviceInfo", new FDeviceInformation(false, token).ToJson().AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("type", type.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("appMode", AppMode.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("dataName", FString.ServiceDatabase.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("extension", FExtensionParam.New(true, FText.AttributeString("V", "SignIn"), FText.AttributeString("E", "SignIn"), FAction.Login).ToJson().AESEncrypt(FCrypto.Key)));
            return GetMessage(par, "CheckHash", FCrypto.Key);
        }

        public static async Task<FMessage> SaveKey(string key, string password, FAction fAction)
        {
            if (!FUtility.HasNetwork)
                return FMessage.FromFail(403, "");

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            if (await UpdateTicket() is FMessage ticketMessage && ticketMessage.Success != 1)
                return ticketMessage;

            var pars = FExtensionParam.New(true, FText.AttributeString("V", "SignIn"), FText.AttributeString("E", "SignIn"), fAction);
            pars.AppMode = AppMode;
            pars.DataName = FString.ServiceDatabase;
            pars.UserID = FString.UserID;
            pars.Key = key.AESEncrypt(FSetting.NetWorkKey);
            pars.Password = password.AESEncrypt(FSetting.NetWorkKey);
            return await ExecuteExtension("200", pars);
        }

        public static async Task<FMessage> GetKeyExt()
        {
            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");
            return await ExecuteExtension("300", new FExtensionParam { AppMode = AppMode, DataName = FString.ServiceDatabase, UserID = FString.UserID, Username = FString.Username });
        }

        public static async Task<(FMessage Message, string Token)> CheckHashExt(bool isUpdate, string key, string sender)
        {
            if (!FUtility.CurrentIsUrl)
                return (FMessage.FromFail(ServerErrorCode, ""), string.Empty);

            isUpdate = isUpdate && !string.IsNullOrWhiteSpace(sender);
            var pars = FExtensionParam.New(true, FText.AttributeString("V", "SignInBio"), FText.AttributeString("E", "SignInBio"), FAction.LoginBiometric);
            pars.AppMode = AppMode;
            pars.DataName = FString.ServiceDatabase;
            pars.UserID = FString.UserID;
            pars.Username = FString.Username;
            pars.Password = FString.Password.AESEncrypt(key);
            pars.IsUpdate = isUpdate;

            if (isUpdate) pars.DeviceInfo = new FDeviceInformation(false) { NotifyToken = await FInterface.IFFirebase.GetTokenAsync(sender, 30) };
            return (await ExecuteExtension("400", pars), pars.DeviceInfo?.NotifyToken);
        }

        public static async Task<FMessage> ValidHash(string hash)
        {
            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            if (await UpdateTicket() is FMessage ticketMessage && !ticketMessage.OK)
                return ticketMessage;

            var par = new List<FParam>();
            var pars = FExtensionParam.New(true, FText.AttributeString("V", "SignInBio"), FText.AttributeString("E", "SignInBio"), FAction.CheckPasswordBiometric);
            pars.AppMode = AppMode;
            pars.DataName = FString.ServiceDatabase;
            pars.UserID = FString.UserID;
            pars.Password = hash.AESEncrypt(FSetting.NetWorkKey);
            return await ExecuteExtension("500", pars);
        }

        public static async Task<(bool OK, string Secret)> ConfirmPassword(string text, bool alert)
        {
            var (OK, Message) = await new FAlertEntry().ShowTextbox(false, true, text);
            if (!OK || string.IsNullOrEmpty(Message)) return (false, "");
            var hashMessage = await FServices.ValidHash((Message.MD5() + FSetting.DeviceID).MD5());
            if (hashMessage.OK) return (true, FCrypto.AESDecrypt(hashMessage.Message, FSetting.NetWorkKey));
            if (alert) MessagingCenter.Send(hashMessage, FChannel.ALERT_BY_MESSAGE);
            return (false, string.Empty);
        }

        public static async Task<FMessage> ExecuteExtension(string method, FExtensionParam param)
        {
            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode, "");

            var par = new List<FParam>();
            par.Add(new FParam("deviceID", FSetting.DeviceID.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("method", method.AESEncrypt(FCrypto.Key)));
            par.Add(new FParam("params", param.ToJson().AESEncrypt(FCrypto.Key)));
            return await GetMessage(par, "ExecuteExtension", FCrypto.Key);
        }

        public static Task<FMessage> ExecuteCommand(string action, string controller, DataSet ds, string method, FExtensionParam extension, string keyword, bool isAlert = true, CancellationTokenSource cancellation = null, string keys = "")
        {
            return GetResult(ds, FSetting.Language, action, controller, method, TimeOut, keyword, isAlert, extension, cancellation, keys);
        }

        public static Task<FMessage> ExecuteCommand(string action, string controller, DataSet ds, string method, FExtensionParam extension, int timeOut, bool isAlert = true, CancellationTokenSource cancellation = null, string keys = "")
        {
            return GetResult(ds, FSetting.Language, action, controller, method, timeOut, "", isAlert, extension, cancellation, keys);
        }

        public static Task<FMessage> ExecuteCommand(string action, string controller, DataSet ds, string method, FExtensionParam extension, bool isAlert = true, CancellationTokenSource cancellation = null, string keys = "")
        {
            return GetResult(ds, FSetting.Language, action, controller, method, TimeOut, "", isAlert, extension, cancellation, keys);
        }

        public static async Task<string> ExecuteCommand(string uri, string nameSpace, string methodName, IEnumerable<FParam> fParams, bool isAlert = true, CancellationTokenSource cancellation = null)
        {
            if (CheckError("", "", FSetting.Language, "", isAlert) is FMessage error)
                return new FInvokeResult { Success = "0", Message = FAlertHelper.MessageByCode(error.Code.ToString()) }.ToJson();

            if (await IsUpdating())
                return FMessage.FromFail(ServerErrorCode).ToJson().AESEncrypt(FCrypto.Key);

            if (await UpdateTicket() is FMessage ticketMessage && ticketMessage.Success != 1)
                return new FInvokeResult { Success = "0", Message = FAlertHelper.MessageByCode(ticketMessage.Code.ToString()) }.ToJson();

            var result = "";
            var @params = new List<FParam>();
            @params.Add(new FParam("id", FString.UserID.AESEncrypt(FCrypto.Key)));
            @params.Add(new FParam("device", FSetting.DeviceID.AESEncrypt(FCrypto.Key)));
            @params.Add(new FParam("language", FSetting.Language.AESEncrypt(FCrypto.Key)));
            fParams.ForEach((x) => @params.Add(new FParam(x.Name, x.ParamValue.AESEncrypt(FSetting.NetWorkKey))));

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidationURI);
                result = await FUtility.InvokeAsync(uri, nameSpace, methodName, TimeOut, @params, cancellation ?? new CancellationTokenSource());
            }
            catch (Exception ex)
            {
                if (isAlert) MessagingCenter.Send(FMessage.FromFail(ServerErrorCode, ex.Message), FChannel.ALERT_BY_MESSAGE);
            }

            if (string.IsNullOrEmpty(result))
            {
                if (isAlert) MessagingCenter.Send(new FMessage("FService.ExecuteCommand"), FChannel.ALERT_BY_MESSAGE);
                return "";
            }
            LastAccess = DateTime.Now;
            return result;
        }

        public static async Task<string> ReleaseInvoice(string controller, string action, string method, Dictionary<string, object> data, FExtensionParam extension)
        {
            try
            {
                if (CheckError("", "", FSetting.Language, "", false) is FMessage error)
                    return error.ToJson().AESEncrypt(FCrypto.Key);

                if (await IsUpdating())
                    return FMessage.FromFail(ServerErrorCode).ToJson().AESEncrypt(FCrypto.Key);

                if (await UpdateTicket() is FMessage ticketMessage && ticketMessage.Success != 1)
                    return ticketMessage.ToJson().AESEncrypt(FCrypto.Key);

                if (!string.IsNullOrEmpty(extension.H))
                    extension.H = FCrypto.AESEncrypt(extension.H, FSetting.NetWorkKey);

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidationURI);
                LastAccess = DateTime.Now;
                return await FUtility.InvokeAsync(FString.ServiceUrl + FCoreString.MOBILE_URL_RELEASE, NamespaceUrl, "ExecuteCommand", TimeOut, ReleaseInvoiceDefaultParams(controller, action, method, data, FSetting.NetWorkKey, FCrypto.Key, extension), new CancellationTokenSource());
            }
            catch (Exception ex)
            {
                return FMessage.FromFail(ServerErrorCode, ex.Message).ToJson().AESEncrypt(FCrypto.Key);
            }
        }

        public static Task<bool> EInvoiceHandler()
        {
            return Task.FromResult(true);
        }

        public static string DownloadUrl(string controller, string controller_ref, string fileName, string sysKey, string lineNbr, string ticket, int isMedia = 0)
        {
            return $"{FString.ServiceUrl}{FCoreString.MOBILE_URL_DOWNLOADEXTENDER}?ui={HttpUtility.UrlEncode(FString.UserID.AESEncrypt(FCrypto.Key)).ToQuery()}&di={HttpUtility.UrlEncode(FSetting.DeviceID.AESEncrypt(FCrypto.Key)).ToQuery()}&co={controller}&cr={HttpUtility.UrlEncode(controller_ref.AESEncrypt(FSetting.NetWorkKey)).ToQuery()}&la={FSetting.Language}&ap={AppMode}&dn={FString.ServiceDatabase}&ve={Version}&fi={fileName.ToQuery()}&sy={sysKey.ToQuery()}&li={lineNbr}&ti={ticket}&im={isMedia}&da={HttpUtility.UrlEncode(sysKey.AESEncrypt(FSetting.NetWorkKey)).ToQuery()}";
        }

        public static async Task<FMessage> Attachment(string controller, string sysKey, string ticket, string type, List<FFileInfo> files)
        {
            try
            {
                if (CheckError("", "", FSetting.Language, "", false) is FMessage error)
                    return error;

                if (await IsUpdating())
                    return FMessage.FromFail(ServerErrorCode);

                if (await UpdateTicket() is FMessage ticketMessage && ticketMessage.Success != 1)
                    return ticketMessage;

                var message = await FUtility.InvokeAsyncAttachment(FString.ServiceUrl + FCoreString.MOBILE_URL_UPLOADEXTENDER, TimeOut, new Dictionary<string, string>
                {
                    { "userID", FString.UserID },
                    { "deviceID", FSetting.DeviceID },
                    { "controller", controller },
                    { "appMode", AppMode },
                    { "dataName", FString.ServiceDatabase },
                    { "version", Version },
                    { "syskey", sysKey },
                    { "language", FSetting.Language },
                    { "type", type },
                    { "data", sysKey.AESEncrypt(FSetting.NetWorkKey) },
                    { "ticket", ticket }
                }.ToJson().AESEncrypt(FCrypto.Key), new CancellationTokenSource(), files);
                if (string.IsNullOrEmpty(message))
                    return new FMessage();
                return message.ToObject<FMessage>();
            }
            catch (Exception ex)
            {
                return FMessage.FromFail(100, ex.Message);
            }
        }

        public static async Task<FMessage> UpdateTicket(string keys = "", bool alert = false, bool checkTime = true)
        {
            try
            {
                TicketUpdating = true;
                if (checkTime && TicketExpireTime.AddMinutes(FOptions.TicketMinute) > DateTime.Now)
                {
                    TicketUpdating = false;
                    return FMessage.FromSuccess();
                }

                if (!FUtility.HasNetwork)
                {
                    TicketUpdating = false;
                    var result = FMessage.FromFail(403);
                    if (alert) MessagingCenter.Send(result, FChannel.ALERT_BY_MESSAGE);
                    return result;
                }

                if (!FUtility.CurrentIsUrl)
                {
                    TicketUpdating = false;
                    var result = FMessage.FromFail(ServerErrorCode);
                    if (alert) MessagingCenter.Send(result, FChannel.ALERT_BY_MESSAGE);
                    return result;
                }

                TicketExpireTime = DateTime.Now;
                var message = await ExecuteExtension("100", new FExtensionParam { AppMode = AppMode, DataName = FString.ServiceDatabase, UserID = FString.UserID });
                TicketUpdating = false;

                if (message.Success == 0 && message.Code == 700)
                    return FMessage.FromSuccess();

                if (message.Success == 0 && message.Code == 205)
                {
                    TicketExpireTime = DateTime.MinValue;
                    if (await FInterface.IFVersion?.IsUsingLatestVersion())
                    {
                        var result = FMessage.FromFail(900);
                        if (alert) MessagingCenter.Send(result, FChannel.ALERT_BY_MESSAGE);
                        return result;
                    }

                    if (alert) MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return message;
                }

                if (message.Success == 0)
                {
                    TicketExpireTime = DateTime.MinValue;
                    if (alert) MessagingCenter.Send(message, FChannel.ALERT_BY_MESSAGE);
                    return message;
                }

                var ds = message.ToDataSet(string.IsNullOrEmpty(keys) ? FSetting.NetWorkKey : keys);
                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || !ds.Tables[0].Columns.Contains("network_key") || !ds.Tables[0].Columns.Contains("expired"))
                {
                    var result = new FMessage();
                    if (alert && FSetting.IsDebug) MessagingCenter.Send(result, FChannel.ALERT_BY_MESSAGE);
                    TicketExpireTime = DateTime.MinValue;
                    return result;
                }

                FSetting.NetWorkKey = ds.Tables[0].Rows[0]["network_key"].ToString();
                FOptions.TicketMinute = Convert.ToInt32(ds.Tables[0].Rows[0]["expired"]);
                return FMessage.FromSuccess();
            }
            catch (Exception ex)
            {
                TicketUpdating = false;
                var result = new FMessage(ex.Message);
                if (alert && FSetting.IsDebug) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                TicketExpireTime = DateTime.MinValue;
                return result;
            }
        }

        public static async Task<Dictionary<string, object>> GetOptions(List<string> strings)
        {
            try
            {
                var para = new FExtensionParam { AppMode = AppMode, DataName = FString.ServiceDatabase, UserID = FString.UserID, OptionsName = strings };
                var deviceID = FSetting.DeviceID.AESEncrypt(FCrypto.Key);
                var method = "GetOptions".AESEncrypt(FCrypto.Key);
                var param = para.ToJson().AESEncrypt(FCrypto.Key);

                var message = await GetMessage(new List<FParam> { new FParam("deviceID", deviceID), new FParam("method", method), new FParam("params", param) }, "ExecuteExtension", FCrypto.Key);

                if (message.Success == 0)
                    return new Dictionary<string, object>();

                return message.Message.ToObject<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug)
                    MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
                return new Dictionary<string, object>();
            }
        }

        public static async Task ForAllAsync<T>(IEnumerable<T> items, Func<T, Task> invoke, Func<Task> afterInvoke)
        {
            await UpdateTicket();
            await items.ForAllAsync(invoke);
            if (afterInvoke != null)
                await afterInvoke.Invoke();
        }

        public static async Task ForAllAsync<T>(IEnumerable<T> items, Func<T, Task> invoke)
        {
            await UpdateTicket();
            await items.ForAllAsync(invoke);
        }

        public static async Task ForAllAsync<T>(Func<T, Task> invoke, params T[] items)
        {
            await UpdateTicket();
            await items.ForAllAsync(invoke);
        }

        private static List<FParam> InitDefaultParam(string action, string controller, string method, object requestParam, string language, string network_key, string keys, FExtensionParam extension)
        {
            return DefaultFParam(action, controller, method, requestParam == null ? "" : requestParam is DataSet dataSet ? dataSet.Encode() : requestParam.ToJson(), language, network_key, keys, extension);
        }

        public static List<FParam> DefaultFParam(string action, string controller, string method, string jsonParam, string language, string network_key, string keys, FExtensionParam extension)
        {
            return new List<FParam>
            {
                new FParam("device", FSetting.DeviceID.AESEncrypt(keys)),
                new FParam("id", FString.UserID.AESEncrypt(keys)),
                new FParam("controller", controller.AESEncrypt(keys)),
                new FParam("action", action.AESEncrypt(keys)),
                new FParam("method", method.AESEncrypt(keys)),
                new FParam("data", jsonParam.AESEncrypt(network_key)),
                new FParam("language", language.AESEncrypt(keys)),
                new FParam("checksum", jsonParam.MD5()),
                new FParam("appMode", AppMode.AESEncrypt(keys)),
                new FParam("dataName", FString.ServiceDatabase.AESEncrypt(keys)),
                new FParam("version", Version.AESEncrypt(keys)),
                new FParam("extension", (extension == null ? string.Empty : extension?.ToJson()).AESEncrypt(keys))
            };
        }

        private static FMessage CheckError(string controller, string action, string language, string keyword, bool isAlert)
        {
            if (FUtility.IsTimeOut)
                return FMessage.FromFail(TimeOutCode, "FService.CheckError");

            if (!FUtility.HasNetwork)
            {
                if (string.IsNullOrEmpty(keyword))
                    return FMessage.FromFail(403, "FService.CheckError");
                var t = GetDataSaved(action, controller, language, keyword);
                if (string.IsNullOrEmpty(t))
                    return FMessage.FromFail(403, "FService.CheckError");
                if (isAlert) MessagingCenter.Send(FMessage.FromFail(403, "FService.CheckError"), FChannel.ALERT_BY_MESSAGE);
                return FMessage.FromSuccess(403, t);
            }

            if (!FUtility.CurrentIsUrl)
                return FMessage.FromFail(ServerErrorCode);

            return null;
        }

        private static async Task<FMessage> GetResult(DataSet ds, string language, string action, string controller, string method, int timeout, string keyword, bool isAlert, FExtensionParam extension, CancellationTokenSource cancellation = null, string keys = "")
        {
            if (CheckError(controller, action, language, keyword, isAlert) is FMessage errorMessage)
                return errorMessage;

            if (await IsUpdating())
                return FMessage.FromFail(ServerErrorCode);

            if (await UpdateTicket(keys) is FMessage ticketMessage && ticketMessage.Success != 1)
                return ticketMessage;

            var result = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidationURI);
                result = await FUtility.InvokeAsync(FString.ServiceUrl + FCoreString.MOBILE_URL_DEFAULT, NamespaceUrl, "ExecuteCommand", timeout, InitDefaultParam(action, controller, method, ds, language, FSetting.NetWorkKey, FCrypto.Key, extension), cancellation ?? new CancellationTokenSource());
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(keyword))
                    return FMessage.FromFail(ServerErrorCode, ex.Message);
                var t = GetDataSaved(action, controller, language, keyword);
                if (string.IsNullOrEmpty(t))
                    return FMessage.FromFail(ServerErrorCode, ex.Message);
                if (isAlert) MessagingCenter.Send(FMessage.FromFail(ServerErrorCode, ex.Message), FChannel.ALERT_BY_MESSAGE);
                return FMessage.FromSuccess(ServerErrorCode, t);
            }
            try
            {
                if (!string.IsNullOrEmpty(result))
                {
                    var message = result.AESDecrypt(FCrypto.Key).ToObject<FMessage>();
                    LastAccess = DateTime.Now;
                    if (message.Success == 1 && !string.IsNullOrEmpty(keyword))
                        SetData(action, controller, language, keyword, message.Message);
                    return message;
                }
            }
            catch (Exception ex)
            {
                if (isAlert) MessagingCenter.Send(new FMessage(ex.Message), FChannel.ALERT_BY_MESSAGE);
            }
            return FMessage.FromFail(100, "FService.GetResult");
        }

        private static async Task<FMessage> GetMessage(IEnumerable<FParam> @params, string methodName, string keys, string appendMessage = "", int timeout = 60000)
        {
            string result = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidationURI);
                result = await FUtility.InvokeAsync(FString.ServiceUrl + FCoreString.MOBILE_URL_DEFAULT, NamespaceUrl, methodName, timeout, @params, new CancellationTokenSource());
            }
            catch (Exception ex)
            {
                return FMessage.FromFail(ServerErrorCode, ex.Message);
            }
            try
            {
                if (!string.IsNullOrEmpty(result))
                {
                    var message = result.AESDecrypt(keys).ToObject<FMessage>();
                    LastAccess = DateTime.Now;
                    if (message.Success == 1)
                        message.Message = appendMessage + message.Message;
                    return message;
                }
                return FMessage.FromFail(ServerErrorCode, "FService.GetMessage");
            }
            catch (Exception ex)
            {
                return FMessage.FromFail(100, ex.Message);
            }
        }

        private static bool ValidationURI(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private static void SetData(string action, string controller, string language, string keyword, string value)
        {
            value.SetCache($"FastMobile.FXamarin.Core.FServices.DataOf: {action}{controller}{language}{keyword}");
        }

        private static string GetDataSaved(string action, string controller, string language, string keyword)
        {
            return $"FastMobile.FXamarin.Core.FServices.DataOf: {action}{controller}{language}{keyword}".GetCache();
        }

        private static List<FParam> ReleaseInvoiceDefaultParams(string controller, string action, string method, Dictionary<string, object> data, string netWorkKey, string cryptKey, FExtensionParam extension)
        {
            return FServices.DefaultFParam(action, controller, method, data.ToJson(), FSetting.Language, netWorkKey, cryptKey, extension);
        }

        private static async Task<bool> IsUpdating()
        {
            if (!TicketUpdating)
                return false;

            var watch = new Stopwatch();
            watch.Start();
            while (TicketUpdating)
            {
                await Task.Delay(20);
                if (watch.ElapsedMilliseconds > 30000)
                {
                    TicketUpdating = false;
                    return true;
                }
            }
            watch.Stop();
            return false;
        }
    }
}