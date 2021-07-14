using NCalc;
using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public static class FJSInvoke
    {
        public static readonly Type ExpressionType = typeof(Expression);
        public static readonly string[] Functions = { "invoke", "invokeFunc", "getObject", "deviceID", "MD5", "CRC32", "Base64", "serializeJson", "AESEncrypt", "UrlEncode", "replace", "length", "trim", "subString", "in", "setTitle", "setTitleDetail", "setVisible", "setVisibleDetail", "setVisibleTabOfInputs", "setReadOnly", "setReadOnlyDetail", "setAllowNulls", "setAllowNullsDetail", "setItems", "split", "array", "addDate", "getDate", "getMonth", "getYear", "goTo", "request", "setFormatNumber", "setFormatNumberDetail", "swap", "swapDetail", "getValueByLanguage", "getReference", "setReference", "clearReference", "showMessage", "warningSaved", "confirmMessage", "goBack", "return", "If", "Iff", "wait", "print", "onSelectMode", "invokeScripts", "setMenuVisible", "isTaxCode", "isPrimaryKey", "isEmail", "isListEmail", "isUrl", "doNothing", "showLine", "now", "toDateTime", "dateTimeToStringFormat", "toDatetimeFormat", "checkDetail", "insertDetail", "selectInsertDetail", "clearDetail", "refresh", "reload", "focus", "unfocus", "setScriptDetail", "addScriptInput", "removeScriptInput", "executeCommand", "nextRecord", "backRecord", "getParentPage", "checkConflictDetail", "toBool", "showBio", "showBioPassword", "IfAsync", "IffAsync", "toBoolAsync", "ExcuteFormCommand", "last", "IsContainUrl", "matchPattern", "getPattern" };

        public static async Task<object> Eval(this FunctionArgs args, object sender, EvaluateFunctionHandler parent, DataSet data, int index, bool nor, int timeOut = -1)
        {
            return await args.Parameters[index].Eval(sender, parent, data, nor, timeOut);
        }

        public static async Task<object> Eval(this Expression expression, object sender, EvaluateFunctionHandler parent, DataSet data, bool nor, int timeOut = -1)
        {
            expression.EvaluateFunction -= parent;
            if (nor)
            {
                try
                {
                    expression.EvaluateFunction += async (name, args) => await OnEvaluate(name, args, sender, expression.ToString(), data, null, null, true);
                    return expression.Evaluate();
                }
                catch (Exception ex)
                {
                    if (FSetting.IsDebug) MessagingCenter.Send(new FMessage($"Script: {expression.ParsedExpression}. Error: {ex.Message}"), FChannel.ALERT_BY_MESSAGE);
                    return null;
                }
            }

            if (!expression.ParsedExpression.IsExpression<Function>()) return expression.Evaluate();
            if (expression.ParsedExpression is BinaryExpression binary && binary.IsBinaryMutiple()) return expression.Evaluate();

            var isUnaryBool = expression.ParsedExpression is UnaryExpression unary && unary.Type == UnaryExpressionType.Not;

            var tcs = new TaskCompletionSource<object>();
            if (timeOut != -1)
            {
                var token = FUtility.TimeoutToken(CancellationToken.None, TimeSpan.FromSeconds(timeOut));
                token.Register(Cancel);
            }

            expression.EvaluateFunction += EvalEvent;
            FUtility.RunAfter(Run, TimeSpan.FromMilliseconds(1));
            var result = await tcs.Task;
            return result;

            async void EvalEvent(string name, FunctionArgs args)
            {
                expression.EvaluateFunction -= EvalEvent;
                await OnEvaluate(name, args, sender, "", data, Completed, EvalEvent, nor);
            }

            void Run()
            {
                try
                {
                    expression.Evaluate();
                }
                catch (Exception ex)
                {
                    if (FSetting.IsDebug) MessagingCenter.Send(new FMessage($"Script: {expression.ParsedExpression}. Error: {ex.Message}"), FChannel.ALERT_BY_MESSAGE);
                    Completed(null, false);
                }
            }

            void Cancel()
            {
                if (isUnaryBool) tcs.TrySetResult(false);
                else tcs.TrySetResult(null);
            }

            void Completed(object result, bool fromResult, [CallermemberName] string name = "")
            {
                if (fromResult)
                {
                    if (!isUnaryBool)
                    {
                        tcs.TrySetResult(result);
                        return;
                    }
                    tcs.TrySetResult(!(bool)result);
                }
            }
        }

        public static int Count(this FunctionArgs args)
        {
            return args.Parameters.Length;
        }

        public static object Invoke(object sender, string expression, DataSet data)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;
            var e = new Expression(expression);

            e.EvaluateFunction += async (name, args) => await OnEvaluate(name, args, sender, expression, data, null, null, true);

            try
            {
                return e.Evaluate();
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage($"Script: {expression}. Error: {ex.Message}"), FChannel.ALERT_BY_MESSAGE);
                return null;
            }
        }

        public static async Task<object> InvokeAsync(object sender, string expression, DataSet data, int timeOut = -1)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;
            var e = new Expression(expression);

            if (!IsFunc(expression))
                return e.Evaluate();

            var tcs = new TaskCompletionSource<object>();
            if (timeOut != -1)
            {
                var token = FUtility.TimeoutToken(CancellationToken.None, TimeSpan.FromSeconds(timeOut));
                token.Register(Cancel);
            }

            e.EvaluateFunction += Event;
            FUtility.RunAfter(Run, TimeSpan.FromMilliseconds(1));
            var result = await tcs.Task;
            return result;

            async void Event(string name, FunctionArgs args)
            {
                e.EvaluateFunction -= Event;
                await OnEvaluate(name, args, sender, expression, data, Completed, Event, false);
            }

            void Run()
            {
                try
                {
                    e.Evaluate();
                }
                catch (Exception ex)
                {
                    if (FSetting.IsDebug) MessagingCenter.Send(new FMessage($"Script: {expression}. Error: {ex.Message}"), FChannel.ALERT_BY_MESSAGE);
                    Completed(null, false);
                }
            }

            void Cancel()
            {
                tcs.TrySetResult(null);
            }

            void Completed(object result, bool fromResult, [CallermemberName] string name = "")
            {
                if (fromResult)
                {
                    var isUnaryBool = e.ParsedExpression is UnaryExpression unary && unary.Type == UnaryExpressionType.Not;
                    tcs.TrySetResult(isUnaryBool ? !(bool)result : result);
                }
            }
        }

        private static async Task OnEvaluate(string name, FunctionArgs args, object sender, string expression, DataSet data, Action<object, bool, string> afterInvoke, EvaluateFunctionHandler parEvent, bool nor)
        {
            try
            {
                args.Result = null;
                switch (name)
                {
                    case "invoke":
                        if (nor) args.Result = Invoke(await args.Eval(sender, parEvent, data, 0, nor), (string)await args.Eval(sender, parEvent, data, 1, nor), data);
                        else args.Result = await InvokeAsync(await args.Eval(sender, parEvent, data, 0, nor), (string)await args.Eval(sender, parEvent, data, 1, nor), data);
                        break;

                    case "invokeFunc":
                        FFunc.CatchScriptMethod(sender, (string)await args.Eval(sender, parEvent, data, 0, nor), args.Count() > 1 ? ';' : (await args.Eval(sender, parEvent, data, 1, nor)).ToString()[0]);
                        break;

                    case "getObject":
                        args.Result = FFunc.GetValueBinding(sender, (string)await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "deviceID":
                        args.Result = FSetting.DeviceID;
                        break;

                    case "MD5":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.MD5();
                        break;

                    case "CRC32":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.Crc32();
                        break;

                    case "Base64":
                        args.Result = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes((await args.Eval(sender, parEvent, data, 0, nor)).ToString()));
                        break;

                    case "serializeJson":
                        args.Result = await SerializeJson(sender, args, parEvent, data, nor);
                        break;

                    case "AESEncrypt":
                        args.Result = ((string)await args.Eval(sender, parEvent, data, 0, nor)).AESEncrypt(FSetting.NetWorkKey);
                        break;

                    case "UrlEncode":
                        args.Result = HttpUtility.UrlEncode((string)await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "replace":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor)).ToString().Replace((string)await args.Eval(sender, parEvent, data, 1, nor), (string)await args.Eval(sender, parEvent, data, 2, nor));
                        break;

                    case "length":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor)).ToString().Length;
                        break;

                    case "trim":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor)).ToString().Trim();
                        break;

                    case "subString":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor)).ToString().Substring(Convert.ToInt32(await args.Eval(sender, parEvent, data, 1, nor)), Convert.ToInt32(await args.Eval(sender, parEvent, data, 2, nor)));
                        break;

                    case "in":
                        args.Result = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor)).Contains((string)await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "setTitle":
                        SetTitle(sender, args, parEvent, data, nor);
                        break;

                    case "setTitleDetail":
                        SetTitleDetail(sender, args, parEvent, data, nor);
                        break;

                    case "setVisible":
                        SetVisible(sender, args, parEvent, data, nor);
                        break;

                    case "setVisibleDetail":
                        SetVisibleDetail(sender, args, parEvent, data, nor);
                        break;

                    case "setVisibleTabOfInputs":
                        SetVisibleTabOfInputs(sender, args, parEvent, data, nor);
                        break;

                    case "setReadOnly":
                        SetReadOnly(sender, args, parEvent, data, nor);
                        break;

                    case "setReadOnlyDetail":
                        SetReadOnlyDetail(sender, args, parEvent, data, nor);
                        break;

                    case "setAllowNulls":
                        SetAllowNulls(sender, args, parEvent, data, nor);
                        break;

                    case "setAllowNullsDetail":
                        SetAllowNullsDetail(sender, args, parEvent, data, nor);
                        break;

                    case "setItems":
                        SetItems(sender, args, parEvent, data, nor);
                        break;

                    case "split":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor)).ToString().Split((string)await args.Eval(sender, parEvent, data, 1, nor));
                        break;

                    case "array":
                        args.Result = ((await args.Eval(sender, parEvent, data, 0, nor)) as string[])[Convert.ToInt32(await args.Eval(sender, parEvent, data, 1, nor))];
                        break;

                    case "last":
                        args.Result = ((await args.Eval(sender, parEvent, data, 0, nor)) as string[]).Last();
                        break;

                    case "addDate":
                        args.Result = ((DateTime)(await args.Eval(sender, parEvent, data, 0, nor))).AddDays(Convert.ToInt32(await args.Eval(sender, parEvent, data, 1, nor)));
                        break;

                    case "getDate":
                        args.Result = DateTime.Now.Date.AddDays(Convert.ToInt32(await args.Eval(sender, parEvent, data, 0, nor)));
                        break;

                    case "getMonth":
                        args.Result = DateTime.Now.Month + Convert.ToInt32(await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "getYear":
                        args.Result = DateTime.Now.Year + Convert.ToInt32(await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "goTo":
                        await Launcher.OpenAsync((string)await args.Eval(sender, parEvent, data, 0, nor));
                        break;

                    case "request":
                        _ = (sender as FPageFilter).Request((string)await args.Eval(sender, parEvent, data, 0, nor), FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor)));
                        break;

                    case "setFormatNumber":
                        SetFormatNumber(sender, args, parEvent, data, nor);
                        break;

                    case "setFormatNumberDetail":
                        SetFormatNumberDetail(sender, args, parEvent, data, nor);
                        break;

                    case "swap":
                        Swap(sender, args, parEvent, data, nor);
                        break;

                    case "swapDetail":
                        SwapDetail(sender, args, parEvent, data, nor);
                        break;

                    case "getValueByLanguage":
                        args.Result = (string)await args.Eval(sender, parEvent, data, FSetting.V ? 0 : 1, nor);
                        break;

                    case "getReference":
                        var path = await args.Eval(sender, parEvent, data, 0, nor);
                        var re = $"FastMobile.FXamarin.Core.FJSInvoke.{FString.ServiceName}.{FString.UserID}.{path}".GetCache();
                        args.Result = string.IsNullOrEmpty(re) ? (await args.Eval(sender, parEvent, data, 1, nor)) : re;
                        break;

                    case "setReference":
                        var path2 = (string)await args.Eval(sender, parEvent, data, 0, nor);
                        (await args.Eval(sender, parEvent, data, 1, nor)).SetCache($"FastMobile.FXamarin.Core.FJSInvoke.{FString.ServiceName}.{FString.UserID}.{path2}");
                        break;

                    case "clearReference":
                        var path3 = (string)await args.Eval(sender, parEvent, data, 0, nor);
                        $"FastMobile.FXamarin.Core.FJSInvoke.{FString.ServiceName}.{FString.UserID}.{path3}".RemoveCache();
                        break;

                    case "showMessage":
                        FFunc.CatchMessage((string)await args.Eval(sender, parEvent, data, args.Count() == 1 || FSetting.V ? 0 : 1, nor));
                        break;

                    case "warningSaved":
                        Warning(sender, args, parEvent, data, nor);
                        break;

                    case "confirmMessage":
                        args.Result = await ConfirmMessage(sender, args, parEvent, data, nor);
                        break;

                    case "goBack":
                        GoBack(sender, args, parEvent, data, nor);
                        break;

                    case "return":
                        (sender as IFStatus).Success = (bool)await args.Eval(sender, parEvent, data, 0, nor);
                        break;

                    case "If":
                        args.Result = await If(true, sender, args, parEvent, data, nor);
                        break;

                    case "Iff":
                        args.Result = await Iff(true, sender, args, parEvent, data, nor);
                        break;

                    case "IfAsync":
                        args.Result = await If(false, sender, args, parEvent, data, nor);
                        break;

                    case "IffAsync":
                        args.Result = await Iff(false, sender, args, parEvent, data, nor);
                        break;

                    case "wait":
                        await Task.Delay(Convert.ToInt32(await args.Eval(sender, parEvent, data, 0, nor)));
                        break;

                    case "print":
                        if (args.Count() == 1) await new FPagePDF((string)await args.Eval(sender, parEvent, data, 0, nor)).Show((sender as FPage));
                        else await new FPagePDF((string)await args.Eval(sender, parEvent, data, 0, nor), (bool)await args.Eval(sender, parEvent, data, 1, nor)).Show((sender as FPage));
                        break;

                    case "onSelectMode":
                        OnSelectMode(sender, args, parEvent, data, nor);
                        break;

                    case "invokeScripts":
                        InvokeScript(sender, args, parEvent, data, nor);
                        break;

                    case "setMenuVisible":
                        SetMenuVisible(sender, args, parEvent, data, nor);
                        break;

                    case "isTaxCode":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString()?.IsTaxCode();
                        break;

                    case "isPrimaryKey":
                        args.Result = await IsPrimaryKey(sender, args, parEvent, data);
                        break;

                    case "isEmail":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString()?.IsEmail();
                        break;

                    case "isListEmail":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString()?.IsListEmail();
                        break;

                    case "isUrl":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString()?.IsUrl();
                        break;

                    case "isContainUrl":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString()?.IsContainUrl();
                        break;

                    case "isUnicode":
                        args.Result = (await args.Eval(sender, parEvent, data, 0, nor))?.ToString().IsUnicode();
                        break;

                    case "showLine":
                        ShowLine(sender, args, parEvent, data, nor);
                        break;

                    case "now":
                        args.Result = DateTime.Now.Date.ToOADate();
                        break;

                    case "toDateTime":
                        args.Result = DateTime.FromOADate(Convert.ToDouble(await args.Eval(sender, parEvent, data, 0, nor)));
                        break;

                    case "dateTimeToStringFormat":
                        args.Result = DateTime.FromOADate(Convert.ToDouble(await args.Eval(sender, parEvent, data, 0, nor))).ToString((string)await args.Eval(sender, parEvent, data, 1, nor));
                        break;

                    case "toDatetimeFormat":
                        args.Result = FUtility.ToDate((string)await args.Eval(sender, parEvent, data, 0, nor), (string)await args.Eval(sender, parEvent, data, 1, nor));
                        break;

                    case "checkDetail":
                        args.Result = await CheckDetail(sender, args, parEvent, data);
                        break;

                    case "insertDetail":
                        InsertDetail(sender, args, parEvent, data, nor);
                        break;

                    case "selectInsertDetail":
                        SelectInsertDetail(sender, args, parEvent, data, nor);
                        break;

                    case "clearDetail":
                        ClearDetail(sender, args, parEvent, data, nor);
                        break;

                    case "refresh":
                        Refresh(sender, args, parEvent, data, nor);
                        break;

                    case "reload":
                        Reload(sender, args, parEvent, data, nor);
                        break;

                    case "focus":
                        Focus(sender, args, parEvent, data, nor);
                        break;

                    case "unfocus":
                        Unfocus(sender, args, parEvent, data, nor);
                        break;

                    case "setScriptDetail":
                        SetScriptDetail(sender, args, parEvent, data, nor);
                        break;

                    case "addScriptInput":
                        AddScriptInput(sender, args, parEvent, data, nor);
                        break;

                    case "removeScriptInput":
                        RemoveScriptInput(sender, args, parEvent, data, nor);
                        break;

                    case "executeCommand":
                        ExecuteCommand(sender, args, parEvent, data, nor);
                        break;

                    case "nextRecord":
                        NextRecord(sender, args, parEvent, data, nor);
                        break;

                    case "backRecord":
                        BackRecord(sender, args, parEvent, data, nor);
                        break;

                    case "getParentPage":
                        args.Result = GetParentPage(sender);
                        break;

                    case "checkConflictDetail":
                        args.Result = await CheckConflictDetail(sender, args, parEvent, data, nor);
                        break;

                    case "toBool":
                        args.Result = (bool)await args.Eval(sender, parEvent, data, 0, nor);
                        break;

                    case "toBoolAsync":
                        args.Result = await ToBoolAsync(sender, args, parEvent, data, nor);
                        break;

                    case "showBio":
                        args.Result = await ShowBio(sender, args, parEvent, data, nor);
                        break;

                    case "showBioPassword":
                        args.Result = await ShowBioPassword(sender, args, parEvent, data, nor);
                        break;
                    case "updateMenuBadge":
                        UpdateMenuBadge(sender, args, parEvent, data, nor);
                        break;

                    case "ExcuteFormCommand":
                        ExcuteFormCommand(sender, args, parEvent, data, nor);
                        break;

                    case "matchPattern":
                        args.Result = await MatchPattern(sender, args, parEvent, data, nor);
                        break;

                    case "getPattern":
                        args.Result = await GetPattern(sender, args, parEvent, data, nor);
                        break;

                    case "doNothing":
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                if (FSetting.IsDebug) MessagingCenter.Send(new FMessage($"Script: {expression} . Error: {ex.Message}"), FChannel.ALERT_BY_MESSAGE);
            }
            finally
            {
                afterInvoke?.Invoke(args.Result, true, name);
            }
        }

        #region Private

        #region Object

        private static FPageReport GetParentPage(object sender)
        {
            if (sender is not FPageReport page) return null;
            return page?.BeforePage;
        }

        #endregion Object

        #region String

        private static async Task<string> SerializeJson(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var d = new Dictionary<string, string>();
            var p = (await args.Eval(sender, parEvent, data, 2, nor)).ToString()[0];
            var k = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor), p);
            var v = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor), p);
            k.ForIndex((x, i) => d.Add(x, v[i]));
            return d.ToJson();
        }

        private static async Task<string> GetPattern(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var text = await args.Eval(sender, parEvent, data, 0, nor) as string;
            var pattern = await args.Eval(sender, parEvent, data, 1, nor) as string;
            var index = args.Count() < 3 ? 0 : Convert.ToInt32(await args.Eval(sender, parEvent, data, 2, nor));
            return new Regex(pattern).Match(text).Groups[index].Value;
        }

        #endregion String

        #region Bool

        private static bool IsFunc(string expression)
        {
            if (!expression.Contains('(') || !expression.Contains(')'))
                return false;
            var exp = expression.Trim();
            if (!exp.StartsWith("!") && !exp.StartsWith("not"))
                return IsFuncIn(expression.Substring(0, expression.IndexOf('(')));
            exp = exp.Replace("!", "").Replace("not", "").Replace("not ", "");
            return IsFuncIn(exp.Substring(0, exp.IndexOf('(')));
        }

        private static bool IsFuncIn(string expression)
        {
            return Functions.Contains(expression.Trim());
        }

        private static bool IsExpression<T>(this LogicalExpression expression) where T : LogicalExpression
        {
            if (expression is T) return true;
            var result = false;
            while (expression is not T)
            {
                if (expression is BinaryExpression binary)
                {
                    result = binary.LeftExpression.IsExpression<T>() || binary.RightExpression.IsExpression<T>();
                    break;
                }

                if (expression is UnaryExpression unary)
                {
                    result = unary.Expression.IsExpression<T>();
                    break;
                }

                if (expression is TernaryExpression ternary)
                {
                    result = ternary.LeftExpression.IsExpression<T>() || (ternary.MiddleExpression.IsExpression<T>() ? true : ternary.RightExpression.IsExpression<T>());
                    break;
                }

                break;
            }
            return result;
        }

        private static bool IsBinaryMutiple(this BinaryExpression expression)
        {
            return expression.IsExpression<ValueExpression>() && expression.IsExpression<Function>();
        }

        private static async Task<bool> If(bool async, object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var check = (bool)await args.Eval(sender, parEvent, data, 0, async);
            object invoke = null;
            if (check) invoke = nor ? Invoke(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data) : await InvokeAsync(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data);
            else invoke = nor ? Invoke(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data) : await InvokeAsync(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data);
            if (invoke is bool result) return result;
            else return check;
        }

        private static async Task<bool> Iff(bool async, object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var check = (bool)await args.Eval(sender, parEvent, data, 0, async);
            object invoke = await args.Eval(sender, parEvent, data, check ? 1 : 2, nor);
            if (invoke is bool result) return result;
            else return check;
        }

        private static async Task<bool> IsPrimaryKey(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var key = (string)await args.Eval(sender, parEvent, data, 0, nor);
            return args.Count() == 1 ? key.IsPrimaryKey() : key.IsPrimaryKey((string)await args.Eval(sender, parEvent, data, 1, nor));
        }

        private static async Task<bool> CheckDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return true;
            var name = (string)await args.Eval(sender, parEvent, data, 0, nor);
            if (form.Input.TryGetValue(name, out FInput input) && input is FInputTable detail)
            {
                var result = true;
                var method = (string)await args.Eval(sender, parEvent, data, 1, nor);
                var condition = (string)await args.Eval(sender, parEvent, data, 2, nor);
                var r = new Regex(name + @"\((.+?)\)");
                detail.Source.ForEach(x =>
                {
                    var c = condition;
                    var m = r.Matches(c);
                    m.ForEach(m =>
                    {
                        var n = m.Groups[1].Value;
                        var p = $"{name}({n})";
                        var v = x[n];
                        if (v.Equals(FInputDate.BaseValue)) c = c.Replace(p, "0");
                        else if (v is DateTime d) c = c.Replace(p, d == default ? "0" : ((int)d.ToOADate()).ToString());
                        else if (v is bool b) c = c.Replace(p, b.ToString().ToLower());
                        else c = c.Replace(p, v.ToString());
                    });
                    if ((bool)Invoke(sender, c, data))
                    {
                        result = false;
                        if (nor) Invoke(sender, method, data);
                        else _ = InvokeAsync(sender, method, data);
                        return;
                    }
                });
                return result;
            }
            return true;
        }

        private static async Task<bool> CheckConflictDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return true;
            var name = (string)await args.Eval(sender, parEvent, data, 0, nor);
            if (form.Input.TryGetValue(name, out FInput input) && input is FInputTable detail)
            {
                var keys = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
                if (keys.Count == 0) return false;
                var table = detail.Value?.Table?.Copy();
                if (table == null || table.Rows.Count <= 1) return false;
                var distinct = table.DefaultView.ToTable(true, keys.ToArray());
                return distinct.Rows.Count != table.Rows.Count;
            }
            return false;
        }

        private static async Task<bool> ConfirmMessage(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var msg = (string)await args.Eval(sender, parEvent, data, 0, nor);
            var result = msg.Contains(" ") ? await new FAlertBase().Confirm(msg) : await FAlertHelper.Confirm(msg);
            if (result)
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data);
            else if (args.Count() > 2)
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data);
            return result;
        }

        private static async Task<bool> ShowBio(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var bioNative = !FSetting.IsAndroid && ((bool)await args.Eval(sender, parEvent, data, 0, nor));
            var alert = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            var trueMethod = (string)await args.Eval(sender, parEvent, data, 2, nor);
            var falseMethod = (string)await args.Eval(sender, parEvent, data, 3, nor);

            if (!FSetting.UseLocalAuthen)
            {
                Completed(falseMethod, string.Empty);
                if (alert) MessagingCenter.Send(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FSetting.SystemLanguageIsV ? "Hủy" : "Cancel");
            config.AllowAlternativeAuthentication = bioNative;
            var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

            if (authResult.IsAuthenticated)
            {
                Completed(trueMethod, FString.Password);
                return true;
            }

            if (authResult.Status == FFingerprintAuthenticationResultStatus.TooManyAttempts && alert)
            {
                MessagingCenter.Send(FMessage.FromFail(1254), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            if (authResult.Status == FFingerprintAuthenticationResultStatus.Canceled)
                return false;

            if (alert) MessagingCenter.Send(FMessage.FromFail(1252), FChannel.ALERT_BY_MESSAGE);
            Completed(falseMethod, string.Empty);
            return false;

            void Completed(string method, string password)
            {
                Invoke(sender, method.Replace("[biometricValue]", password), data);
            }
        }

        private static async Task<bool> ShowBioPassword(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var bioNative = !FSetting.IsAndroid && ((bool)await args.Eval(sender, parEvent, data, 0, nor));
            var alert = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            var trueMethod = (string)await args.Eval(sender, parEvent, data, 2, nor);
            var falseMethod = (string)await args.Eval(sender, parEvent, data, 3, nor);

            if (!FSetting.UseLocalAuthen)
                return await ShowPassword(FSetting.V ? "Xác nhận mật khẩu" : "Confirm Password");

            var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FSetting.SystemLanguageIsV ? "Hủy" : "Cancel");
            config.AllowAlternativeAuthentication = bioNative;
            var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

            if (authResult.IsAuthenticated)
            {
                Completed(trueMethod, FString.Password);
                return true;
            }

            //if (authResult.Status == FFingerprintAuthenticationResultStatus.TooManyAttempts && alert)
            //{
            //    MessagingCenter.Send(FMessage.FromFail(1254), FChannel.ALERT_BY_MESSAGE);
            //    return false;
            //}

            //if (authResult.Status == FFingerprintAuthenticationResultStatus.Canceled)
            //    return false;

            return await ShowPassword(FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V) + (FSetting.V ? " thất bại, vui lòng xác thực mật khẩu" : " failed, please confirm password"));

            async Task<bool> ShowPassword(string text)
            {
                var (OK, Secret) = await FServices.ConfirmPassword(text, alert);
                if (OK)
                {
                    Completed(trueMethod, Secret);
                    return true;
                }

                Completed(falseMethod, string.Empty);
                return false;
            }

            void Completed(string method, string password)
            {
                Invoke(sender, method.Replace("[biometricValue]", password), data);
            }
        }

        private static async Task<bool> ToBoolAsync(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var mode = await args.Eval(sender, parEvent, data, 0, nor) as string;
            if (string.IsNullOrEmpty(mode))
                return false;
            var isAnd = mode.ToLower() == "and";
            var result = isAnd;

            for (int i = 1; i < args.Count(); i++)
            {
                var check = (bool)await args.Eval(sender, parEvent, data, i, nor);
                result = isAnd ? result && check : result || check;
            }
            return result;
        }

        private static async Task<bool> MatchPattern(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var text = await args.Eval(sender, parEvent, data, 0, nor) as string;
            var pattern = await args.Eval(sender, parEvent, data, 1, nor) as string;
            return Regex.IsMatch(text, pattern);
        }

        #endregion Bool

        #region Void

        private static async void SetTitle(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = (string)await args.Eval(sender, parEvent, data, 0, nor);
            var title = (string)await args.Eval(sender, parEvent, data, 1, nor);
            if (!title.Contains(",") && !title.Contains("|"))
            {
                if (form.Input.TryGetValue(input, out FInput i)) i.Title = title;
                return;
            }

            var inputs = FFunc.GetArrayString(input);
            var titles = FFunc.GetArrayString(title, clearSpace: false);
            inputs.ForIndex((x, k) => { if (form.Input.TryGetValue(x, out FInput i)) i.Title = titles[k].Split("|")[FSetting.V ? 0 : 1]; });
        }

        private static async void SetTitleDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = (string)await args.Eval(sender, parEvent, data, 1, nor);
            var title = (string)await args.Eval(sender, parEvent, data, 2, nor);

            if (!title.Contains(",") && !title.Contains("|"))
            {
                if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputGrid d)
                {
                    if (d.Detail.Settings.Fields.Find(f => f.Name == input) is FField f) f.Title = title;
                    d.InitColumn();
                }
                return;
            }

            var inputs = FFunc.GetArrayString(input);
            var titles = FFunc.GetArrayString(title, clearSpace: false);
            await inputs.ForIndex(async (x, k) =>
            {
                if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputGrid d)
                {
                    if (d.Detail.Settings.Fields.Find(f => f.Name == x) is FField f) f.Title = titles[k].Split("|")[FSetting.V ? 0 : 1];
                    d.InitColumn();
                }
            });
        }

        private static async void OnSelectMode(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var status = args.Count() < 1 || (bool)await args.Eval(sender, parEvent, data, 0, nor);
            if (sender is FPageReport report && report.Grid.IsAlowSelect == !status) await report.Grid.SelectButton(null);
            else if (sender is FPageFilter form && form.Root is FPageReport root && root.Grid.IsAlowSelect == !status) await root.Grid.SelectButton(null);
        }

        private static async void GoBack(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not FPage page) return;
            if (FSetting.IsAndroid) await Task.Delay(400);
            if ((bool)await args.Eval(sender, parEvent, data, 0, nor)) await page.Navigation.PopToRootAsync(true);
            else await page.Navigation.PopAsync(true);
            if (args.Count() > 1)
            {
                await Task.Delay(150);
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data);
            }
        }

        private static async void SetVisibleTabOfInputs(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var isVisible = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i) && i.Expander != null) i.Expander.Show = isVisible; });
        }

        private static async void SetVisible(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var isVisible = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i)) i.IsVisible = isVisible; });
        }

        private static async void SetVisibleDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var isVisible = (bool)await args.Eval(sender, parEvent, data, 2, nor);
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput detail) && detail is FInputGrid d)
            {
                input.ForEach(x => { if (d.Detail.Settings.Fields.Find(f => f.Name == x) is FField f) f.Hidden = !isVisible; });
                d.InitColumn();
            }
        }

        private static async void SetMenuVisible(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            FPageReport r = sender is FPageFilter f ? f.Root as FPageReport : sender as FPageReport;
            var menu = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var visible = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            r.MenuButton.ToList().ForEach(x => { if (menu.Contains(x.Toolbar.Command)) x.Visible = visible; });
            r.RefreshMenuView();
        }

        private static async void SetReadOnly(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var isReadOnly = !(bool)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i)) i.IsReadOnly = isReadOnly; });
        }

        private static async void SetReadOnlyDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var isReadOnly = (bool)await args.Eval(sender, parEvent, data, 2, nor);
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput detail) && detail is FInputGrid d)
                input.ForEach(x => { if (d.Detail.Settings.Fields.Find(f => f.Name == x) is FField f) f.IsReadOnly = isReadOnly; });
        }

        private static async void SetAllowNulls(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var notAllowNulls = !(bool)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i)) i.NotAllowsNull = notAllowNulls; });
        }

        private static async void SetAllowNullsDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var isAllowNulls = (bool)await args.Eval(sender, parEvent, data, 2, nor);
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput detail) && detail is FInputGrid d)
                input.ForEach(x => { if (d.Detail.Settings.Fields.Find(f => f.Name == x) is FField f) f.AllowNulls = isAllowNulls; });
        }

        private static async void SetItems(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var isList = args.Count() > 2 && ((bool)await args.Eval(sender, parEvent, data, 2, nor));
            var isCompleted = args.Count() > 3 && (bool)await args.Eval(sender, parEvent, data, 3, nor);
            var list = ((string)await args.Eval(sender, parEvent, data, 1, nor)).Split('ÿ');

            input.ForIndex((x, j) =>
            {
                if (form.Input.TryGetValue(x, out FInput i))
                    i.SetInput(isList ? list[j] : list[0], isCompleted);
            });
        }

        private static async void SetFormatNumber(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var format = (string)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i) && i is FInputNumber f) f.Format = format; });
        }

        private static async void SetFormatNumberDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var format = (string)await args.Eval(sender, parEvent, data, 2, nor);
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputGrid d)
            {
                input.ForEach(x => { if (d.Detail.Settings.Fields.Find(f => f.Name == x) is FField f) f.DataFormatString = format; });
                d.InitColumn();
            }
        }

        private static async void Swap(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input1 = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var input2 = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var isSwapTitle = args.Count() <= 2 || ((bool)await args.Eval(sender, parEvent, data, 2, nor));
            input1.ForIndex((x, i) => { if (form.Input.TryGetValue(x, out FInput a) && form.Input.TryGetValue(input2[i], out FInput b)) FPageFilter.SwapInput(a, b, isSwapTitle); });
        }

        private static async void SwapDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input1 = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));
            var input2 = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 2, nor));
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputGrid d)
            {
                var field = d.Detail.Settings.Fields;
                input1.ForIndex((x, i) => { if (field.Find(f => f.Name == x) is FField f1 && field.Find(f => f.Name == input2[i]) is FField f2) FPageFilter.SwapFields(f1, f2); });
                d.InitColumn();
            }
        }

        private static async void ShowLine(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var input = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var value = (bool)await args.Eval(sender, parEvent, data, 1, nor);
            input.ForEach(x => { if (form.Input.TryGetValue(x, out FInput i)) i.IsShowLine = value; });
        }

        private static async void InsertDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputTable d)
            {
                var index = Convert.ToInt32(await args.Eval(sender, parEvent, data, 1, nor));
                d.InsertDetail(data.Tables[index], false, (bool)await args.Eval(sender, parEvent, data, 2, nor));
            }
        }

        private static async void ClearDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputTable d) _ = d.ClearDetail();
        }

        private static void Refresh(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is FPageReport report) report.Grid.Refresh();
            else if (sender is IFPageFilter form && form.Root is FPageReport root) root.Grid.Refresh();
            else if (sender is FGridBase grid) grid.Refresh();
        }

        private static void Reload(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is FPageReport report) _ = report.Grid.LoadingGrid(report.Grid.TargetType);
            else if (sender is IFPageFilter form && form.Root is FPageReport root) _ = root.Grid.LoadingGrid(root.Grid.TargetType);
            else if (sender is FGridBase grid) _ = grid.LoadingGrid(grid.TargetType);
        }

        private static async void Focus(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is IFPageFilter form && form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput input)) input.FocusInput();
        }

        private static async void Unfocus(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is IFPageFilter form && form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput input)) input.UnFocusInput();
        }

        private static async void InvokeScript(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var scriptID = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var detail = args.Count() < 2 ? string.Empty : ((string)await args.Eval(sender, parEvent, data, 1, nor));
            scriptID.ForEach(x => { _ = form.InvokeScript(x, detail, null); });
        }

        private static async void SetScriptDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is IFPageFilter form && form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput input) && input is FInputGrid detail)
                detail.Detail.ClientScript = (string)await args.Eval(sender, parEvent, data, 1, nor);
        }

        private static async void AddScriptInput(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var fields = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var scripts = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));

            fields.ForEach(f => { if (form.Input.TryGetValue(f, out FInput input)) input.AddScript(scripts); });
        }

        private static async void RemoveScriptInput(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            var fields = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 0, nor));
            var scripts = FFunc.GetArrayString((string)await args.Eval(sender, parEvent, data, 1, nor));

            fields.ForEach(f => { if (form.Input.TryGetValue(f, out FInput input)) input.RemoveScript(scripts); });
        }

        private static async void ExecuteCommand(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not FPageReport report) return;
            var grid = report.Grid;
            var command = (string)await args.Eval(sender, parEvent, data, 0, nor);
            var commandArgument = (string)await args.Eval(sender, parEvent, data, 1, nor);
            var toolbar = grid.Settings.Toolbars.Find(t => t.Command == command);
            var data2 = grid.GridView.SelectedIndex == -1 ? null : grid.Source[grid.GridView.SelectedIndex - 1];

            if (toolbar == null) return;
            if (string.IsNullOrWhiteSpace(commandArgument)) commandArgument = null;
            toolbar.CommandArgument = commandArgument;
            if (command.Equals("New"))
            {
                await grid.Toolbar.NewRecordGrid(toolbar);
                return;
            }
            if (command.Equals("Delete") && data2 != null)
            {
                await grid.Toolbar.DeleteRecordGrid(toolbar, data2);
                return;
            }
        }

        private static async void NextRecord(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not FPageReport report) return;
            if (await report.Grid.Next((bool)await args.Eval(sender, parEvent, data, 0, nor)))
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data);
            else
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data);
        }

        private static async void BackRecord(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            if (sender is not FPageReport report) return;
            if (await report.Grid.Back((bool)await args.Eval(sender, parEvent, data, 0, nor)))
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 1, nor), data);
            else
                Invoke(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data);
        }

        private static async void Warning(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data = null, bool nor = false)
        {
            var message = (string)await args.Eval(sender, parEvent, data, 0, nor);
            if (string.IsNullOrWhiteSpace(message)) return;
            if (!message.Contains(" ")) message = FAlertHelper.MessageByCode(message);
            message += FSetting.V ? $"<br>@@Lưu ý: Hệ thống vẫn lưu dữ liệu vừa cập nhật.@@" : $"<br>@@Warnings: Updated data has been saved.@@";
            await new FAlert().Show(message);
        }

        private static async void SelectInsertDetail(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            if (form.Input.TryGetValue((string)await args.Eval(sender, parEvent, data, 0, nor), out FInput dt) && dt is FInputGrid d)
            {
                var index = Convert.ToInt32(args.Parameters[1].Evaluate());
                d.ShowSelectionPage(data.Tables[index], Accept, Cancel, args.Count() > 4 && (bool)await args.Eval(sender, parEvent, data, 4, nor), args.Count() > 5 ? (string)await args.Eval(sender, parEvent, data, 5, nor) : null);
            }

            async void Accept(FInputGridValue v1, FInputGridValue v2)
            {
                if (nor) Invoke(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), new DataSet().AddTable(v1.Table).AddTable(v2.Table));
                else await InvokeAsync(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), new DataSet().AddTable(v1.Table).AddTable(v2.Table));
            }

            async void Cancel()
            {
                if (nor) Invoke(sender, (string)await args.Eval(sender, parEvent, data, 3, nor), data);
                else await InvokeAsync(sender, (string)await args.Eval(sender, parEvent, data, 2, nor), data);
            }
        }

        private static async void UpdateMenuBadge(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data, bool nor = false)
        {
            if (FApplication.Current is FApplication fapp && fapp.Notify is FPageNotify notifyPage)
                notifyPage.OnReceived((string)await args.Eval(sender, parEvent, data, 0, nor), (string)await args.Eval(sender, parEvent, data, 1, nor));
        }

        private static async void ExcuteFormCommand(object sender, FunctionArgs args, EvaluateFunctionHandler parEvent, DataSet data, bool nor = false)
        {
            if (sender is not IFPageFilter form) return;
            await form.ExcuteButton();
        }

        #endregion

        #endregion Private
    }

    internal class CallermemberNameAttribute : Attribute
    {
    }
}