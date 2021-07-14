using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public class FViewPage
    {
        public static FCacheArray ReportReference { get; }

        public List<FField> Fields { get; set; }
        public List<FField> Details { get; set; }
        public List<FViews> Views { get; set; }
        public List<FToolbar> Toolbars { get; set; }
        public List<FScript> Scripts { get; set; }
        public ReportType ReportType { get; set; }
        public ReportCacheType ReportCacheType { get; set; }
        public int TableData { get; set; }
        public int TableDetails { get; set; }
        public int TableExtend { get; set; }
        public int Freeze { get; set; }
        public int PageItems { get; set; }
        public List<FField> Code { get; set; }
        public List<FField> Scatter { get; set; }
        public string Filter { get; set; }
        public string Title { get; set; }
        public FViewPage Tips { get; set; }
        public string SubTitle { get; set; }
        public string Type { get; set; }
        public bool HasCheck { get; set; }
        public bool HasInit { get; set; }
        public bool SelectMode { get; set; }
        public bool ClearMode { get; set; }
        public bool CopyMode { get; set; }
        public bool ExpandCache { get; set; }
        public bool IsSearchBar { get; set; }
        public string PlacHolderSearchBar { get; set; }
        public int SearchData { get; set; }
        public int SearchDetail { get; set; }
        public ReportCacheType SearchCache { get; set; }
        public string[] Order { get; set; }
        public List<string> Reference { get; set; }

        static FViewPage()
        {
            ReportReference = new FCacheArray("FastMobile.FXamarin.Core.FViewPage.ReportReferenceID");
        }

        public FViewPage(string id, JObject settings)
        {
            Base(settings);
            SaveSetting(id, settings);
        }

        public FViewPage(JObject settings)
        {
            Base(settings);
        }

        static public async Task<JObject> InitSettingsFromDevice(string id)
        {
            if (!ReportReference.Contains(id)) return null;
            var dt = id.GetCache();
            if (string.IsNullOrEmpty(dt)) return null;
            var st = JObject.Parse(dt);
            if (st.Type == JTokenType.Null) return null;
            return await Task.FromResult(st);
        }

        static public async Task<DataSet> InitDataFromDevice(string id)
        {
            if (!ReportReference.Contains(id)) return null;
            var dt = id.GetCache();
            if (string.IsNullOrEmpty(dt)) return null;
            return await Task.FromResult(dt.ToDataSet());
        }

        static public void ClearAll()
        {
            ReportReference.Clear();
        }

        static public void SaveSetting(string id, JObject settings)
        {
            settings.SetCache(id);
            ReportReference.Add(id);
        }

        static public void SaveDataSet(string id, string data)
        {
            data.SetCache(id);
            ReportReference.Add(id);
        }

        private void Base(JObject settings)
        {
            JToken f = settings["Field"], d = settings["Detail"], v = settings["Layout"], t = settings["Toolbar"], s = settings["Script"], p = settings["Tips"];
            var c = FFunc.GetArrayString(FFunc.GetStringValue(settings, "Code")).ToArray();
            var e = FFunc.GetArrayString(FFunc.GetStringValue(settings, "Scatter")).ToArray();

            Fields = FFunc<FField>.GetFListObject((JArray)f);
            Details = FFunc<FField>.GetFListObject((JArray)d);
            Views = FFunc<FViews>.GetFListObject((JArray)v);
            Toolbars = FFunc<FToolbar>.GetFListObject((JArray)t);
            Scripts = FFunc<FScript>.GetFListObject((JArray)s);
            ReportType = FFunc.GetBooleanValue(settings, "LoadMore") ? ReportType.Loadmore : ReportType.Pagging;
            ReportCacheType = FFunc.GetStringValue(settings, "Cache") switch { "None" => ReportCacheType.None, "IIS" => ReportCacheType.IIS, "Device" => ReportCacheType.Device, _ => ReportCacheType.Dynamic };
            HasCheck = FFunc.GetBooleanValue(settings, "HasCheck");
            HasInit = FFunc.GetBooleanValue(settings, "HasInit");
            SelectMode = FFunc.GetBooleanValue(settings, "SelectMode");
            ClearMode = FFunc.GetBooleanValue(settings, "ClearMode");
            CopyMode = FFunc.GetBooleanValue(settings, "CopyMode", true);
            TableData = (int)FFunc.GetNumberValue(settings, "DataIndex");
            TableDetails = (int)FFunc.GetNumberValue(settings, "DetailIndex", -1);
            TableExtend = (int)FFunc.GetNumberValue(settings, "ExtendIndex", -1);
            Freeze = (int)FFunc.GetNumberValue(settings, "Freeze");
            PageItems = (int)FFunc.GetNumberValue(settings, "PageItems", 20);
            Filter = FFunc.GetStringValue(settings, "Filter");
            Title = FFunc.GetStringValue(settings, "Title");
            Order = FFunc.GetStringValue(settings, "Order").Split(",");
            Reference = FFunc.GetArrayString(FFunc.GetStringValue(settings, "Reference"));
            SubTitle = FFunc.GetStringValue(settings, "Subtitle");
            Type = FFunc.GetStringValue(settings, "Type");
            Code = Fields.FindAll(x => c.Contains(x.Name));
            Scatter = e.Length == 0 ? Code : Fields.FindAll(x => e.Contains(x.Name));
            Tips = CheckNull(p) ? new FViewPage((JObject)p) : null;
            ExpandCache = FFunc.GetBooleanValue(settings, "ExpandCache");
            IsSearchBar = FFunc.GetBooleanValue(settings, "IsSearchBar");
            PlacHolderSearchBar = FFunc.GetStringValue(settings, "PlacHolderSearchBar");
            SearchData = (int)FFunc.GetNumberValue(settings, "SearchData");
            SearchDetail = (int)FFunc.GetNumberValue(settings, "SearchDetail", -1);
            SearchCache = FFunc.GetStringValue(settings, "SearchCache") switch { "None" => ReportCacheType.None, "IIS" => ReportCacheType.IIS, "Device" => ReportCacheType.Device, _ => ReportCacheType.Dynamic };
        }

        public static bool CheckNull(JToken token)
        {
            return !(token == null || token.Type == JTokenType.Null);
        }
    }
}