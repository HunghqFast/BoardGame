using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FFunc
    {
        public static string ConvertDateTime(object obj, string format = "yyyyMMdd")
        {
            return DateTime.Parse(obj.ToString()).ToString(format);
        }

        public static string ReplaceHtmlText(string html)
        {
            return FHtml.ReplaceHtml(html);
        }

        public static bool IsBinding(string value)
        {
            return value.Contains("[") && value.Contains("]");
        }

        public static string ReplaceBinding(string value)
        {
            return value.Remove("[").Remove("]");
        }

        public static int Compare(string x, string y, FieldType type)
        {
            return type switch
            {
                FieldType.DateTime => DateTime.Parse(x).CompareTo(DateTime.Parse(y)),
                FieldType.Bool => bool.Parse(x).CompareTo(bool.Parse(y)),
                FieldType.Number => Int32.Parse(x).CompareTo(Int32.Parse(y)),
                _ => x.CompareTo(y),
            };
        }

        public static object Compute(object sender, string expression, DataSet data = null, bool isNumber = false)
        {
            expression = ReplaceExpression(sender, expression, isNumber);
            return FJSInvoke.Invoke(sender, expression, data);
        }

        public static async Task<object> ComputeAsync(object sender, string expression, DataSet data = null, bool isNumber = false)
        {
            expression = ReplaceExpression(sender, expression, isNumber);
            return await FJSInvoke.InvokeAsync(sender, expression, data);
        }

        public static void CatchMessage(string msg)
        {
            if (FSetting.IsAndroid) msg = msg.Replace("</br>", Environment.NewLine).Replace(">", "&gt;").Replace("<", "&lt;");
            if (msg.Equals("$NotAuthorized")) MessagingCenter.Send(new FMessage(0, 900, string.Empty), FChannel.ALERT_BY_MESSAGE);
            else if (msg.Equals("$Error")) return;
            else if (msg.Contains(" ") || !int.TryParse(msg, out _)) MessagingCenter.Send(new FMessage(0, 0, msg), FChannel.ALERT_BY_MESSAGE);
            else MessagingCenter.Send(new FMessage(0, int.Parse(msg), string.Empty), FChannel.ALERT_BY_MESSAGE);
        }

        public static void CatchScriptMethod(object sender, DataSet data, string tag = "script")
        {
            if (data == null || data.Tables.Count == 0 || !data.Tables[0].Columns.Contains(tag) || data.Tables[0].Rows.Count == 0) return;
            var script = data.Tables[0].Rows[0][tag].ToString();
            if (string.IsNullOrEmpty(script)) return;
            var scripts = GetArrayString(script, ';');
            scripts.ForEach(x => Compute(sender, x, data));
        }

        public static void CatchScriptMethod(object sender, string script, char separate = ';')
        {
            if (string.IsNullOrWhiteSpace(script)) return;
            var scripts = GetArrayString(script, separate);
            scripts.ForEach(x => Compute(sender, x, null));
        }

        public static void AddTypeAction(ref DataSet ds, string type, string target, string action = null, string successAction = null, bool isCopy = false, string commandArgument = null, string controllerParent = null)
        {
            var dt = new DataTable();
            dt.AddRowValue("type", type).AddRowValue(0, "target", target);
            if (!string.IsNullOrEmpty(action)) dt.AddRowValue(0, "action", action);
            if (!string.IsNullOrEmpty(successAction)) dt.AddRowValue(0, "success", successAction);
            if (isCopy) dt.AddRowValue(0, "copy", true, typeof(bool));
            if (!string.IsNullOrEmpty(commandArgument)) dt.AddRowValue(0, "commandArgument", commandArgument);
            if (!string.IsNullOrEmpty(controllerParent)) dt.AddRowValue(0, "controllerParent", controllerParent);
            dt.AddRowValue(0, "platform", DeviceInfo.Platform.ToString());
            ds.AddTable(dt);
        }

        public static void AddColumnTable(DataTable dt, List<FField> fields)
        {
            fields.ForEach((x) =>
            {
                if (x.ItemStyle != FItemStyle.Password && x.Push)
                {
                    switch (x.FieldType)
                    {
                        case FieldType.Bool:
                            dt.Columns.Add(x.Name, typeof(bool));
                            break;

                        case FieldType.NumberString:
                        case FieldType.Number:
                            dt.Columns.Add(x.Name, typeof(decimal));
                            break;

                        case FieldType.Table:
                        case FieldType.DateTime:
                        default:
                            dt.Columns.Add(x.Name, typeof(string));
                            break;
                    }
                }
            });
        }

        public static List<DataTable> AddFieldInput(ref DataSet ds, DataTable dt, List<FField> fields, FData data, bool isRequest = true, List<string> rFields = null)
        {
            AddColumnTable(dt, fields);
            var ext = AddRowTable(dt, data, fields, isRequest, 0, rFields);
            if (data.CheckName(FReportToolbar.BioRequestField))
            {
                dt.Columns.Add(FReportToolbar.BioRequestField, typeof(string));
                dt.AddRowValue(0, FReportToolbar.BioRequestField, data[FReportToolbar.BioRequestField]);
            }
            ds.Tables.Add(dt.Copy());
            dt.Dispose();
            return ext;
        }

        public static List<DataTable> AddRowTable(DataTable dt, FData data, List<FField> fields, bool isRequest, int index, List<string> rFields = null)
        {
            var ext = new List<DataTable>();
            fields.ForEach((field) =>
            {
                if (field.ItemStyle != FItemStyle.Password && field.Push && (isRequest || field.AllowRequest))
                {
                    var value = rFields == null || rFields.Count == 0 ? data[field.Name] : data[rFields[fields.IndexOf(field)]];
                    switch (field.FieldType)
                    {
                        case FieldType.Bool:
                            value = StringToBoolean(value.ToString());
                            break;

                        case FieldType.DateTime:
                            value = FInputDate.GetRequestValue(value);
                            break;

                        case FieldType.NumberString:
                            if (string.IsNullOrWhiteSpace(value.ToString())) value = 0;
                            break;

                        case FieldType.Table:
                            var grid = value as FInputGridValue;
                            if (isRequest && grid.Table != null) ext.Add(grid.Table);
                            value = isRequest ? grid.Edited : "0";
                            break;

                        case FieldType.Number:
                        default:
                            if (field.ItemStyle == FItemStyle.Lookup || field.ItemStyle == FItemStyle.AutoComplete)
                            {
                                value = value?.ToString().Split(FInput.Seperate)[0].TrimEnd();
                                break;
                            }
                            value = value?.ToString()?.TrimEnd();
                            break;
                    }
                    dt.AddRowValue(index, field.Name, value);
                }
            });
            return ext;
        }

        public static void SwapObject(object a, object b, string propertyName)
        {
            var temp = GetValueBinding(b, propertyName);
            SetValueBinding(b, propertyName, GetValueBinding(a, propertyName));
            SetValueBinding(a, propertyName, temp);
        }

        public static object GetValueBinding(object e, string propertyName)
        {
            return e.GetType().GetProperty(propertyName)?.GetValue(e, null);
        }

        public static void SetValueBinding(object e, string propertyName, object value)
        {
            e.GetType().GetProperty(propertyName)?.SetValue(e, value);
        }

        public static string GetStringValue(JObject obj, string key)
        {
            return (string)obj.SelectToken(key) ?? string.Empty;
        }

        public static double GetNumberValue(JObject obj, string key, double init = 0)
        {
            return double.TryParse(GetStringValue(obj, key), out double value) ? value : init;
        }

        public static bool GetBooleanValue(JObject obj, string key, bool init = false)
        {
            return StringToBoolean(GetStringValue(obj, key), init);
        }

        public static List<string> GetArrayString(string value, char split = ',', bool clearSpace = true)
        {
            if (string.IsNullOrEmpty(value)) return new List<string>();
            var l = value.Split(split).OfType<string>();
            if (clearSpace) l = l.Select(x => x.Trim());
            return l.ToList();
        }

        public static void CreateEventArgs(object obj, EventHandler eventHandler, EventArgs e = null)
        {
            eventHandler?.Invoke(obj, e);
        }

        public static bool StringToBoolean(string value, bool init = false)
        {
            if (string.IsNullOrEmpty(value)) return init;
            return value == "1" || (bool.TryParse(value, out bool result) && result);
        }

        private static string ReplaceExpression(object sender, string expression, bool isNumber = false)
        {
            expression = " " + expression;
            if (isNumber || !Regex.IsMatch(expression, @"['](.?)+[']"))
            {
                expression = Regex.Replace(expression, @"\d+\,(\d+?E\+)?\d", m => m.ToString().Replace(',', '.'));
                expression = Regex.Replace(expression, @"[\s-?\(]\d+(\.\d+)?", m => m.ToString().Contains(".") ? m.ToString() : $"{m}.0");
            }
            expression = expression.Replace("@@language", FSetting.Language);
            expression = expression.Replace("@@admin", FString.Admin.ToString().ToLower());
            expression = expression.Replace("@@platform", $"'{DeviceInfo.Platform}'");
            if (sender is FPageFilter form) expression = expression.Replace("@@action", form.FormType.ToString());
            return expression;
        }
    }

    public class FFunc<T>
    {
        public static List<T> GetFListObject(JToken objects) => FViewPage.CheckNull(objects) ? objects.Select(o => (T)Activator.CreateInstance(typeof(T), o)).ToList() : new List<T>();
    }
}