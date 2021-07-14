using Syncfusion.ListView.XForms;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using DataRow = System.Data.DataRow;

namespace FastMobile.FXamarin.Core
{
    public interface IFReportSettings
    {
        FTargetType TargetType { get; set; }
        string Controller { get; set; }
        string CacheDeviceID { get; }
        string SettingsDeviceID { get; }
        string TotalRecordName { get; }
        GridType GridType { get; set; }
        FDataObservation Source { get; set; }
        DataRow[] DataCache { get; }
        FData Details { get; set; }
        SfListView ListView { get; set; }
        SfDataGrid GridView { get; set; }
        View WebView { get; set; }
        int TotalItem { get; set; }
        int LastPage { get; set; }
        int LastIndex { get; set; }
        string FirstItem { get; }
        string LastItem { get; }
        int DataIndex { get; set; }
        int DetailIndex { get; set; }
        int ExtendIndex { get; set; }
        public string Html { get; set; }
        bool IsBusyGrid { get; set; }
        FData DataFilter { get; set; }
        Dictionary<string, object> ExtData { get; set; }
        DataTable ExtenderData { get; set; }
        FPageReport Root { get; set; }
        FViewPage Settings { get; set; }
        INavigation Navigation { get; }
        IFCommand Command { get; set; }
        public IFApproval Approval { get; set; }
        string CommentText { get; }

        void DeleteItem(ref DataSet ds, int index);

        void EditItem(ref DataSet ds, int index, DataRow dr);

        void AddItem(DataRow dr);

        void SelectItem(ref DataSet ds, int index, bool isChanged);

        void OpenDetail(DataTable table);

        List<FData> GetSelectedItems();

        Task<bool> Next(bool isDelete);

        Task<bool> Back(bool isDelete);

        void OnClearCacheComment(object sender, EventArgs e);
    }
}