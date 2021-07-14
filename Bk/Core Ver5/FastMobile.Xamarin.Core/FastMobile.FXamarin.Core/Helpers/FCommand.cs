using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public class FCommand : IFCommand
    {
        private readonly IFReportSettings report;
        private FPageFilter Filter => report.Root.Filter;

        public FCommand(IFReportSettings report)
        {
            this.report = report;
        }

        private void AddMemvar(ref DataTable dt, IFPaging paging)
        {
            var typeInt = typeof(int);
            dt.AddRowValue(0, "refresh", "1", typeInt).AddRowValue(0, "pageIndex", paging.PageIndex - 1, typeInt)
              .AddRowValue(0, "pageCount", paging.ItemPerPage, typeInt).AddRowValue(0, "lastPage", report.LastPage, typeInt)
              .AddRowValue(0, "lastCount", paging.TotalItem, typeInt).AddRowValue(0, "firstItem", report.FirstItem)
              .AddRowValue(0, "lastItem", report.LastItem);
        }

        #region Static

        public static async Task<FMessage> SelectedCommand(List<FField> fields, FData newValue, List<FField> oldfields, FData oldValue, string target, string command, string controller, string action, string successAction, string commandArgument, FExtensionParam extension)
        {
            var ds = new DataSet();
            var ext = new List<DataTable>();

            FFunc.AddTypeAction(ref ds, "command", target, action, successAction, false, commandArgument);
            if (newValue != null) ext = FFunc.AddFieldInput(ref ds, new DataTable(), fields, newValue, true);
            if (oldValue != null) FFunc.AddFieldInput(ref ds, new DataTable(), oldfields, oldValue, false);
            ext.ForEach(e => { ds.Tables.Add(e.Copy()); e.Dispose(); });
            FMessage result = await FServices.ExecuteCommand(command, controller, ds, "300", extension, true);
            ds.Dispose();
            return result;
        }

        public static async Task<FMessage> SelectedCommand(DataTable newData, DataTable oldData, List<DataTable> extend, string target, string command, string controller, string action, string successAction, string commandArgument, FExtensionParam extension)
        {
            var ds = new DataSet();
            FFunc.AddTypeAction(ref ds, "command", target, action, successAction, false, commandArgument);
            if (newData != null) { newData.TableName = "Table2"; ds.Tables.Add(newData.Copy()); };
            if (oldData != null) { oldData.TableName = "Table3"; ds.Tables.Add(oldData.Copy()); };
            extend?.ForEach(e => { ds.Tables.Add(e.Copy()); e.Dispose(); });
            FMessage result = await FServices.ExecuteCommand(command, controller, ds, "300", extension, true);
            newData?.Dispose();
            oldData?.Dispose();
            ds.Dispose();
            return result;
        }

        public static Task<FMessage> DirLoading(List<FField> code, FData selected, FToolbar toolbar, string controller, FAction type)
        {
            return SelectedCommand(code, selected, null, null, "Dir", "Loading", controller, toolbar.Command, null, toolbar.CommandArgument, FExtensionParam.New(true, controller, type));
        }

        public static Task<FMessage> DirScattering(List<FField> scatter, FData selected, string action, string controller)
        {
            return SelectedCommand(scatter, selected, null, null, "Dir", "Scattering", controller, action, null, null, FExtensionParam.New(true, controller, action.Equals("New") ? FAction.New : FAction.Edit));
        }

        public static Task<FMessage> Inserting(FPageFilter dir, string controller)
        {
            return SelectedCommand(dir.Settings.Fields, dir.FDataDirForm(), null, null, "Dir", "Inserting", controller, "New", "Inserted", null, null);
        }

        public static Task<FMessage> Updating(FPageFilter dir, string cotroller)
        {
            var fieldRequest = new List<FField>();
            dir.Settings.Fields.ForEach(f => { if (dir.Settings.Code.Contains(f) || f.AllowRequest) fieldRequest.Add(f); });
            return SelectedCommand(dir.Settings.Fields, dir.FDataDirForm(), fieldRequest, dir.OldData, "Dir", "Updating", cotroller, "Edit", "Updated", null, null);
        }

        public static Task<FMessage> Deleting(List<FField> code, FData selected, FToolbar toolbar, string controller)
        {
            return SelectedCommand(code, selected, null, null, "Dir", "Deleting", controller, "Delete", "Deleted", toolbar.CommandArgument, FExtensionParam.New(true, controller, FAction.Delete));
        }

        public static Task<FMessage> Showing(List<FField> fields, FData selected, string controller)
        {
            return SelectedCommand(fields, selected, null, null, "Grid", "Showing", controller, null, null, null, null);
        }

        public static Task<FMessage> Printing(FViewPage settings, FData selected, FToolbar toolbar, string controller)
        {
            var fieldRequest = new List<FField>();
            settings.Fields.ForEach(f => { if (settings.Code.Contains(f) || toolbar.Fields.Contains(f.Name)) fieldRequest.Add(f); });
            return SelectedCommand(fieldRequest, selected, null, null, "Grid", "Printing", controller, toolbar.Command, toolbar.CommandSuccess, toolbar.CommandArgument, null);
        }

        public static Task<FMessage> Command(FViewPage settings, FData selected, FToolbar toolbar, string controller)
        {
            var fieldRequest = new List<FField>();
            settings.Fields.ForEach(f => { if (settings.Code.Contains(f) || toolbar.Fields.Contains(f.Name)) fieldRequest.Add(f); });
            return SelectedCommand(fieldRequest, selected, null, null, "Grid", "Command", controller, toolbar.Command, toolbar.CommandSuccess, toolbar.CommandArgument, null);
        }

        #endregion Static

        #region FInputBase

        public virtual async Task<FMessage> Prossessing(IFPaging paging, bool isLog)
        {
            var ds = new DataSet();
            var dt = new DataTable();
            var fs = Filter.Settings;
            var gs = report.Settings;
            var fc = fs.ReportCacheType;
            var gc = gs.ReportCacheType;

            FFunc.AddTypeAction(ref ds, "command", "Filter");
            if (fc == ReportCacheType.None || fc == ReportCacheType.Dynamic && (gc == ReportCacheType.Dynamic || gc == ReportCacheType.None)) AddMemvar(ref dt, paging);
            if (gs.IsSearchBar) dt.AddRowValue(0, "searchText", report.Root.SearchText);
            FFunc.AddFieldInput(ref ds, dt, fs.Fields, report.DataFilter);
            FMessage result = await FServices.ExecuteCommand("Processing", report.Controller, ds, "300", FExtensionParam.New(isLog, report.Controller, FAction.Filter), true);
            if (result.Success == 1)
            {
                report.DataIndex = fs.TableData;
                report.DetailIndex = fs.TableDetails;
                report.ExtendIndex = fs.TableExtend;
            }
            ds.Dispose();
            return result;
        }

        public virtual async Task<FMessage> Searching(IFPaging paging, bool isLog)
        {
            var ds = new DataSet();
            var dt = new DataTable();
            var fs = Filter?.Settings;
            var ca = report.Settings.ReportCacheType;

            FFunc.AddTypeAction(ref ds, "command", "Grid");
            if (ca == ReportCacheType.None || ca == ReportCacheType.Dynamic) AddMemvar(ref dt, paging);
            dt.AddRowValue(0, "searchText", report.Root.SearchText);
            if (fs == null) ds.Tables.Add(dt);
            else
            {
                if (Filter.InputView.Content == null) await Filter.InitBySetting();
                FData data = report.DataFilter ?? Filter.FDataDirForm();
                FFunc.AddFieldInput(ref ds, dt, fs.Fields, data);
            }
            FMessage result = await FServices.ExecuteCommand("Searching", report.Controller, ds, "300", FExtensionParam.New(isLog, report.Controller, FAction.Search), true);
            if (result.Success == 1)
            {
                report.DataIndex = report.Settings.SearchData;
                report.DetailIndex = report.Settings.SearchDetail;
                report.ExtendIndex = report.Settings.TableExtend;
            }
            ds.Dispose();
            return result;
        }

        public virtual async Task<FMessage> Loading(IFPaging paging, bool isLog)
        {
            var ds = new DataSet();
            var cache = report.Settings.ReportCacheType;
            FFunc.AddTypeAction(ref ds, "command", "Grid");
            if (cache == ReportCacheType.None || cache == ReportCacheType.Dynamic)
            {
                var dt = new DataTable();
                AddMemvar(ref dt, paging);
                ds.Tables.Add(dt.Copy());
                dt.Dispose();
            }
            FMessage result = await FServices.ExecuteCommand("Loading", report.Controller, ds, "300", FExtensionParam.New(isLog, report.Controller, FAction.Load), true);
            if (result.Success == 1)
            {
                report.DataIndex = report.Settings.TableData;
                report.DetailIndex = report.Settings.TableDetails;
                report.ExtendIndex = report.Settings.TableExtend;
            }
            ds.Dispose();
            return result;
        }

        public virtual async Task<FMessage> GetDataSet(IFPaging paging, bool isLog)
        {
            return report.Settings.Type switch
            {
                "Report" => await Prossessing(paging, isLog),
                "Approval" => await Loading(paging, isLog),
                "Voucher" => await Prossessing(paging, isLog),
                "" => await Loading(paging, isLog),
                _ => new FMessage(1, 100, "")
            };
        }

        #endregion FInputBase
    }
}