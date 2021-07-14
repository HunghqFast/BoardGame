using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public class FInvoke
    {
        public async Task<FMessage> InvokeMethod(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
                return FMessage.FromSuccess();
            if (!ds.Last().Columns.Contains("dv_invoke"))
                return FMessage.FromSuccess();

            if (ds.Last().Rows.Count == 0)
                return new FMessage(1, 0, "");
            string invokeString = ds.Last().Rows[0]["dv_invoke"].ToString();

            if (string.IsNullOrEmpty(invokeString))
                return new FMessage(1, 0, "");

            string[] arrayInvoke = invokeString.Split($"&{(char)255};", StringSplitOptions.None);

            foreach (string method in arrayInvoke)
            {
                int k = method.IndexOf("(");
                var res = await InvokeMethod(method.Substring(0, k), method.Substring(k + 1, method.Length - k - 2).Split(","));
                if (res == null || res.Success != 1)
                    return res ?? new FMessage();
            }
            return FMessage.FromSuccess();
        }

        public Task<FMessage> InvokeMethod(string method, object[] args)
        {
            try
            {
                return method switch
                {
                    "SendPrivate" => SendPrivate(args),
                    "SendMail" => SendMail(args),
                    _ => Task.FromResult(FMessage.FromSuccess())
                };
            }
            catch (Exception ex)
            {
                return Task.FromResult(new FMessage(ex.Message));
            }
        }

        public async Task<FMessage> SendPrivate(params object[] @params)
        {
            var message = await OnApprove(@params[0].ToString().Trim(), @params[1].ToString().Trim(), @params[2].ToString().Trim(), @params[3].ToString().Trim(), @params[4].ToString().Trim(), @params[5].ToString().Trim(), @params[6].ToString().Trim(), @params.Length > 8 ? @params[8].ToString() : string.Empty, @params[7].ToString().Trim());
            return message == null ? new FMessage() : message.Success == "1" ? new FMessage(1, 0, "") : new FMessage(0, string.IsNullOrEmpty(message.Message) ? 902 : 0, string.IsNullOrEmpty(message.Message) ? "" : message.Message);
        }

        public async Task<FInvokeResult> OnApprove(string typeApprove, string url, string namespaces, string method, string idMail, string ref_code, string priority, string comment, string external)
        {
            var @params = new List<FParam>
            {
                new FParam("idMail", idMail),
                new FParam("type", typeApprove),
                new FParam("query", new FQuery { IDNumber = ref_code, Priority = Convert.ToInt32(priority), Comment = comment }.ToJson()),
                new FParam("dataID", FString.ServiceDatabase),
                new FParam("external", external)
            };

            var message = await FServices.ExecuteCommand(FServices.ServiceUrl + url, namespaces, method, @params, true);
            return string.IsNullOrEmpty(message) ? null : message.ToObject<FInvokeResult>();
        }

        public async Task<FMessage> SendMail(params object[] @params)
        {
            var message = await OnMail(@params[0].ToString().Trim(), @params[1].ToString().Trim(), @params[2].ToString().Trim(), @params[3].ToString().Trim(), @params[4].ToString().Trim(), @params[5].ToString().Trim(), @params[6].ToString().Trim(), @params[7].ToString().Trim());
            return string.IsNullOrEmpty(message.Message) || message.Success == "1" ? FMessage.FromSuccess() : FMessage.FromFail(Convert.ToInt32(message.Code), message.Message);
        }

        public async Task<FInvokeResult> OnMail(string url, string namespaces, string method, string idMessage, string idNumber, string priority, string contactID, string type)
        {
            var @params = new List<FParam>
            {
                new FParam("idMessage", idMessage),
                new FParam("idNumber", idNumber),
                new FParam("dataID", FString.ServiceDatabase),
                new FParam("priority", priority),
                new FParam("contactID", contactID),
                new FParam("type", type)
            };

            var message = await FServices.ExecuteCommand(FServices.ServiceUrl + url, namespaces, method, @params, true);
            return string.IsNullOrEmpty(message) ? null : message.ToObject<FInvokeResult>();
        }
    }
}