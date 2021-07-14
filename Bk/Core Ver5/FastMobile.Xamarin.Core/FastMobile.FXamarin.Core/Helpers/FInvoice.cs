using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInvoice
    {
        private const string split = "ü";

        public static async Task<bool> Release(FData data, FData filter, List<FData> list, FData bioData)
        {
            var controller = data["controller"].ToString();
            var method = data["method"].ToString();
            var action = data["action"].ToString();
            var processType = data["xu_ly"].ToString();
            var extFields = data["externalFields"].ToString();
            var voucherBook = filter["ma_hd"].ToString().Trim();
            var unit = filter["ma_dvcs"].ToString();
            var limitInv = Convert.ToInt32(data["limit_invoice"]);
            var extObj = new FExtensionParam { WriteLog = true };
            extObj.H = bioData == null || !bioData.CheckName(FReportToolbar.BioRequestField) ? string.Empty : bioData[FReportToolbar.BioRequestField].ToString();
            object idNumber, listDays, voucherDate, customerList;

            if (processType.Equals("2") || processType.Equals("3"))
            {
                FData dataRow = list[0];
                idNumber = processType.Equals("2") ? dataRow["stt_rec"] : dataRow["stt_rec1"];
                listDays = dataRow["chuoi_ngay"];
                voucherDate = processType.Equals("2") ? FFunc.ConvertDateTime(dataRow["ngay_ct"]) : FFunc.ConvertDateTime(dataRow["ngay_ct1"]);
                customerList = dataRow["ma_kh"].ToString().Trim();
            }
            else
            {
                if (limitInv < list.Count)
                {
                    await Task.Delay(150);
                    MessagingCenter.Send(new FMessage(0, 148, limitInv.ToString()), FChannel.ALERT_BY_MESSAGE);
                    return false;
                }

                idNumber = string.Join(",", list.Select(x => x["stt_rec"]).ToArray());
                listDays = string.Join(",", list.Select(x => x["chuoi_ngay"]).ToArray());
                voucherDate = string.Join(",", list.Select(x => FFunc.ConvertDateTime(x["ngay_ct"])).ToArray());
                customerList = string.Join(",", list.Select(x => x["ma_kh"]).ToArray());
            }

            if (FFunc.StringToBoolean(data["useToken"].ToString()))
            {
                await Task.Delay(150);
                MessagingCenter.Send(new FMessage(0, 159, unit), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            var dt = new Dictionary<string, object>
            {
                { "dFrom", FFunc.ConvertDateTime(filter["ngay_ct1"]) },
                { "dTo", FFunc.ConvertDateTime(filter["ngay_ct2"]) },
                { "unit", unit },
                { "voucherCode", data["voucherCode"] },
                { "idNumber", idNumber },
                { "listDays", listDays },
                { "voucherDate", voucherDate },
                { "customerList", customerList },
                { "voucherBook", filter["ma_hd"].ToString().Trim() },
                { "debugMode", "0" },
                { "external", extFields == "#" ? "" : extFields }
            };
            return processType.Equals("3") ? await Request(controller, action, method, dt, ShowError39, list.Count, extObj) : await Request(controller, action, method, dt, ShowError12, list.Count, extObj);
        }

        public static async Task<bool> Cancel(FData currentRow, FData data, FData filter, FData confirmForm)
        {
            var controller = data["controller"].ToString();
            var type = currentRow["huy_yn"].ToString() == "0";
            var method = data["method"].ToString();
            var action = data["action"].ToString().Trim();
            var external = type ? confirmForm["so_bien_ban"].ToString() + split + FFunc.ConvertDateTime(confirmForm["ngay_bien_ban"]) + split + confirmForm["ly_do"].ToString() : "";
            var dt = new Dictionary<string, object>
            {
                { "unit", filter["ma_dvcs"] },
                { "voucherCode", data["voucherCode"] },
                { "idNumber", currentRow["stt_rec"] },
                { "voucherDate", FFunc.ConvertDateTime(currentRow["ngay_ct"]) },
                { "listDays", currentRow["chuoi_ngay"] },
                { "external", external }
            };
            return await Request(controller, action, method, dt, ShowError39, 1, new FExtensionParam { WriteLog = true });
        }

        #region private

        private static async Task<bool> Request(string controller, string action, string method, Dictionary<string, object> data, Action<FInvoiceRelease, int> error, int length, FExtensionParam extension)
        {
            var ch = await FServices.ReleaseInvoice(controller, action, method, data, extension);
            if (string.IsNullOrEmpty(ch))
                return false;
            try
            {
                var ms = ch.AESDecrypt(FCrypto.Key).ToObject<FMessage>();
                if (ms.Success == 1)
                {
                    MessagingCenter.Send(new FMessage(1, 901, ""), FChannel.ALERT_BY_MESSAGE);
                    return true;
                }
                if (!ms.OK && (string.IsNullOrWhiteSpace(ms.Message) || !(ms.Message.Contains("Reference") && ms.Message.Contains("Code") && ms.Message.Contains("Field"))))
                    MessagingCenter.Send(ms, FChannel.ALERT_BY_MESSAGE);
                else error?.Invoke(ms.Message.ToObject<FInvoiceRelease>(), length);
                return false;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
        }

        private static void ShowError12(FInvoiceRelease r, int length)
        {
            var f = string.IsNullOrEmpty(r.Reference);
            FMessage m;

            if (r.Code != "-1")
            {
                m = new FMessage
                {
                    Success = 0,
                    Code = f ? r.Code.Trim() switch
                    {
                        "1" => 151,
                        "2" => 149,
                        "3" => 150,
                        "4" => 153,
                        _ => 408,
                    } : 157,
                    Message = f ? r.Field : string.Format(r.Message, r.Reference)
                };
            }
            else
            {
                m = new FMessage
                {
                    Success = 0,
                    Code = 158,
                    Message = f ? $"{r.Number}{(char)254}{length}{(char)254} " : $"{r.Number}{(char)254}{length}{(char)254}{string.Format(r.Message, r.Reference)}"
                };
            }
            MessagingCenter.Send(m, FChannel.ALERT_BY_MESSAGE);
        }

        private static void ShowError39(FInvoiceRelease r, int length)
        {
            var f = string.IsNullOrEmpty(r.Reference);
            var m = new FMessage
            {
                Success = 0,
                Code = f ? r.Code.Trim() switch
                {
                    "0" => 150,
                    "1" => 151,
                    "2" => 152,
                    "3" => 153,
                    "4" => 154,
                    "5" => 155,
                    "6" => 156,
                    _ => 408,
                } : 157,
                Message = f ? r.Field : string.Format(r.Message, r.Reference)
            };
            MessagingCenter.Send(m, FChannel.ALERT_BY_MESSAGE);
        }

        #endregion private
    }
}