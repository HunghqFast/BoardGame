using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FPageFilterMethod
    {
        private readonly IFPageFilter form;
        private readonly IFStatus status;
        private readonly List<Func<Task>> extendFunc;
        private readonly List<Func<Task>> extendAction;

        public bool IsCopy { get; set; }

        public FPageFilterMethod(IFPageFilter filter, IFStatus status)
        {
            this.form = filter;
            this.status = status;
            this.extendFunc = new List<Func<Task>>();
            this.extendAction = new List<Func<Task>>();
        }

        public FInput GetFInput(FInput input)
        {
            input.InitValue();
            FillValueInit(input);
            FillValue(input, false);
            return input;
        }

        public void FillValueInit(FInput input)
        {
            if (form.InitData == null || form.InitData.Tables.Count == 0) return;
            var checkData = form.InitData.Tables[0]?.Rows;
            if (checkData != null && checkData.Count > 0 && checkData[0].Table.Columns.Contains(input.Name)) input.SetInput(checkData[0][input.Name]);
        }

        public async void FillValue(FInput input, bool isDisable)
        {
            if (form.InputData == null || input == null) return;
            var data = form.InputData[input.Name];
            if (data.ToString() == FInput.IsValidData) return;
            if (input is FInputTable inputTable && !(isDisable && inputTable.Disable)) extendFunc.Add(inputTable.InitData);
            if (input is IFInputRequest inputRequest) extendAction.Add(inputRequest.InvokeService);
            if (input is not FInputTable) input.SetInput(data, isDisable: isDisable);
            if (isDisable && !input.Disable && input.IsVisible && !form.InputEdited.Contains(input.Name)) form.InputEdited.Add(input.Name);
            await Task.CompletedTask;
        }

        public async Task InitSettings(bool isHasSettings)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Error(403);
                return;
            }

            if (isHasSettings)
            {
                await Created(form.Settings);
                return;
            }

            var settings = await FViewPage.InitSettingsFromDevice(form.SettingsDeviceID);
            if (settings != null)
            {
                await Created(new FViewPage(settings));
                return;
            }

            var mess = await GetSettings();
            if (mess.Success == 1) await Created(new FViewPage(form.SettingsDeviceID, (JObject)JObject.Parse(mess.Message.AESDecrypt(FSetting.NetWorkKey)).SelectToken("ViewPage")));
            else await Error(mess.Code);
        }

        #region Button

        public virtual async void OnSaveToobarClicked()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send(new FMessage(0, 2, ""), FChannel.ALERT_BY_MESSAGE);
                FData.FDataToString(form.FDataDirForm(1)).SetCache(form.SaveToolbarDeviceID);
                form.InitToobar();
            });
            await Task.CompletedTask;
        }

        public virtual async void OnDownFileToobarClicked()
        {
            await form.Page.SetBusy(true);
            if (await FAlertHelper.Confirm("3"))
            {
                await Task.Delay(100);
                IsCopy = true;
                form.InputData = FData.StringToFData(form.SaveToolbarDeviceID.GetCache());
                form.FillAll(true);
                form.Input.ForEach(i => { if (i.Value.ScriptCopy) i.Value.Completed(); });
                IsCopy = false;
                form.SaveToolbarDeviceID.RemoveCache();
                form.InitToobar();
            }
            await form.Page.SetBusy(false);
        }

        public virtual async void OnTipsToolbarClicked()
        {
            await form.Page.SetBusy(true);
            await FPageReport.SetReportByAction(form.Page, "Inquiry", "", form.Root as FPageReport, new DataSet().AddTable(new DataTable().AddRowValue("default", "")), form.Settings.Tips);
            await form.Page.SetBusy(false);
        }

        #endregion Button

        #region Command

        public async Task<FMessage> Initing()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", form.Target.ToString());
            if (form.GridData != null)
            {
                var gr = (form.Root as FPageReport).Grid;
                FFunc.AddFieldInput(ref ds, new DataTable(), gr.Settings.Fields, form.GridData);
                if (gr.Details != null) FFunc.AddFieldInput(ref ds, new DataTable(), gr.Settings.Details, gr.Details);
            }
            FMessage result = await FServices.ExecuteCommand("Initing", form.Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        public async Task<FMessage> Checking()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", form.Target.ToString());
            FFunc.AddFieldInput(ref ds, new DataTable(), form.Settings.Fields, form.FDataDirForm());
            FMessage result = await FServices.ExecuteCommand("Checking", form.Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        public async Task<FMessage> Requesting(string action, List<string> sender)
        {
            var ds = new DataSet();
            var fi = new List<FField>();
            var it = new FData();

            sender.ForEach(s =>
            {
                var r = FInput.IsRootParam(s) ? (form.Root as FInputGrid).Root : form.Page as FPageFilter;
                var f = r.Settings.Fields.Find(f => f.Name == FInput.RootParam(s)).Clone() as FField;
                var i = r.Input[f.Name];

                if (f != null)
                {
                    fi.Add(f);
                    it[f.Name, i.Type] = i.GetInput(0);
                }
            });
            FFunc.AddTypeAction(ref ds, "response", form.Target.ToString(), form.FormType.ToString(), isCopy: IsCopy);
            FFunc.AddFieldInput(ref ds, new DataTable(), fi, it);
            FMessage result = await FServices.ExecuteCommand(action, form.Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        public async Task<FMessage> GetSettings()
        {
            return await FServices.ExecuteCommand(form.Target.ToString(), form.Controller, new DataSet().AddTable(new DataTable().AddRowValue("type", form.Target)), "300", null, true);
        }

        #endregion Command

        #region Check

        public async Task<bool> CheckDataSet(DataSet data)
        {
            await CheckScripts(data);
            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0) return true;
            var table = data.Tables[0];
            if (!table.Columns.Contains("message")) return true;
            var msg = table.Rows[0]["message"].ToString();
            if (string.IsNullOrWhiteSpace(msg)) return true;
            FFunc.CatchMessage(msg);
            return false;
        }

        public async Task<bool> CheckFault()
        {
            foreach (FField fi in form.Settings.Fields)
            {
                if (!form.Input.TryGetValue(fi.Name, out FInput input)) continue;

                if (fi.ItemStyle == FItemStyle.Grid && !fi.ItemAllowNulls && input is FInputGrid grid && grid.Source.Count == 0)
                    return await ShowError(613, input.NoticeName, input, true);

                var c = input.GetInput(0)?.ToString();
                if (input.NotAllowsNull && ((input.Type == FieldType.Number && decimal.Parse(c) == 0) || (input.Type == FieldType.DateTime && DateTime.Parse(c) == default) || c.Equals("")))
                    return await ShowError(606, input.NoticeName, input, true);

                if (!input.IsReadOnly) continue;

                if (input is FInputLookup lookup) { if (!lookup.IsValidValue && lookup.HasValid) return await ShowError(606, input.NoticeName, input, true); }
                else { if (!input.IsValidValue) return await ShowError(606, input.NoticeName, input, true); }

                if (fi.IsPrimaryKey && !c.IsPrimaryKey())
                    return await ShowError(40, input.NoticeName, input, true);

                if (fi.ItemStyle == FItemStyle.Tax && !c.Equals("") && !c.IsTaxCode())
                    return await ShowError(606, input.NoticeName, input, true);

                if (fi.ItemStyle == FItemStyle.Email && !c.Equals("") && !c.IsListEmail())
                    return await ShowError(236, input.NoticeName, input, true);

                if (fi.ItemStyle == FItemStyle.CaptCha && !await (input as FInputCaptcha).Valid())
                    return await Task.FromResult(false);

                if (!string.IsNullOrEmpty(fi.Check) && !(bool)await input.InvokeScriptsAsync(fi.Check))
                    return await Task.FromResult(false);

                try
                {
                    if (fi.Condition.Equals("")) continue;

                    var fr = fi.Condition.IndexOf("[") + 1;
                    var to = fi.Condition.IndexOf("]");
                    var indexCondition = form.Input[fi.Condition[fr..to]];
                    var result = indexCondition.GetInput(0).ToString().Trim();

                    if (fi.Condition.Contains("<="))
                    {
                        if (FFunc.Compare(c, result, input.Type) == 1)
                            return await ShowError(607, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                    else if (fi.Condition.Contains(">="))
                    {
                        if (FFunc.Compare(c, result, input.Type) == -1)
                            return await ShowError(608, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                    else if (fi.Condition.Contains("<>"))
                    {
                        if (FFunc.Compare(c, result, input.Type) == 0)
                            return await ShowError(612, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                    else if (fi.Condition.Contains("<"))
                    {
                        if (FFunc.Compare(c, result, input.Type) != 1)
                            return await ShowError(609, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                    else if (fi.Condition.Contains(">"))
                    {
                        if (FFunc.Compare(c, result, input.Type) != -1)
                            return await ShowError(610, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                    else if (fi.Condition.Contains("=="))
                    {
                        if (FFunc.Compare(c, result, input.Type) != 0)
                            return await ShowError(611, input.NoticeName + (char)254 + indexCondition.NoticeName, null, true);
                    }
                }
                catch (Exception ex)
                {
                    MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                    return await Task.FromResult(false);
                }
            }
            if (form.Settings.HasCheck)
            {
                var mess = await Checking();
                if (mess.Success == 1) return await CheckDataSet(mess.ToDataSet());
            }
            return await Task.FromResult(true);
        }

        public Task<bool> CheckEdited()
        {
            if (form.FormType != FFormType.Edit) return Task.FromResult(true);
            if (form.Input.Values.Any(x => !x.IsValidValue)) return Task.FromResult(true);
            if (form.InputEdited.Count == 0) return Task.FromResult(false);
            foreach (var e in form.InputEdited)
            {
                var input = form.Input[e];
                if (!input.IsVisible) continue;
                if (!input.IsEqual(form.OldData[e])) return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task CheckScripts(DataSet data, string tag = "script")
        {
            FFunc.CatchScriptMethod(form.Page, data, tag);
            return Task.CompletedTask;
        }

        public async Task CheckExtendFunc()
        {
            if (extendFunc.Count == 0) return;
            await Task.WhenAll(extendFunc.Select(e => e?.Invoke())).ConfigureAwait(false);
        }

        public async Task InvokeExtendAction()
        {
            if (extendAction.Count == 0) return;
            await Task.WhenAll(extendAction.Select(e => e?.Invoke())).ConfigureAwait(false);
        }

        public Task ClearExtendFunc()
        {
            extendFunc.Clear();
            extendAction.Clear();
            return Task.CompletedTask;
        }

        #endregion Check

        #region Private

        private async Task InitChecking()
        {
            if (form.Settings.HasInit)
            {
                var ds = await FViewPage.InitDataFromDevice(form.DataSetDeviceID);
                if (ds != null && form.Settings.ReportCacheType != ReportCacheType.None)
                {
                    form.InitData = ds;
                    return;
                }
                var mess = await Initing();
                if (mess.Success == 1)
                {
                    try { form.InitData = mess.ToDataSet(); FViewPage.SaveDataSet(form.DataSetDeviceID, mess.Message.AESDecrypt(FSetting.NetWorkKey)); }
                    catch { form.InitData = null; }
                }
            }
            else form.InitData = null;
        }

        private async Task SetReferenceKey()
        {
            var task = new List<Task>();
            form.Input.ForEach(i => { if (i.Value is FInputLookup lookup) task.Add(lookup.SetReference()); });
            await Task.WhenAll(task).ConfigureAwait(false);
        }

        private void InvokeHandleKey()
        {
            form.Input.ForEach(k => k.Value.Handle());
        }

        private async Task Created(FViewPage settings)
        {
            form.IsOpening = true;
            form.Settings = settings;
            form.Page.Title = $"{form.Action} {form.Settings.Title}";
            form.InitToobar();
            await InitChecking();
            await ClearExtendFunc();
            await form.InitInput();
            await form.InitContent();
            form.Script?.Invoke();
            await SetReferenceKey();
            InvokeHandleKey();
            status.Success = true;
            await CheckDataSet(form.InitData);
            await CheckExtendFunc();
            FFunc.CatchScriptMethod(form.Page, form.ClientScript);
            form.OldData = form.FDataDirForm();
            form.IsOpening = false;
        }

        private async Task Error(int code)
        {
            MessagingCenter.Send(new FMessage(0, code, ""), FChannel.ALERT_BY_MESSAGE);
            status.Success = false;
            await Task.CompletedTask;
        }

        private async Task<bool> ShowError(int code, string mess, FInput input, bool showMess)
        {
            input?.FocusInput();
            await Task.Delay(50);
            if (showMess) MessagingCenter.Send(new FMessage(0, code, mess), FChannel.ALERT_BY_MESSAGE);
            return await Task.FromResult(false);
        }

        #endregion Private
    }
}