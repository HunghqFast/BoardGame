using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FastMobile.FXamarin.Core
{
    public class FReportMethod
    {
        private readonly IFReportSettings report;

        public FReportMethod(IFReportSettings report)
        {
            this.report = report;
        }

        public virtual Action<object> LoadMoreItems(Func<FTargetType, bool, Task> task)
        {
            return async (obj) =>
            {
                try
                {
                    report.IsBusyGrid = true;
                    await task.Invoke(report.TargetType, false);
                }
                catch { MessagingCenter.Send(new FMessage(), FChannel.ALERT_BY_MESSAGE); }
                finally { report.IsBusyGrid = false; }
            };
        }

        public virtual bool CanLoadMoreItems(object arg)
        {
            return !(report.TotalItem <= report.Source.Count || Connectivity.NetworkAccess != NetworkAccess.Internet || report.Settings.ReportType == ReportType.Pagging || report.GridType == GridType.GridView);
        }

        public virtual void UpdatePaging(int total, IFPaging paging)
        {
            if (total == 0)
            {
                paging.ItemFrom = 0;
                paging.ItemTo = 0;
            }
            else
            {
                paging.ItemFrom = 1 + (paging.PageIndex - 1) * paging.ItemPerPage > total ? paging.ItemFrom : 1 + (paging.PageIndex - 1) * paging.ItemPerPage;
                paging.ItemTo = paging.PageIndex * paging.ItemPerPage > total ? total : paging.PageIndex * paging.ItemPerPage;
            }
            paging.ListPaging = FGridStyle.UpdatePaging(total, paging.PageIndex, paging.ItemPerPage);
        }

        public virtual void GetDataItem(ref string result, FData data)
        {
            var order = report.Settings.Order;
            var field = report.Settings.Fields;
            foreach (string name in order)
            {
                var f = field.Find(x => x.Name == name.Trim());
                if (f != null)
                {
                    try
                    {
                        result += f.FieldType switch
                        {
                            FieldType.Bool => bool.Parse(data[f.Name].ToString()) ? "1" : "0",
                            FieldType.DateTime => DateTime.Parse(data[f.Name].ToString()).ToString("yyyyMMdd"),
                            _ => data[f.Name].ToString()
                        };
                    }
                    catch { result += ""; }
                }
            }
        }

        public virtual void AddDataDetails(DataSet data, int index, bool condition)
        {
            if (!condition || data == null) return;
            var ex = report.ExtData;
            foreach (DataTable t in data.Tables)
            {
                if (t.Rows.Count == 0) continue;
                var r = t.Rows[0];
                foreach (DataColumn c in t.Columns)
                {
                    if (ex.ContainsKey(c.ColumnName)) ex[c.ColumnName] = r[c.ColumnName];
                    else ex.Add(c.ColumnName, r[c.ColumnName]);
                }
            }
            if (report.Settings.Details.Count > 0) report.Details = FData.NewItem(data.Tables[index].Rows[0], report.Settings.Details);
        }

        public virtual void AddDataExtend(DataSet data, int index)
        {
            report.ExtenderData = index == -1 || data == null || data.Tables.Count <= index ? null : data.Tables[index];
        }

        public virtual void UpdateWebView(DataRow[] dataRow)
        {
            if (dataRow.Length == 0)
            {
                report.Html = string.Empty;
                return;
            }
            var data = dataRow[0];
            var view = report.Settings.Views.Find(x => x.Id == "Item");
            var text = view.Row[0].Text;
            var item = new FData();

            foreach (var f in report.Settings.Fields)
            {
                switch (f.FieldStatus)
                {
                    case FieldStatus.Default:
                        if (!data.Table.Columns.Contains(f.Name)) continue;
                        text = text.Replace($"[{f.Name}].Title", f.Title).Replace($"[{f.Name}]", data[f.Name].ToString());
                        item[f.Name, f.FieldType] = data[f.Name];
                        break;

                    case FieldStatus.Internal:
                        text = text.Replace($"[{f.Name}].Title", f.Title).Replace($"[{f.Name}]", f.DefaultValue == null ? "" : f.DefaultValue.ToString());
                        item[f.Name, f.FieldType] = f.DefaultValue ?? "";
                        break;

                    default:
                        break;
                }
            }
            report.Html = FFunc.ReplaceHtmlText(text);
            if (report.Source.Count == 0) report.Source.Add(item);
            else report.Source[0] = item;
        }

        public virtual void AddDataSource(IFPaging paging)
        {
            if (report.GridType == GridType.WebView)
            {
                UpdateWebView(report.DataCache);
                return;
            }
            var sourceCache = report.DataCache;
            if (sourceCache == null || sourceCache.Length == 0)
            {
                report.Source = new FDataObservation();
                UpdatePaging(0, paging);
                return;
            }
            var source = new FDataObservation();
            if (report.Settings.ReportType == ReportType.Loadmore && report.GridType == GridType.ListView)
            {
                source = report.Source;
                int start = report.Source.Count;
                int end = start + paging.ItemPerPage > sourceCache.Length ? sourceCache.Length : start + paging.ItemPerPage;
                paging.ItemFrom = 1;
                paging.ItemTo = end;

                for (int i = start; i < end; i++)
                    source.Add(FData.NewItem(sourceCache[i], report.Settings.Fields));
            }
            else
            {
                UpdatePaging(sourceCache.Length, paging); ;
                for (int i = paging.ItemFrom - 1; i <= paging.ItemTo - 1; i++)
                    source.Add(FData.NewItem(sourceCache[i], report.Settings.Fields));
            }
            report.Source = source;
        }

        public virtual void AddDataSource(DataSet data, int index, int total, IFPaging paging)
        {
            var result = data.Tables.Count > 0 && index < data.Tables.Count ? data.Tables[index].Select() : new DataTable().Select();
            if (result == null) return;
            if (report.GridType == GridType.WebView)
            {
                UpdateWebView(result);
                return;
            }
            var source = report.Source;
            if (!(report.Settings.ReportType == ReportType.Loadmore && report.GridType == GridType.ListView))
            {
                UpdatePaging(total, paging);
                source = new FDataObservation();
            }
            else
            {
                paging.ItemFrom = 1;
                paging.ItemTo = report.Source.Count + result.Length;
                paging.PageIndex++;
            }
            for (int i = 0; i < result.Length; i++)
                source.Add(FData.NewItem(result[i], report.Settings.Fields));
            report.Source = source;
        }
    }
}