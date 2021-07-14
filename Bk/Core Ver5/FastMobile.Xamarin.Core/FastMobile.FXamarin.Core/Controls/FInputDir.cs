using Newtonsoft.Json.Linq;
using Syncfusion.SfDataGrid.XForms;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FInputDir : FInputGrid
    {
        public override string Target => "Dir";

        public override FInputGridValue Value
        {
            get
            {
                return new FInputGridValue(null, IsEdited);
            }
        }

        public FInputDir(string controller, JObject setting) : base(controller, setting)
        {
            Detail.Target = FFormTarget.Dir;
        }

        public FInputDir(FField field) : base(field)
        {
            Detail.Target = FFormTarget.Dir;
        }

        public override bool IsEqual(object oldValue)
        {
            return true;
        }

        #region Protected

        protected override async Task<bool> NewCommand(FData data)
        {
            var toolbar = new FToolbar { Command = "New", FormType = 1 };
            var check = await FCommand.DirScattering(Settings.Scatter, data, toolbar.Command, Controller);
            if (check.Success == 1)
            {
                var mess = await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType,
                    (dt) =>
                    {
                        Detail.FormType = FFormType.New;
                        Detail.InputData = dt.Rows.Count > 0 ? FData.NewItem(dt.Rows[0], Settings.Fields) : null;
                        Detail.ClearAll();
                        Detail.FillAll(false);
                        return Task.CompletedTask;
                    });
                return mess.Result;
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            return false;
        }

        protected override async Task<bool> EditCommand(FData data)
        {
            var toolbar = new FToolbar { Command = "New", FormType = 1 };
            var check = await FCommand.DirScattering(Settings.Scatter, data, toolbar.Command, Controller);
            if (check.Success == 1)
            {
                var mess = await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType,
                    (dt) =>
                    {
                        Detail.FormType = FFormType.Edit;
                        return Task.CompletedTask;
                    });
                return mess.Result;
            }
            MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            return false;
        }

        protected override async Task OnNewItem(object obj)
        {
            var toolbar = obj as FToolbar;
            var check = await FCommand.DirLoading(Settings.Code, FData.NewItem(null, Settings.Code), toolbar, Controller, FAction.New);
            if (check.Success == 1)
            {
                await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType,
                    async (dt) =>
                    {
                        FData fdata = dt.Rows.Count > 0 ? FData.NewItem(dt.Rows[0], Settings.Fields) : null;
                        if (await InitDirForm(FFormType.New, fdata) is Page detail) await Navigation.PushAsync(detail, true);
                    });
            }
            else MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
        }

        protected override async Task OnEditItem(object obj, FData data)
        {
            var toolbar = obj as FToolbar;
            var check = await FCommand.DirLoading(Settings.Code, data, toolbar, Controller, FAction.Edit);
            if (check.Success == 1)
            {
                await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType,
                    async (dt) =>
                    {
                        FData fdata = FData.NewItem(dt.Rows[0], Settings.Fields);
                        if (await InitDirForm(FFormType.Edit, fdata) is Page detail) await Navigation.PushAsync(detail, true);
                    });
            }
            else MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
        }

        protected override async Task OnDeleteItem(object obj, FData data)
        {
            await Root.SetBusy(true);
            Grid.SelectedIndex = data.GetIndex();
            var toolbar = obj as FToolbar;
            if (IsModifier && (Settings.ClearMode || await FAlertHelper.Confirm("805")))
            {
                await Task.Delay(100);
                var check = await FCommand.Deleting(Settings.Code, data, toolbar, Controller);
                if (check.Success == 1)
                    await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType, async (dt) => { await DeleteItem(); });
                else
                    MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
            }
            await Root.SetBusy(false);
        }

        protected override async Task OnViewItem(object sender, GridTappedEventArgs e)
        {
            var toolbar = new FToolbar { Command = "View", FormType = 1 };
            Grid.SelectedIndex = e.RowColumnIndex.RowIndex;
            var check = await FCommand.DirLoading(Settings.Code, e.RowData as FData, toolbar, Controller, FAction.View);
            if (check.Success == 1)
            {
                await FReportToolbar.TryCatchMessage(Root, Root.Page, check.ToDataSet(), toolbar.FormType,
                    async (dt) =>
                    {
                        FData fdata = FData.NewItem(dt.Rows[0], Settings.Fields);
                        if (await InitDirForm(IsModifier ? FFormType.View : FFormType.ViewDetail, fdata) is Page detail) await Navigation.PushAsync(detail, true);
                    });
            }
            else MessagingCenter.Send(check, FChannel.ALERT_BY_MESSAGE);
        }

        protected override async Task OnSaveItem(FFormType type, FData data)
        {
            var mess = type == FFormType.New ? await FCommand.Inserting(Detail, Controller) : await FCommand.Updating(Detail, Controller);
            if (mess.Success == 1)
            {
                await FReportToolbar.TryCatchMessage(Root, Root.Page, mess.ToDataSet(), 1,
                    async (dt) =>
                    {
                        FData fdata = FData.NewItem(dt.Rows[0], Settings.Fields);
                        await base.OnSaveItem(type, fdata);
                    }
                );
            }
            else MessagingCenter.Send(mess, FChannel.ALERT_BY_MESSAGE);
        }

        protected override async Task<FMessage> Loading()
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", Target);
            FFunc.AddFieldInput(ref ds, new DataTable(), Settings.Code, Root.InputData);
            FMessage result = await FServices.ExecuteCommand("Query", Controller, ds, "300", null, true);
            ds.Dispose();
            return result;
        }

        #endregion Protected
    }
}