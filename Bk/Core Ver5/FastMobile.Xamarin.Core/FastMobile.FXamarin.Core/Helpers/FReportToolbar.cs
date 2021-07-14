using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FReportToolbar : IFReportToolbar
    {
        public const string BioRequestField = "biometricValue";

        private readonly IFReportSettings report;
        private FPageFilter dir => report.Root.Dir;

        public FReportToolbar(IFReportSettings report)
        {
            this.report = report;
        }

        public static async Task<FCommnadValue> TryCatchMessage(object sender, FPage root, DataSet ds, int type, Func<DataTable, Task> success, Func<Task> failed = null, bool check = true)
        {
            try
            {
                var detail = ds.Tables[0];
                var data = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable();
                if (!detail.Columns.Contains("message"))
                    return await InvokeActions(ds, detail, data, success);

                var msg = detail.Rows[0]["message"].ToString();
                if (string.IsNullOrWhiteSpace(msg))
                    return await InvokeActions(ds, detail, data, success);

                if (failed != null) await failed.Invoke();
                FFunc.CatchMessage(msg);
                return new FCommnadValue(data, false);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                await root.SetBusy(false);
                return new FCommnadValue();
            }
            finally
            {
                if (check) FFunc.CatchScriptMethod(sender, ds);
                await root.SetBusy(false);
            }
        }

        public virtual async Task<FCommnadValue> ViewRecordGrid(object obj, FData data)
        {
            var toolbar = obj as FToolbar;
            await report.Root.SetBusy(true);
            report.GridView.SelectedIndex = data.GetIndex();
            var check = await FCommand.DirLoading(report.Settings.Code, data, toolbar, report.Controller, FAction.View);
            if (check.Success == 1)
            {
                var ds = check.ToDataSet();
                return await TryCatchMessage(dir, report.Root, ds, toolbar.FormType,
                    async (dt) =>
                    {
                        await report.Root.DirCustom(FFormType.View, FData.NewItem(dt.Rows[0], dir.Settings.Fields), data, () => FFunc.CatchScriptMethod(dir, ds));
                    }, check: false);
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            await report.Root.SetBusy(false);
            return new FCommnadValue();
        }

        public virtual async Task<FCommnadValue> ScatterRecordGrid(object obj, FData data)
        {
            var toolbar = obj as FToolbar;
            var check = await FCommand.DirScattering(dir.Settings.Scatter, data, toolbar.Command, dir.Controller);
            if (check.Success == 1)
            {
                return await TryCatchMessage(dir, report.Root, check.ToDataSet(), toolbar.FormType,
                    (dt) =>
                    {
                        switch (toolbar.Command)
                        {
                            case "New":
                                dir.FormType = FFormType.New;
                                dir.InputData = dt.Rows.Count > 0 ? FData.NewItem(dt.Rows[0], dir.Settings.Fields) : null;
                                dir.ClearAll();
                                dir.FillAll(false);
                                break;

                            case "Edit":
                                dir.FormType = FFormType.Edit;
                                break;
                        }
                        return Task.CompletedTask;
                    });
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            return new FCommnadValue();
        }

        public virtual async Task<FCommnadValue> EditRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var check = await FCommand.DirLoading(report.Settings.Code, data, toolbar, report.Controller, FAction.Edit);
                if (check.Success == 1)
                {
                    var ds = check.ToDataSet();
                    return await TryCatchMessage(dir, report.Root, ds, toolbar.FormType,
                        async (dt) =>
                        {
                            await report.Root.DirCustom(FFormType.Edit, FData.NewItem(dt.Rows[0], dir.Settings.Fields), data, () => FFunc.CatchScriptMethod(dir, ds));
                        }, check: false);
                }
                MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> NewRecordGrid(object obj)
        {
            var toolbar = obj as FToolbar;
            await report.Root.SetBusy(true);
            var check = await FCommand.DirLoading(report.Settings.Code, FData.NewItem(null, report.Settings.Code), toolbar, report.Controller, FAction.New);
            if (check.Success == 1)
            {
                var ds = check.ToDataSet();
                return await TryCatchMessage(dir, report.Root, ds, toolbar.FormType,
                    async (dt) =>
                    {
                        await report.Root.DirCustom(FFormType.New, dt.Rows.Count > 0 ? FData.NewItem(dt.Rows[0], dir.Settings.Fields) : null, null, () => FFunc.CatchScriptMethod(dir, ds));
                    }, check: false);
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            await report.Root.SetBusy(false);
            return new FCommnadValue();
        }

        public virtual async Task<FCommnadValue> DeleteRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var mess = await FCommand.Deleting(report.Settings.Code, data, toolbar, report.Controller);
                if (mess.Success == 1)
                {
                    return await TryCatchMessage(dir, report.Root, mess.ToDataSet(), toolbar.FormType,
                    (dt) =>
                    {
                        var ds = report.CacheDeviceID.GetCache().ToDataSet();
                        report.DeleteItem(ref ds, data.GetIndex());
                        ds.Encode().SetCache(report.CacheDeviceID);
                        return Task.CompletedTask;
                    });
                }
                MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            }, "805");
        }

        public virtual async Task<FCommnadValue> SaveRecordGrid(FFormType type)
        {
            await report.Root.SetBusy(true);
            var mess = type == FFormType.New ? await FCommand.Inserting(report.Root.Dir, report.Controller) : await FCommand.Updating(report.Root.Dir, report.Controller);
            if (mess.Success == 1)
            {
                return await TryCatchMessage(dir, report.Root, mess.ToDataSet(), 1,
                    (dt) =>
                    {
                        if (type == FFormType.New) report.AddItem(dt.Rows[0]);
                        else
                        {
                            var ds = report.CacheDeviceID.GetCache().ToDataSet();
                            report.EditItem(ref ds, dir.GridData.GetIndex(), dt.Rows[0]);
                            ds.Encode().SetCache(report.CacheDeviceID);
                            report.GridView.Refresh();
                            report.ListView.RefreshView();
                        }
                        return report.Navigation.PopAsync();
                    }
                );
            }
            MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
            await report.Root.SetBusy(false);
            return new FCommnadValue();
        }

        public virtual async Task<FCommnadValue> PrintRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                await Task.Delay(100);
                try
                {
                    var link = report.ExtData["lookup_link"].ToString();
                    link = link.Replace("#type#", "3");
                    link = link.Replace("#keys#", data["so_bi_mat"].ToString());
                    link = link.Replace("#hash#", data["hash_value"].ToString());
                    await new FPagePDF(link).Show(report.Root);
                    await report.Root.SetBusy(false);
                    return new FCommnadValue();
                }
                catch (Exception ex)
                {
                    MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                    await report.Root.SetBusy(false);
                    return new FCommnadValue();
                }
            });
        }

        public virtual async Task<FCommnadValue> PrintVoucherRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var mess = await FCommand.Printing(report.Settings, data, toolbar, report.Controller);
                if (mess.Success == 1)
                {
                    return await TryCatchMessage(report.Root, report.Root, mess.ToDataSet(), toolbar.FormType,
                    async (dt) =>
                    {
                        if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("link"))
                            await new FPagePDF(dt.Rows[0]["link"].ToString()).Show(report.Root);
                    });
                }
                MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> XmlDownLoadRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                await Task.Delay(100);
                try
                {
                    if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
                    {
                        FPermissions.ShowMessage();
                        await report.Root.SetBusy(false);
                        return new FCommnadValue();
                    }
                    var link = report.ExtData["lookup_link"].ToString();
                    link = link.Replace("#type#", "2");
                    link = link.Replace("#keys#", HttpUtility.UrlEncode(data["so_bi_mat"].ToString()));
                    link = link.Replace("#hash#", HttpUtility.UrlEncode(data["hash_value"].ToString()));

                    FInterface.IFDownload.OnFileDownloaded += FUtility.DownloadCompleted;
                    await FInterface.IFDownload.DownloadFileWithText(link, $"{DateTime.Now:yyyyMMddHHmmss}.xml");
                    FInterface.IFDownload.OnFileDownloaded -= FUtility.DownloadCompleted;
                    await report.Root.SetBusy(false);
                    return new FCommnadValue();
                }
                catch (Exception ex)
                {
                    MessagingCenter.Send(new FMessage(0, 310, ex.Message), FChannel.ALERT_BY_MESSAGE);
                    await report.Root.SetBusy(false);
                    return new FCommnadValue();
                }
            });
        }

        public virtual async Task<FCommnadValue> ReleaseRecordGrid(object obj, FData data)
        {
            var se = report.GetSelectedItems();
            var xl = report.Details["xu_ly"].ToString();
            var vc = report.DataFilter["ma_hd"].ToString().Trim();

            if (xl.Equals("9")) return new FCommnadValue();
            if (CheckNullSelected(se)) return new FCommnadValue();
            if (se.Count > 1 & (xl.Equals("2") || xl.Equals("3")))
            {
                MessagingCenter.Send(new FMessage(0, xl.Equals("2") ? 146 : 147, ""), FChannel.ALERT_BY_MESSAGE);
                return new FCommnadValue();
            }
            if (string.IsNullOrEmpty(vc))
            {
                if (xl.Equals("1") && !await FAlertHelper.Confirm("808")) return new FCommnadValue();
                else if (xl.Equals("2") && !await FAlertHelper.Confirm("809")) return new FCommnadValue();
            }
            else if (!xl.Equals("3") && !await FAlertHelper.Confirm("812", $"{report.DataFilter["ky_hieu"]}{(char)254}{report.DataFilter["so_seri"]}")) return new FCommnadValue();
            return await BaseRecordGrid(obj as FToolbar, xl.Equals("1") ? data : se[0], async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var check = await FInvoice.Release(report.Details, report.DataFilter, se, formData);
                if (check)
                {
                    await report.Root.Grid.LoadingGrid(report.TargetType);
                    return new FCommnadValue(null, true);
                }
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> CancelRecordGrid(object obj, FData data)
        {
            var se = report.GetSelectedItems();
            var xl = report.Details["xu_ly"].ToString();

            if (!xl.Equals("9")) return new FCommnadValue();
            if (CheckNullSelected(se)) return new FCommnadValue();
            if (se.Count > 1)
            {
                MessagingCenter.Send(new FMessage(0, 145, ""), FChannel.ALERT_BY_MESSAGE);
                return new FCommnadValue();
            }
            if (!await FAlertHelper.Confirm("811")) return new FCommnadValue();
            return await BaseRecordGrid(obj as FToolbar, se[0], async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var check = await FInvoice.Cancel(se[0], report.Details, report.DataFilter, formData);
                if (check)
                {
                    var ds = report.CacheDeviceID.GetCache().ToDataSet();
                    report.DeleteItem(ref ds, se[0].GetIndex());
                    ds.Encode().SetCache(report.CacheDeviceID);
                    await report.Root.SetBusy(false);
                    return new FCommnadValue(null, true);
                }
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> CustomRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var fieldRequest = new List<FField>();
                report.Settings.Fields.ForEach(f => { if (report.Settings.Code.Contains(f) || toolbar.Fields.Contains(f.Name)) fieldRequest.Add(f); });
                var check = await FCommand.SelectedCommand(report.Settings.Fields, data, null, null, "Grid", toolbar.Command, report.Controller, toolbar.Command, toolbar.CommandSuccess, toolbar.CommandArgument, null);
                if (check.Success == 1)
                    return await TryCatchMessage(report.Root, report.Root, check.ToDataSet(), toolbar.FormType, (dt) => { return Task.CompletedTask; });
                MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> CustomRecordOther(object obj, FData data)
        {
            var fieldRequest = new List<FField>();
            if (report.Root.Filter != null)
            {
                fieldRequest = report.Root.Filter.Settings.Fields;
                data = report.DataFilter;
            }
            else
            {
                fieldRequest = report.Settings.Details;
                data = report.Details;
            }
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var check = await FCommand.SelectedCommand(fieldRequest, data, null, null, "Grid", toolbar.Command, report.Controller, toolbar.Command, toolbar.CommandSuccess, toolbar.CommandArgument, null);
                if (check.Success == 1)
                    return await TryCatchMessage(report.Root, report.Root, check.ToDataSet(), toolbar.FormType, (dt) => { return Task.CompletedTask; });
                MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> DownloadAttachment(object obj, FData data)
        {
            var toolbar = obj as FToolbar;
            var table = toolbar.Data == null ? null : toolbar.Data as DataTable;
            await report.Root.SetBusy(true);
            if (!await FPermissions.HasPermission<Permissions.StorageWrite>(true))
            {
                FPermissions.ShowMessage();
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            }
            var check = await FCommand.SelectedCommand(table, null, null, "Grid", "Attachment", report.Controller, "Download", null, toolbar.Command, null);
            if (check.Success == 1)
            {
                return await TryCatchMessage(report.Root, report.Root, check.ToDataSet(), toolbar.FormType, async (dt) =>
                {
                    if (dt == null || dt.Rows.Count == 0) return;
                    var row = dt.Rows[0];
                    var controler = row["controller"].ToString().Trim();
                    var filename = row["file_name"].ToString().Trim();
                    var fileEncrypt = row["file_enc"].ToString().Trim();
                    var lineNbr = row["line_nbr"].ToString().Trim();
                    var sysKey = row["sysKey"].ToString().Trim();
                    var ticket = row["ticket"].ToString().Trim();

                    FInterface.IFDownload.OnFileDownloaded += FUtility.DownloadCompleted;
                    await FInterface.IFDownload.DownloadFile(FServices.DownloadUrl(controler, report.Root.BeforePage?.Controller, fileEncrypt, sysKey, lineNbr, ticket), filename);
                    FInterface.IFDownload.OnFileDownloaded -= FUtility.DownloadCompleted;
                });
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            await report.Root.SetBusy(false);
            return new FCommnadValue();
        }

        public virtual async Task<FCommnadValue> CommandRecordGrid(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                var check = await FCommand.Command(report.Settings, data, toolbar, report.Controller);
                if (check.Success == 1)
                {
                    return await TryCatchMessage(report.Root, report.Root, check.ToDataSet(), toolbar.FormType, (dt) => { return Task.CompletedTask; });
                }
                MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
                await report.Root.SetBusy(false);
                return new FCommnadValue();
            });
        }

        public virtual async Task<FCommnadValue> AcceptApproval(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                report.Approval.Executing += report.OnClearCacheComment;
                var result = await report.Approval.Execute("1");
                report.Approval.Executing -= report.OnClearCacheComment;
                await report.Root.SetBusy(false);
                return new FCommnadValue(null, result);
            }, "905");
        }

        public virtual async Task<FCommnadValue> CancelApproval(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                report.Approval.Executing += report.OnClearCacheComment;
                var result = await report.Approval.Execute("2");
                report.Approval.Executing -= report.OnClearCacheComment;
                await report.Root.SetBusy(false);
                return new FCommnadValue(null, result);
            }, "906");
        }

        public virtual async Task<FCommnadValue> UndoApproval(object obj, FData data)
        {
            return await BaseRecordGrid(obj as FToolbar, data, async (toolbar, formData) =>
            {
                await report.Root.SetBusy(true);
                report.Approval.Executing += report.OnClearCacheComment;
                var result = await report.Approval.Execute("0");
                report.Approval.Executing -= report.OnClearCacheComment;
                await report.Root.SetBusy(false);
                return new FCommnadValue(null, result);
            }, "904");
        }

        #region private

        private async Task<FCommnadValue> BaseRecordGrid(FToolbar toolbar, FData data, Func<FToolbar, FData, Task<FCommnadValue>> task, string code = "")
        {
            report.GridView.SelectedIndex = data != null ? data.GetIndex() : -1;
            if (!string.IsNullOrEmpty(toolbar.Check) && !await CheckCondition(data, report.Details, toolbar.Check)) return new FCommnadValue();

            if (!string.IsNullOrEmpty(code))
            {
                if (!await FAlertHelper.Confirm(code)) return new FCommnadValue();
                else await Task.Delay(150);
            }

            if (!string.IsNullOrEmpty(toolbar.Bio) && !await Bio(await CheckCondition(data, report.Details, toolbar.Bio), toolbar.BioNative)) return new FCommnadValue();

            if (!string.IsNullOrEmpty(toolbar.BioPassword))
            {
                var (OK, Secret) = await BioPassword(await CheckCondition(data, report.Details, toolbar.BioPassword), toolbar.BioNative, !string.IsNullOrEmpty(toolbar.BioPassword) && await CheckCondition(data, report.Details, toolbar.BioPassword));
                if (!OK) return new FCommnadValue();
                data[BioRequestField, FieldType.String] = Secret;
            }

            toolbar.CommandArgument = toolbar.MenuItem.Count > 0 ? await new FAlertOptions().ShowOptions(toolbar.Title, FText.Accept, FText.Cancel, toolbar.GetMenuItem, FItem.ItemID, FItem.ItemValue) : null;
            if (toolbar.CommandArgument == "") return new FCommnadValue();
            if (!string.IsNullOrEmpty(toolbar.CommandSuccessScript) && !string.IsNullOrEmpty(toolbar.CommandArgument))
            {
                toolbar.CommandSuccess = toolbar.CommandSuccessScript.Replace("[commandArgument]", toolbar.CommandArgument);
                toolbar.CommandSuccess = (string)FFunc.Compute(report.Root, toolbar.CommandSuccessScript.Replace("[commandArgument]", toolbar.CommandArgument));
            }
            else toolbar.CommandSuccess = null;
            if (!string.IsNullOrEmpty(toolbar.ShowForm))
            {
                await ShowForm(toolbar, data, task);
                return new FCommnadValue();
            }
            return await task.Invoke(toolbar, null);
        }

        private async Task ShowForm(FToolbar toolbar, FData data, Func<FToolbar, FData, Task<FCommnadValue>> task)
        {
            var shForm = toolbar.ShowForm;
            if (FFunc.IsBinding(shForm))
            {
                shForm = report.Details?[FFunc.ReplaceBinding(shForm)]?.ToString().Trim();
                if (string.IsNullOrEmpty(shForm))
                {
                    await task.Invoke(toolbar, data);
                    return;
                }
            }
            var form = new FPageFilter(shForm)
            {
                Target = FFormTarget.Filter,
                FormType = FFormType.Filter,
                Root = report.Root,
                GridData = data
            };
            form.CancelClick += async (s, e) => { await report.Navigation.PopAsync(); };
            form.BackButtonClicked += async (s, e) => { await report.Navigation.PopAsync(); };
            form.InputView.Content = null;
            switch (toolbar.FormType)
            {
                case 1:
                    await form.SetBusy(true);
                    await report.Navigation.PushAsync(form, true);
                    await form.InitByController();
                    await form.SetBusy(false);
                    form.OkClick += async (s, e) => { await report.Navigation.PopAsync(); await task.Invoke(toolbar, form.FDataDirForm()); };
                    break;

                case 2:
                    var check = await task.Invoke(toolbar, null);
                    if (check.Result) break;
                    await form.InitByController();
                    if (check.Table == null || check.Table.Rows.Count == 0) break;
                    form.InputData = FData.NewItem(check.Table.Rows[0], form.Settings.Fields);
                    form.FillAll(false);
                    await report.Navigation.PushAsync(form, true);
                    form.OkClick += async (s, e) => await form.SetBusy(false);
                    break;

                default:
                    return;
            }
        }

        private async Task<bool> CheckCondition(FData data, FData detail, string check)
        {
            foreach (Match m in new Regex($"detail\\[(.+?)\\]").Matches(check))
                if (report.Settings.Details.Count > 0 && detail.CheckName(m.Groups[1].Value))
                    check = check.Replace($"detail[{m.Groups[1].Value}]", $"'{detail[m.Groups[1].Value]}'");
            foreach (Match m in new Regex($"\\[(.+?)\\]").Matches(check))
                if (data.CheckName(m.Groups[1].Value))
                    check = check.Replace($"[{m.Groups[1].Value}]", $"'{data[m.Groups[1].Value]}'");
            var result = await FFunc.ComputeAsync(null, check);
            return result != null && (bool)result;
        }

        private async Task<bool> Bio(bool bio, bool bioNative, bool alert = true)
        {
            if (!bio) return true;

            if (!FSetting.UseLocalAuthen)
            {
                if (alert) MessagingCenter.Send(FMessage.FromFail(1251, FUtility.TextFingerType(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V)), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            bioNative = !FSetting.IsAndroid && bioNative;
            if (!FInterface.IFFingerprint.IsAvailable(bioNative)) return true;

            var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FText.AttributeString(FSetting.SystemLanguage, "Cancel"));
            config.AllowAlternativeAuthentication = bioNative;
            var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

            if (authResult.Status == FFingerprintAuthenticationResultStatus.TooManyAttempts)
            {
                if (alert) MessagingCenter.Send(FMessage.FromFail(1254), FChannel.ALERT_BY_MESSAGE);
                return false;
            }

            if (authResult.Status == FFingerprintAuthenticationResultStatus.Canceled)
                return false;

            if (!authResult.IsAuthenticated)
            {
                if (alert) MessagingCenter.Send(FMessage.FromFail(1252), FChannel.ALERT_BY_MESSAGE);
                return false;
            }
            return true;
        }

        private async Task<(bool OK, string Secret)> BioPassword(bool bioPassword, bool bioNative, bool alert = true)
        {
            if (!bioPassword) return (true, string.Empty);

            if (!FSetting.UseLocalAuthen)
                return await FServices.ConfirmPassword(FText.ConfirmPassword, alert);

            bioNative = !FSetting.IsAndroid && bioNative;
            if (!FInterface.IFFingerprint.IsAvailable(bioNative))
                return await FServices.ConfirmPassword(FText.ConfirmPassword, alert);

            var config = new FAuthenticationRequestConfiguration(FText.ApplicationTitle, FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.SystemLanguageIsV), FText.AttributeString(FSetting.SystemLanguage, "Cancel"));
            config.AllowAlternativeAuthentication = bioNative;
            var authResult = await FInterface.IFFingerprint.AuthenticateAsync(config);

            if (authResult.IsAuthenticated)
                return (true, FString.Password);

            //if (authResult.Status == FFingerprintAuthenticationResultStatus.Canceled)
            //    return (false, string.Empty);

            return await FServices.ConfirmPassword(FUtility.TextFinger(FInterface.IFFingerprint.GetAuthenticationType(), FSetting.V) + FText.AuthenConfirmAppend, alert);
        }

        private bool CheckNullSelected(List<FData> selected)
        {
            if (selected == null || selected.Count == 0)
            {
                MessagingCenter.Send(new FMessage(0, 804, ""), FChannel.ALERT_BY_MESSAGE);
                return true;
            }
            return false;
        }

        private static async Task<FCommnadValue> InvokeActions(DataSet ds, DataTable detail, DataTable data, Func<DataTable, Task> success)
        {
            await success.Invoke(data);
            if (detail.Columns.Contains("warning")) MessagingCenter.Send(new FMessage(0, 0, detail.Rows[0]["warning"].ToString()), FChannel.ALERT_BY_MESSAGE);
            var invoke = await new FInvoke().InvokeMethod(ds);
            if (invoke.Success == 0) MessagingCenter.Send(invoke, FChannel.ALERT_BY_MESSAGE);
            return new FCommnadValue(data, true);
        }

        #endregion private
    }
}